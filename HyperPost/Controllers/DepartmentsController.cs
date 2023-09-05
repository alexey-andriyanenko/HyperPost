using HyperPost.DB;
using HyperPost.DTO.Department;
using HyperPost.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;
using FluentValidation;
using HyperPost.DTO.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Azure;
using HyperPost.Shared;

namespace HyperPost.Controllers
{
    [Route("/departments")]
    public class DepartmentsController : Controller
    {
        private readonly HyperPostDbContext _dbContext;
        private readonly IValidator<CreateDepartmentRequest> _createDepartmentRequestValidator;
        private readonly IValidator<UpdateDepartmentRequest> _updateDepartmentRequestValidator;
        private readonly IValidator<PaginationRequest> _paginationRequestValidator;

        public DepartmentsController(
            HyperPostDbContext dbContext,
            IValidator<CreateDepartmentRequest> createDepartmentRequestValidator,
            IValidator<UpdateDepartmentRequest> updateDepartmentRequestValidator,
            IValidator<PaginationRequest> paginationRequestValidator
        )
        {
            _dbContext = dbContext;
            _createDepartmentRequestValidator = createDepartmentRequestValidator;
            _updateDepartmentRequestValidator = updateDepartmentRequestValidator;
            _paginationRequestValidator = paginationRequestValidator;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentResponse>> GetDepartmentById(int id)
        {
            var model = await _dbContext.Departments.FindAsync(id);
            if (model == null)
                return NotFound();

            return Ok(_GetDepartmentResponse(model));
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PaginationResponse<DepartmentResponse>>> GetDepartments(
            [FromQuery] PaginationRequest paginationRequest
        )
        {
            var validationResult = await _paginationRequestValidator.ValidateAsync(
                paginationRequest
            );
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var query = _dbContext.Departments.AsQueryable();
            var totalCount = await query.CountAsync();
            var totalPages = (int)System.Math.Ceiling((double)totalCount / paginationRequest.Limit);
            var list = await query
                .Skip((paginationRequest.Page - 1) * paginationRequest.Limit)
                .Take(paginationRequest.Limit)
                .ToListAsync();

            var response = new PaginationResponse<DepartmentResponse>
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                List = list.Select(_GetDepartmentResponse).ToList()
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
            {
                var error = new AppError("department-validation-error");
                error.Errors = validationResult.ToDictionary();

                return BadRequest(error);
            }

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
                var error = new AppError("department-unique-constraint-error", ex.Message);
                return BadRequest(error);
            }
            catch (MaxLengthExceededException ex)
            {
                var error = new AppError("department-max-length-exceeded-error", ex.Message);
                return BadRequest(error);
            }

            return Created("department", _GetDepartmentResponse(model));
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
            {
                var error = new AppError("department-validation-error");
                error.Errors = validationResult.ToDictionary();
                return BadRequest(error);
            }

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
                var error = new AppError("department-unique-constraint-error", ex.Message);
                return BadRequest(error);
            }
            catch (MaxLengthExceededException ex)
            {
                var error = new AppError("department-max-length-exceeded-error", ex.Message);
                return BadRequest(error);
            }

            return Ok(_GetDepartmentResponse(model));
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

        private DepartmentResponse _GetDepartmentResponse(DepartmentModel model)
        {
            return new DepartmentResponse
            {
                Id = model.Id,
                Number = model.Number,
                FullAddress = model.FullAddress
            };
        }
    }
}
