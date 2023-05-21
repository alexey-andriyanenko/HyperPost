using EntityFramework.Exceptions.SqlServer;
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

        public HyperPostDbContext(DbContextOptions<HyperPostDbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseExceptionProcessor();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<UserModel>()
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

            modelBuilder
                .Entity<UserRoleModel>()
                .HasData(
                    new UserRoleModel { Id = 1, Name = "admin" },
                    new UserRoleModel { Id = 2, Name = "manager" },
                    new UserRoleModel { Id = 3, Name = "client" }
                );

            modelBuilder
                .Entity<PackageStatusModel>()
                .HasData(
                    new PackageStatusModel { Id = 1, Name = "created" },
                    new PackageStatusModel { Id = 2, Name = "sent" },
                    new PackageStatusModel { Id = 3, Name = "arrived" },
                    new PackageStatusModel { Id = 4, Name = "received" },
                    new PackageStatusModel { Id = 5, Name = "archived" },
                    new PackageStatusModel { Id = 6, Name = "modified" }
                );

            modelBuilder
                .Entity<PackageCategoryModel>()
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

            modelBuilder
                .Entity<DepartmentModel>()
                .HasData(
                    new DepartmentModel
                    {
                        Id = 1,
                        Number = 1,
                        FullAddress =
                            "HyperPost Department #1, 5331 Rexford Court, Montgomery AL 36116"
                    },
                    new DepartmentModel
                    {
                        Id = 2,
                        Number = 2,
                        FullAddress = "HyperPost Department #2, 6095 Terry Lane, Golden CO 80403"
                    },
                    new DepartmentModel
                    {
                        Id = 3,
                        Number = 3,
                        FullAddress = "HyperPost Department #3, 1002 Hilltop Drive, Dalton GA 30720"
                    },
                    new DepartmentModel
                    {
                        Id = 4,
                        Number = 4,
                        FullAddress =
                            "HyperPost Department #4, 2325 Eastridge Circle, Moore OK 73160"
                    },
                    new DepartmentModel
                    {
                        Id = 5,
                        Number = 5,
                        FullAddress =
                            "HyperPost Department #5, 100219141 Pine Ridge Circle, Anchorage AK 99516"
                    },
                    new DepartmentModel
                    {
                        Id = 6,
                        Number = 6,
                        FullAddress =
                            "HyperPost Department #6, 5275 North 59th Avenue, Glendale AZ 85301"
                    },
                    new DepartmentModel
                    {
                        Id = 7,
                        Number = 7,
                        FullAddress = "HyperPost Department #7, 5985 Lamar Street, Arvada CO 80003"
                    },
                    new DepartmentModel
                    {
                        Id = 8,
                        Number = 8,
                        FullAddress =
                            "HyperPost Department #8, 136 Acacia Drive, Blue Lake CA 95525"
                    },
                    new DepartmentModel
                    {
                        Id = 9,
                        Number = 9,
                        FullAddress =
                            "HyperPost Department #9, 7701 Taylor Oaks Circle, Montgomery AL 36116"
                    },
                    new DepartmentModel
                    {
                        Id = 10,
                        Number = 10,
                        FullAddress =
                            "HyperPost Department #10, 243 Kentucky Avenue, Pasadena MD 21122"
                    }
                );
        }
    }
}
