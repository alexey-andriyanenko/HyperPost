using HyperPost.DB;
using HyperPost.DTO.Department;
using HyperPost.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using FluentValidation;

namespace HyperPost.Controllers
{
    [Route("/departments")]
    public class DepartmentsController : Controller
    {
        private readonly HyperPostDbContext _dbContext;
        private readonly IValidator<CreateDepartmentRequest> _createDepartmentRequestValidator;
        private readonly IValidator<UpdateDepartmentRequest> _updateDepartmentRequestValidator;

        public DepartmentsController(
            HyperPostDbContext dbContext,
            IValidator<CreateDepartmentRequest> createDepartmentRequestValidator,
            IValidator<UpdateDepartmentRequest> updateDepartmentRequestValidator
        )
        {
            _dbContext = dbContext;
            _createDepartmentRequestValidator = createDepartmentRequestValidator;
            _updateDepartmentRequestValidator = updateDepartmentRequestValidator;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentResponse>> GetDepartmentById(int id)
        {
            var model = await _dbContext.Departments.FindAsync(id);
            if (model == null)
                return NotFound();

            var response = new DepartmentResponse
            {
                Id = model.Id,
                Number = model.Number,
                FullAddress = model.FullAddress
            };

            return Ok(response);
        }

        [Authorize(Policy = "admin, manager")]
        [HttpPost]
        public async Task<ActionResult<DepartmentResponse>> CreateDepartment(
            [FromBody] CreateDepartmentRequest request
        )
        {
            var validationResult = await _createDepartmentRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var model = new DepartmentModel
            {
                Number = request.Number,
                FullAddress = request.FullAddress
            };

            try
            {
                await _dbContext.Departments.AddAsync(model);
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

            var response = new DepartmentResponse
            {
                Id = model.Id,
                Number = model.Number,
                FullAddress = model.FullAddress
            };

            return Created("department", response);
        }

        [Authorize(Policy = "admin, manager")]
        [HttpPut("{id}")]
        public async Task<ActionResult<DepartmentResponse>> UpdateDepartment(
            [FromRoute] int id,
            [FromBody] UpdateDepartmentRequest request
        )
        {
            var validationResult = await _updateDepartmentRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var model = await _dbContext.Departments.FindAsync(id);
            if (model == null)
                return NotFound();

            model.FullAddress = request.FullAddress;

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

            var response = new DepartmentResponse
            {
                Id = model.Id,
                Number = model.Number,
                FullAddress = model.FullAddress
            };

            return Ok(response);
        }

        [Authorize(Policy = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDepartment(int id)
        {
            var model = await _dbContext.Departments.FindAsync(id);
            if (model == null)
                return NotFound();

            _dbContext.Departments.Remove(model);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
