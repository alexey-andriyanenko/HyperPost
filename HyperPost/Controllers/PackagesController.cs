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

namespace HyperPost.Controllers
{
    [Route("/packages")]
    public class PackagesController : Controller
    {
        private readonly HyperPostDbContext _dbContext;
        private readonly IValidator<CreatePackageRequest> _createPackageRequestValidator;
        private readonly IValidator<UpdatePackageRequest> _updatePackageRequestValidator;

        public PackagesController(
            HyperPostDbContext dbContext,
            IValidator<CreatePackageRequest> createPackageRequestValidator,
            IValidator<UpdatePackageRequest> updatePackageRequestValidator
        )
        {
            _dbContext = dbContext;
            _createPackageRequestValidator = createPackageRequestValidator;
            _updatePackageRequestValidator = updatePackageRequestValidator;
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PackageResponse>> GetPackageById(Guid id)
        {
            var model = await _dbContext.Packages.FindAsync(id);
            if (model == null)
                return NotFound();

            return Ok(_GetPackageResponse(model));
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

            var statuses = await _dbContext.PackageStatuses.ToListAsync();
            var createdStatus = statuses.Select(x => x).Where(x => x.Name == "created").Single();

            var model = new PackageModel
            {
                StatusId = createdStatus.Id,
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

            return Created("package", _GetPackageResponse(model));
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

            var model = await _dbContext.Packages.FindAsync(id);
            if (model == null)
                return NotFound();

            var category = await _dbContext.PackageCategoties.FindAsync(request.CategoryId);
            if (category == null)
                return BadRequest("Category with provided id was not found");

            // TODO: add statuses enum and use it instead
            var statuses = await _dbContext.PackageStatuses.ToListAsync();
            var modifiedStatus = statuses.Select(x => x).Where(x => x.Name == "modified").Single();

            model.CategoryId = request.CategoryId;
            model.Description = request.Description;
            model.StatusId = modifiedStatus.Id;
            model.ModifiedAt = DateTime.Now;

            await _dbContext.SaveChangesAsync();

            return Ok(_GetPackageResponse(model));
        }

        private PackageResponse _GetPackageResponse(PackageModel model)
        {
            return new PackageResponse
            {
                Id = model.Id,
                StatusId = model.StatusId,
                CategoryId = model.CategoryId,
                SenderUserId = model.SenderUserId,
                ReceiverUserId = model.ReceiverUserId,
                SenderDepartmentId = model.SenderDepartmentId,
                ReceiverDepartmentId = model.ReceiverDepartmentId,
                CreatedAt = model.CreatedAt,
                ModifiedAt = model.ModifiedAt,
                SentAt = model.SentAt,
                ArrivedAt = model.ArrivedAt,
                ReceivedAt = model.ReceivedAt,
                PackagePrice = model.PackagePrice,
                DeliveryPrice = model.DeliveryPrice,
                Weight = model.Weight,
                Description = model.Description
            };
        }
    }
}
