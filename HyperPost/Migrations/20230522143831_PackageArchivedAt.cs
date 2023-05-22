using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyperPost.Migrations
{
    /// <inheritdoc />
    public partial class PackageArchivedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "Package",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "Package");
        }
    }
}
