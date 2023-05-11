using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyperPost.Migrations
{
    /// <inheritdoc />
    public partial class DepartmentNumberUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Department_Number",
                table: "Department",
                column: "Number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Department_Number",
                table: "Department");
        }
    }
}
