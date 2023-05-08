using HyperPost.Models;
using Microsoft.EntityFrameworkCore;

namespace HyperPost.DB
{
    public class HyperPostDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserRoleModel> Roles { get; set; }
        public DbSet<PackageModel> Packages { get; set; }
        public DbSet<DepartmentModel> Departments { get; set; }
        public DbSet<PackageCategoryModel> PackageCategoties { get; set; }
        public DbSet<PackageStatusModel> PackageStatuses { get; set; }
        public HyperPostDbContext(DbContextOptions<HyperPostDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserModel>()
                .HasData(
                    new UserModel
                    {
                        Id = 1,
                        RoleId = 1,
                        FirstName = "Admin",
                        LastName = "User",
                        PhoneNumber = "111111",
                        Email = "admin@example.com",
                        Password = "root",
                    },
                    new UserModel
                    {
                        Id = 2,
                        RoleId = 2,
                        FirstName = "Manager",
                        LastName = "User",
                        PhoneNumber = "222222",
                        Email = "manager@example.com",
                        Password = "manager_password",
                    },
                    new UserModel
                    {
                        Id = 3,
                        RoleId = 3,
                        FirstName = "Client",
                        LastName = "User",
                        PhoneNumber = "333333",
                        Email = "client@example.com",
                        Password = "client_password",
                    }
                );

            modelBuilder.Entity<UserRoleModel>()
                .HasData(
                    new UserRoleModel { Id = 1, Name = "admin" },
                    new UserRoleModel { Id = 2, Name = "manager" },
                    new UserRoleModel { Id = 3, Name = "client" }
                );

            modelBuilder.Entity<PackageStatusModel>()
                .HasData(
                    new PackageStatusModel { Id = 1, Name = "created" },
                    new PackageStatusModel { Id = 2, Name = "sent" },
                    new PackageStatusModel { Id = 3, Name = "arrived" },
                    new PackageStatusModel { Id = 4, Name = "received" }
                );

            modelBuilder.Entity<PackageCategoryModel>()
                .HasData(
                    new PackageCategoryModel { Id = 1, Name = "Food" },
                    new PackageCategoryModel { Id = 2, Name = "Money" },
                    new PackageCategoryModel { Id = 3, Name = "Medicaments" },
                    new PackageCategoryModel { Id = 4, Name = "Accumulators" },
                    new PackageCategoryModel { Id = 5, Name = "Sports Products" },
                    new PackageCategoryModel { Id = 6, Name = "Clothes" },
                    new PackageCategoryModel { Id = 7, Name = "Shoes" },
                    new PackageCategoryModel { Id = 8, Name = "Documents" },
                    new PackageCategoryModel { Id = 9, Name = "Books" },
                    new PackageCategoryModel { Id = 10, Name = "Computers" },
                    new PackageCategoryModel { Id = 11, Name = "Accessories" }
                );
        }
    }
}
