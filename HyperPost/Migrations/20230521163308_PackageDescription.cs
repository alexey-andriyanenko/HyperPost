using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyperPost.Migrations
{
    /// <inheritdoc />
    public partial class PackageDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Package",
                type: "nvarchar(50)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Package");
        }
    }
}
