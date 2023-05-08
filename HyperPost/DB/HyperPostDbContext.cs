using HyperPost.Models;
using Microsoft.EntityFrameworkCore;

namespace HyperPost.DB
{
    public class HyperPostDbContext : DbContext
    {
        public DbSet<PackageModel> Packages { get; set; }
        public DbSet<DepartmentModel> Departments { get; set; }
        public DbSet<PackageCategoryModel> PackageCategoties { get; set; }
        public DbSet<PackageStatusModel> PackageStatuses { get; set; }
        public HyperPostDbContext(DbContextOptions<HyperPostDbContext> options) : base(options) { }
    }
}
