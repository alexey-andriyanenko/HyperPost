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
        private readonly IValidator<CreatePackageRequest> _packageRequestValidator;

        public PackagesController(HyperPostDbContext dbContext)
        {
            _dbContext = dbContext;
            _packageRequestValidator = new CreatePackageRequestValidator();
        }

        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PackageResponse>> GetPackageById(Guid id)
        {
            var model = await _dbContext.Packages.FindAsync(id);
            if (model == null)
                return NotFound();

            var response = new PackageResponse
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
                Weight = model.Weight
            };

            return Ok(response);
        }

        [Authorize(Policy = "admin, manager")]
        [HttpPost]
        public async Task<ActionResult<PackageResponse>> CreatePackage(
            [FromBody] CreatePackageRequest request
        )
        {
            var validationResult = await _packageRequestValidator.ValidateAsync(request);
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
            };

            await _dbContext.Packages.AddAsync(model);
            await _dbContext.SaveChangesAsync();

            var response = new PackageResponse
            {
                Id = model.Id,
                StatusId = model.StatusId,
                CreatedAt = model.CreatedAt,
                SenderUserId = model.SenderUserId,
                ReceiverUserId = model.ReceiverUserId,
                SenderDepartmentId = model.SenderDepartmentId,
                ReceiverDepartmentId = model.ReceiverDepartmentId,
                PackagePrice = model.PackagePrice,
                DeliveryPrice = model.DeliveryPrice,
                Weight = model.Weight,
            };

            return Created("package", response);
        }
    }
}
