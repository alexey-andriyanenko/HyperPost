using EntityFramework.Exceptions.Common;
using FluentValidation;
using FluentValidation.Validators;
using HyperPost.DB;
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
        private readonly IValidator<UserRequest> _userRequestValidator;

        public UsersController(HyperPostDbContext dbContext, IValidator<UserRequest> userRequestValidator)
        {
            _dbContext = dbContext;
            _userRequestValidator = userRequestValidator;
        }

        [HttpPost("login/email")]
        public async Task<ActionResult<UserLoginResponse>> LoginViaEmail([FromBody] UserLoginViaEmailRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);
            if (user == null) return Unauthorized();

            return Ok(await _GetUserLoginResponse(user));
        }

        [HttpPost("login/phone")]
        public async Task<ActionResult<UserLoginResponse>> LoginViaPhoneNumber([FromBody] UserLoginViaPhoneNumberRequest request)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber && u.Password == request.Password);
            if (user == null) return Unauthorized();

            return Ok(await _GetUserLoginResponse(user));
        }

        [Authorize(Policy = "admin, manager")]
        [HttpPost]
        public async Task<ActionResult<UserResponse>> CreateUser([FromBody] UserRequest request)
        {
            var role = HttpContext.User.Claims.Single(c => c.Type == "Role").Value;

            if (role != "admin" && (request.RoleId == (int)UserRolesEnum.Admin || request.RoleId == (int)UserRolesEnum.Manager))
            {
                return Forbid();
            }

            var validationResult = await _userRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)  return BadRequest(validationResult.Errors);

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

            return Ok();
        }

        private UserResponse _GetUserResponse(UserModel model)
        {
            return new UserResponse
            {
                Id = model.Id,
                RoleId = model.RoleId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
            };
        }

        private async Task<UserLoginResponse> _GetUserLoginResponse(UserModel model)
        {
            return new UserLoginResponse
            {
                Id = model.Id,
                AccessToken = await _GetJwtToken(model)
            };
        }

        private async Task<string> _GetJwtToken(UserModel model)
        {
            var role = await _dbContext.Roles.SingleAsync(r => r.Id == model.RoleId);
            var claims = new List<Claim>
            {
                new Claim("Id", model.Id.ToString()),
                new Claim("FirstName", model.FirstName),
                new Claim("LastName", model.LastName),
                new Claim("Email", model.Email),
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
                signingCredentials: new SigningCredentials(AuthService.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
            );

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

            return encodedJwt;
        }
    }
}
