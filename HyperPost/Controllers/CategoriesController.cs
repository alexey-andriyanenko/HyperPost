using FluentValidation;
using HyperPost.DB;
using HyperPost.Models;
using HyperPost.DTO.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using EntityFramework.Exceptions.Common;

namespace HyperPost.Controllers
{
    [Route("/package/categories")]
    public class CategoriesController : Controller
    {
        private readonly HyperPostDbContext _dbContext;
        private readonly IValidator<PackageCategoryRequest> _categoryRequestValidator;

        public CategoriesController(
            HyperPostDbContext dbContext,
            IValidator<PackageCategoryRequest> categoryRequestValidator
        )
        {
            _dbContext = dbContext;
            _categoryRequestValidator = categoryRequestValidator;
        }

        [Authorize(Policy = "admin")]
        [HttpPost]
        public async Task<ActionResult<PackageCategoryResponse>> CreateCategory(
            [FromBody] PackageCategoryRequest category
        )
        {
            var validationResult = await _categoryRequestValidator.ValidateAsync(category);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var model = new PackageCategoryModel { Name = category.Name };

            try
            {
                await _dbContext.PackageCategoties.AddAsync(model);
                await _dbContext.SaveChangesAsync();
            }
            catch (MaxLengthExceededException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UniqueConstraintException ex)
            {
                return BadRequest(ex.Message);
            }

            var response = new PackageCategoryResponse { Id = model.Id, Name = model.Name };
            return Created("category", response);
        }
    }
}
