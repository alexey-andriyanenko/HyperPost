using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HyperPost.Migrations
{
    /// <inheritdoc />
    public partial class InitialSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Department",
                columns: new[] { "Id", "FullAddress", "Number" },
                values: new object[,]
                {
                    { 1, "HyperPost Department #1, 5331 Rexford Court, Montgomery AL 36116", 1 },
                    { 2, "HyperPost Department #2, 6095 Terry Lane, Golden CO 80403", 2 },
                    { 3, "HyperPost Department #3, 1002 Hilltop Drive, Dalton GA 30720", 3 },
                    { 4, "HyperPost Department #4, 2325 Eastridge Circle, Moore OK 73160", 4 },
                    { 5, "HyperPost Department #5, 100219141 Pine Ridge Circle, Anchorage AK 99516", 5 },
                    { 6, "HyperPost Department #6, 5275 North 59th Avenue, Glendale AZ 85301", 6 },
                    { 7, "HyperPost Department #7, 5985 Lamar Street, Arvada CO 80003", 7 },
                    { 8, "HyperPost Department #8, 136 Acacia Drive, Blue Lake CA 95525", 8 },
                    { 9, "HyperPost Department #9, 7701 Taylor Oaks Circle, Montgomery AL 36116", 9 },
                    { 10, "HyperPost Department #10, 243 Kentucky Avenue, Pasadena MD 21122", 10 }
                });

            migrationBuilder.InsertData(
                table: "PackageCategory",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Food" },
                    { 2, "Money" },
                    { 3, "Medicaments" },
                    { 4, "Accumulators" },
                    { 5, "Sports Products" },
                    { 6, "Clothes" },
                    { 7, "Shoes" },
                    { 8, "Documents" },
                    { 9, "Books" },
                    { 10, "Computers" },
                    { 11, "Accessories" }
                });

            migrationBuilder.InsertData(
                table: "PackageStatus",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "created" },
                    { 2, "sent" },
                    { 3, "arrived" },
                    { 4, "received" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Password", "PhoneNumber", "RoleId" },
                values: new object[,]
                {
                    { 1, "admin@example.com", "Admin", "User", "root", "111111", 1 },
                    { 2, "manager@example.com", "Manager", "User", "manager_password", "222222", 2 },
                    { 3, "client@example.com", "Client", "User", "client_password", "333333", 3 }
                });

            migrationBuilder.InsertData(
                table: "UserRole",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "admin" },
                    { 2, "manager" },
                    { 3, "client" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Department",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "PackageCategory",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PackageCategory",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PackageCategory",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PackageCategory",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PackageCategory",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "PackageCategory",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "PackageCategory",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "PackageCategory",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "PackageCategory",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "PackageCategory",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "PackageCategory",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "PackageStatus",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PackageStatus",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PackageStatus",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PackageStatus",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserRole",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
