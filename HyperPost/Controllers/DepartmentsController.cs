﻿using HyperPost.DB;
using HyperPost.DTO.Department;
using HyperPost.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using EntityFramework.Exceptions.Common;

namespace HyperPost.Controllers
{
    [Route("/departments")]
    public class DepartmentsController : Controller
    {
        private readonly HyperPostDbContext _dbContext;

        public DepartmentsController(HyperPostDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize(Policy = "admin, manager")]
        [HttpPost]
        public async Task<ActionResult<DepartmentResponse>> CreateDepartment([FromBody] DepartmentRequest request)
        {
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
            catch(UniqueConstraintException ex)
            {
                return BadRequest(ex.Message);
            } 
            catch(MaxLengthExceededException ex)
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

    }
}
