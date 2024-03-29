﻿using Azure.Core;
using EntityFramework.Exceptions.Common;
using FluentValidation;
using FluentValidation.Validators;
using HyperPost.DB;
using HyperPost.DTO.Package;
using HyperPost.DTO.Pagination;
using HyperPost.DTO.User;
using HyperPost.Models;
using HyperPost.Services;
using HyperPost.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HyperPost.Controllers
{
    [Route("/users")]
    public class UsersController : Controller
    {
        private readonly HyperPostDbContext _dbContext;
        private readonly IValidator<CreateUserRequest> _createUserRequestValidator;
        private readonly IValidator<UpdateUserRequest> _updateUserRequestValidator;
        private readonly IValidator<UpdateMeRequest> _updateMeRequestValidator;
        private readonly IValidator<PaginationRequest> _paginationRequestValidator;
        private readonly IValidator<CheckIfUserExistsRequest> _checkIfUserExistsRequestValidator;

        public UsersController(
            HyperPostDbContext dbContext,
            IValidator<CreateUserRequest> createUserRequestValidator,
            IValidator<UpdateUserRequest> updateUserRequestValidator,
            IValidator<UpdateMeRequest> updateMeRequestValidator,
            IValidator<PaginationRequest> paginationRequestValidator,
            IValidator<CheckIfUserExistsRequest> checkIfUserExistsRequestValidator
        )
        {
            _dbContext = dbContext;
            _createUserRequestValidator = createUserRequestValidator;
            _updateUserRequestValidator = updateUserRequestValidator;
            _updateMeRequestValidator = updateMeRequestValidator;
            _paginationRequestValidator = paginationRequestValidator;
            _checkIfUserExistsRequestValidator = checkIfUserExistsRequestValidator;
        }

        [HttpPost("login/email")]
        public async Task<ActionResult<UserLoginResponse>> LoginViaEmail(
            [FromBody] UserLoginViaEmailRequest request
        )
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(
                u => u.Email == request.Email && u.Password == request.Password
            );
            if (user == null)
                return Unauthorized();

            return Ok(await _GetUserLoginResponse(user));
        }

        [HttpPost("login/phone")]
        public async Task<ActionResult<UserLoginResponse>> LoginViaPhoneNumber(
            [FromBody] UserLoginViaPhoneNumberRequest request
        )
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(
                u => u.PhoneNumber == request.PhoneNumber && u.Password == request.Password
            );
            if (user == null)
                return Unauthorized();

            return Ok(await _GetUserLoginResponse(user));
        }

        [Authorize(Policy = "admin, manager")]
        [HttpPost]
        public async Task<ActionResult<UserResponse>> CreateUser(
            [FromBody] CreateUserRequest request
        )
        {
            /* description
             * admin can create any user
             * manager can create only client users
             */

            var roleId = int.Parse(HttpContext.User.Claims.Single(c => c.Type == "RoleId").Value);

            if (
                roleId != (int)UserRolesEnum.Admin
                && (
                    request.RoleId == (int)UserRolesEnum.Admin
                    || request.RoleId == (int)UserRolesEnum.Manager
                )
            )
            {
                return Forbid();
            }

            var validationResult = await _createUserRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var user = new UserModel
            {
                RoleId = request.RoleId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Password = request.Password,
            };

            try
            {
                await _dbContext.Users.AddAsync(user);
                await _dbContext.SaveChangesAsync();
            }
            catch (UniqueConstraintException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (MaxLengthExceededException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(user.ToResponse());
        }

        [Authorize(Policy = "admin, manager")]
        [HttpGet]
        public async Task<ActionResult<PaginationResponse<UserResponse>>> GetUsers(
            [FromRoute] PaginationRequest paginationRequest
        )
        {
            /* Description ↓
             * Admins can access all users
             * Managers can acess only client users
             */

            var roleId = int.Parse(HttpContext.User.Claims.Single(c => c.Type == "RoleId").Value);

            var validationResult = await _paginationRequestValidator.ValidateAsync(
                paginationRequest
            );
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var query = _dbContext.Users.AsQueryable();
            var models = await query
                .Skip(paginationRequest.Limit * (paginationRequest.Page - 1))
                .Take(paginationRequest.Limit)
                .ToListAsync();

            if (roleId == (int)UserRolesEnum.Manager)
                models = models.Where(u => u.RoleId == (int)UserRolesEnum.Client).ToList();

            var totalCount = models.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / paginationRequest.Limit);

            var response = new PaginationResponse<UserResponse>
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                List = models.Select(ps => ps.ToResponse()).ToList()
            };

            return Ok(response);
        }

        [Authorize(Policy = "admin, manager")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUserById(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return NotFound($"User with id={id} not found");

            return Ok(user.ToResponse());
        }

        [Authorize(Policy = "admin, manager")]
        [HttpGet("check/exists")]
        public async Task<ActionResult<UserResponse>> CheckIfUserExists(
            [FromQuery] CheckIfUserExistsRequest request
        )
        {
            var validationResult = await _checkIfUserExistsRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var error = new AppError("request-validation-fail");
                error.Errors = validationResult.ToDictionary();

                return BadRequest(error);
            }

            UserModel? user = null;

            if (request.Email != null)
            {
                user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            }
            else
            {
                user = await _dbContext.Users.FirstOrDefaultAsync(
                    u => u.PhoneNumber == request.Phone
                );
            }

            if (user == null)
                return NotFound();

            return Ok(user.ToResponse());
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserResponse>> GetMe()
        {
            var userId = int.Parse(HttpContext.User.Claims.Single(c => c.Type == "Id").Value);
            var user = await _dbContext.Users.FindAsync(userId);

            if (user == null)
                return NotFound($"User with id={userId} not found");

            return Ok(user.ToResponse());
        }

        [Authorize(Policy = "admin, manager")]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponse>> UpdateUser(
            [FromRoute] int id,
            [FromBody] UpdateUserRequest request
        )
        {
            /* description
             * admin can update only manager and client users
             * manager can update only client users
             */

            var roleId = int.Parse(HttpContext.User.Claims.Single(c => c.Type == "RoleId").Value);

            if (roleId == (int)UserRolesEnum.Admin && request.RoleId == (int)UserRolesEnum.Admin)
                return Forbid();

            if (
                roleId == (int)UserRolesEnum.Manager
                && (
                    request.RoleId == (int)UserRolesEnum.Admin
                    || request.RoleId == (int)UserRolesEnum.Manager
                )
            )
                return Forbid();

            var validationResult = await _updateUserRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return NotFound($"User with id={id} not found");

            // NOTE: editing email if it is already set is not yet supported
            if (user.Email == null && (user.Email != request.Email))
            {
                user.Email = request.Email;
            }
            if (user.Email != null && (user.Email != request.Email))
            {
                return BadRequest("Email cannot be changed");
            }

            user.RoleId = request.RoleId;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (UniqueConstraintException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (MaxLengthExceededException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(user.ToResponse());
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<ActionResult<UserResponse>> UpdateMe([FromBody] UpdateMeRequest request)
        {
            var userId = int.Parse(HttpContext.User.Claims.Single(c => c.Type == "Id").Value);

            var validationResult = await _updateMeRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var user = await _dbContext.Users.SingleAsync(u => u.Id == userId);

            // NOTE: editing email if it is already set is not yet supported
            if (user.Email == null && (user.Email != request.Email))
            {
                user.Email = request.Email;
            }
            if (user.Email != null && (user.Email != request.Email))
            {
                return BadRequest("Email cannot be changed");
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (UniqueConstraintException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (MaxLengthExceededException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(user.ToResponse());
        }

        [Authorize(Policy = "admin, manager")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            /* description
             * admin can delete any user
             * manager can delete only client user
             */

            var roleId = int.Parse(HttpContext.User.Claims.Single(c => c.Type == "RoleId").Value);
            var user = await _dbContext.Users.FindAsync(id);

            if (user == null)
                return NotFound($"User with id={id} not found");

            if (roleId == (int)UserRolesEnum.Manager && user.RoleId != (int)UserRolesEnum.Client)
                return Forbid();

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }

        private async Task<UserLoginResponse> _GetUserLoginResponse(UserModel model)
        {
            return new UserLoginResponse { Id = model.Id, AccessToken = await _GetJwtToken(model) };
        }

        private async Task<string> _GetJwtToken(UserModel model)
        {
            var role = await _dbContext.Roles.SingleAsync(r => r.Id == model.RoleId);
            var claims = new List<Claim>
            {
                new Claim("Id", model.Id.ToString()),
                new Claim("FirstName", model.FirstName),
                new Claim("LastName", model.LastName),
                new Claim("Email", model.Email ?? ""),
                new Claim("PhoneNumber", model.PhoneNumber),
                new Claim("RoleId", role.Id.ToString()),
                new Claim("Role", role.Name),
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: AuthService.ISSUER,
                audience: AuthService.AUDIENCE,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(30),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    AuthService.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256
                )
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

            return encodedJwt;
        }
    }
}
