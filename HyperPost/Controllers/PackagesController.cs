using System;
using System.Linq;
using HyperPost.DB;
using HyperPost.DTO.Package;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FluentValidation;
using HyperPost.Models;
using Microsoft.EntityFrameworkCore;
using HyperPost.DTO.Pagination;
using HyperPost.Shared;
using HyperPost.DTO.PackageCategory;
using HyperPost.DTO.Department;
using HyperPost.DTO.User;

namespace HyperPost.Controllers
{
    [Route("/packages")]
    public class PackagesController : Controller
    {
        private readonly HyperPostDbContext _dbContext;
        private readonly IValidator<CreatePackageRequest> _createPackageRequestValidator;
        private readonly IValidator<UpdatePackageRequest> _updatePackageRequestValidator;
        private readonly IValidator<PaginationRequest> _paginationRequestValidator;

        public PackagesController(
            HyperPostDbContext dbContext,
            IValidator<CreatePackageRequest> createPackageRequestValidator,
            IValidator<UpdatePackageRequest> updatePackageRequestValidator,
            IValidator<PaginationRequest> paginationRequestValidator
        )
        {
            _dbContext = dbContext;
            _createPackageRequestValidator = createPackageRequestValidator;
            _updatePackageRequestValidator = updatePackageRequestValidator;
            _paginationRequestValidator = paginationRequestValidator;
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PackageResponse>> GetPackageById(Guid id)
        {
            var model = await _dbContext.Packages
                .Include(p => p.SenderUser)
                .Include(p => p.ReceiverUser)
                .Include(p => p.SenderDepartment)
                .Include(p => p.ReceiverDepartment)
                .Include(p => p.Category)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (model == null)
                return NotFound();

            return Ok(model.ToResponse());
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PaginationResponse<PackageResponse>>> GetPackages(
            [FromQuery] PaginationRequest paginationRequest
        )
        {
            var validationResult = await _paginationRequestValidator.ValidateAsync(
                paginationRequest
            );
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var roleId = int.Parse(HttpContext.User.Claims.Single(c => c.Type == "RoleId").Value);
            var query = _dbContext.Packages.AsQueryable();

            var models = await query
                .Skip(paginationRequest.Limit * (paginationRequest.Page - 1))
                .Take(paginationRequest.Limit)
                .Include(p => p.SenderUser)
                .Include(p => p.ReceiverUser)
                .Include(p => p.SenderDepartment)
                .Include(p => p.ReceiverDepartment)
                .Include(p => p.Category)
                .ToListAsync();

            if (roleId == (int)UserRolesEnum.Client)
            {
                var userId = int.Parse(HttpContext.User.Claims.Single(c => c.Type == "Id").Value);
                models = models
                    .Where(x => x.ReceiverUserId == userId || x.SenderUserId == userId)
                    .ToList();
            }

            var totalCount = models.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / paginationRequest.Limit);

            var response = new PaginationResponse<PackageResponse>
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                List = models.Select(x => x.ToResponse()).ToList(),
            };

            return Ok(response);
        }

        [Authorize(Policy = "admin, manager")]
        [HttpPost]
        public async Task<ActionResult<PackageResponse>> CreatePackage(
            [FromBody] CreatePackageRequest request
        )
        {
            var validationResult = await _createPackageRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var senderUser = await _dbContext.Users.FindAsync(request.SenderUserId);
            if (senderUser == null)
                return BadRequest("Sender user with provided id was not found");

            var reciverUser = await _dbContext.Users.FindAsync(request.ReceiverUserId);
            if (reciverUser == null)
                return BadRequest("Receiver user with provided id was not found");

            var senderDepartment = await _dbContext.Departments.FindAsync(
                request.SenderDepartmentId
            );
            if (senderDepartment == null)
                return BadRequest("Sender department with provided id was not found");

            var receiverDepartment = await _dbContext.Departments.FindAsync(
                request.ReceiverDepartmentId
            );
            if (receiverDepartment == null)
                return BadRequest("Receiver department with provided id was not found");

            var category = await _dbContext.PackageCategoties.FindAsync(request.CategoryId);
            if (category == null)
                return BadRequest("Category with provided id was not found");

            var model = new PackageModel
            {
                StatusId = (int)PackageStatusesEnum.Created,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.Now,
                SenderUserId = request.SenderUserId,
                ReceiverUserId = request.ReceiverUserId,
                SenderDepartmentId = request.SenderDepartmentId,
                ReceiverDepartmentId = request.ReceiverDepartmentId,
                PackagePrice = request.PackagePrice,
                DeliveryPrice = request.DeliveryPrice,
                Weight = request.Weight,
                Description = request.Description,
            };

            await _dbContext.Packages.AddAsync(model);
            await _dbContext.SaveChangesAsync();

            return Created("package", model.ToResponse());
        }

        [Authorize(Policy = "admin, manager")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PackageResponse>> UpdatePackage(
            [FromRoute] Guid id,
            [FromBody] UpdatePackageRequest request
        )
        {
            var validationResult = await _updatePackageRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var model = await _dbContext.Packages
                .Include(p => p.SenderUser)
                .Include(p => p.ReceiverUser)
                .Include(p => p.SenderDepartment)
                .Include(p => p.ReceiverDepartment)
                .Include(p => p.Category)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (model == null)
                return NotFound();

            var category = await _dbContext.PackageCategoties.FindAsync(request.CategoryId);
            if (category == null)
                return BadRequest("Category with provided id was not found");

            model.CategoryId = request.CategoryId;
            model.Description = request.Description;
            model.StatusId = (int)PackageStatusesEnum.Modified;
            model.ModifiedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return Ok(model.ToResponse());
        }

        [Authorize(Policy = "admin, manager")]
        [HttpPatch("{id:guid}/archive")]
        public async Task<ActionResult<PackageResponse>> ArchivePackage(Guid id)
        {
            var model = await _dbContext.Packages
                .Include(p => p.SenderUser)
                .Include(p => p.ReceiverUser)
                .Include(p => p.SenderDepartment)
                .Include(p => p.ReceiverDepartment)
                .Include(p => p.Category)
                .SingleOrDefaultAsync(p => p.Id == id);
            if (model == null)
                return NotFound();

            model.StatusId = (int)PackageStatusesEnum.Archived;
            model.ArchivedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return Ok(model.ToResponse());
        }
    }
}
