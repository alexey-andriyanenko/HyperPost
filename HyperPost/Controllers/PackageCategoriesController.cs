using FluentValidation;
using HyperPost.DB;
using HyperPost.Models;
using HyperPost.DTO.PackageCategory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using EntityFramework.Exceptions.Common;
using HyperPost.DTO.Package;
using HyperPost.DTO.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using HyperPost.Shared;

namespace HyperPost.Controllers
{
    [Route("/package/categories")]
    public class PackageCategoriesController : Controller
    {
        private readonly HyperPostDbContext _dbContext;
        private readonly IValidator<CreatePackageCategoryRequest> _categoryRequestValidator;
        private readonly IValidator<UpdatePackageCategoryRequest> _updateCategoryRequestValidator;
        private readonly IValidator<PaginationRequest> _paginationRequestValidator;

        public PackageCategoriesController(
            HyperPostDbContext dbContext,
            IValidator<CreatePackageCategoryRequest> categoryRequestValidator,
            IValidator<UpdatePackageCategoryRequest> updateCategoryRequestValidator,
            IValidator<PaginationRequest> paginationRequestValidator
        )
        {
            _dbContext = dbContext;
            _categoryRequestValidator = categoryRequestValidator;
            _updateCategoryRequestValidator = updateCategoryRequestValidator;
            _paginationRequestValidator = paginationRequestValidator;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<PackageCategoryResponse>> GetCategoryById([FromRoute] int id)
        {
            var model = await _dbContext.PackageCategoties.FindAsync(id);
            if (model == null)
                return NotFound();

            return Ok(model.ToResponse());
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PaginationResponse<PackageCategoryResponse>>> GetCategories(
            [FromRoute] PaginationRequest paginationRequest
        )
        {
            var validationResult = await _paginationRequestValidator.ValidateAsync(
                paginationRequest
            );
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var query = _dbContext.PackageCategoties.AsQueryable();
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / paginationRequest.Limit);
            var list = await query
                .Skip((paginationRequest.Page - 1) * paginationRequest.Limit)
                .Take(paginationRequest.Limit)
                .ToListAsync();
            var response = new PaginationResponse<PackageCategoryResponse>
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                List = list.Select(pc => pc.ToResponse()).ToList()
            };

            return Ok(response);
        }

        [Authorize(Policy = "admin")]
        [HttpPost]
        public async Task<ActionResult<PackageCategoryResponse>> CreateCategory(
            [FromBody] CreatePackageCategoryRequest category
        )
        {
            var validationResult = await _categoryRequestValidator.ValidateAsync(category);
            if (!validationResult.IsValid)
            {
                var error = new AppError("package-category-validation-error");
                error.Errors = validationResult.ToDictionary();

                return BadRequest(error);
            }

            var model = new PackageCategoryModel { Name = category.Name };

            try
            {
                await _dbContext.PackageCategoties.AddAsync(model);
                await _dbContext.SaveChangesAsync();
            }
            catch (MaxLengthExceededException ex)
            {
                var error = new AppError("package-category-max-length-exceeded-error", ex.Message);
                return BadRequest(error);
            }
            catch (UniqueConstraintException ex)
            {
                var error = new AppError("package-category-unique-constraint-error", ex.Message);
                return BadRequest(error);
            }

            return Created("category", model.ToResponse());
        }

        [Authorize(Policy = "admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<PackageResponse>> UpdateCategory(
            [FromRoute] int id,
            [FromBody] UpdatePackageCategoryRequest category
        )
        {
            var validationResult = await _updateCategoryRequestValidator.ValidateAsync(category);
            if (!validationResult.IsValid)
            {
                var error = new AppError("package-category-validation-error");
                error.Errors = validationResult.ToDictionary();

                return BadRequest(error);
            }

            var model = await _dbContext.PackageCategoties.FindAsync(id);
            if (model == null)
                return NotFound();

            model.Name = category.Name;

            try
            {
                _dbContext.PackageCategoties.Update(model);
                await _dbContext.SaveChangesAsync();
            }
            catch (UniqueConstraintException ex)
            {
                var error = new AppError("package-category-unique-constraint-error", ex.Message);
                return BadRequest(error);
            }
            catch (MaxLengthExceededException ex)
            {
                var error = new AppError("package-category-max-length-exceeded-error", ex.Message);
                return BadRequest(error);
            }

            return Ok(model.ToResponse());
        }

        [Authorize(Policy = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var model = await _dbContext.PackageCategoties.FindAsync(id);
            if (model == null)
                return NotFound();

            _dbContext.PackageCategoties.Remove(model);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
