using FluentValidation;
using HyperPost.DB;
using HyperPost.DTO.PackageStatus;
using HyperPost.DTO.Pagination;
using HyperPost.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperPost.Controllers
{
    [Route("/packages/statuses")]
    public class PackageStatusesController : Controller
    {
        private readonly HyperPostDbContext _dbContext;

        public PackageStatusesController(HyperPostDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Authorize(Policy = "admin, manager")]
        [HttpGet]
        public async Task<ActionResult<List<PackageStatusResponse>>> GetStatuses()
        {
            var query = _dbContext.PackageStatuses.AsQueryable();
            var statuses = await query.ToListAsync();

            return Ok(statuses.Select(_GetPackageStatusResponse).ToList());
        }

        private PackageStatusResponse _GetPackageStatusResponse(PackageStatusModel model)
        {
            return new PackageStatusResponse { Id = model.Id, Name = model.Name };
        }
    }
}
