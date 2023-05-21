﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HyperPost.Migrations
{
    /// <inheritdoc />
    public partial class PackageStatusArchived : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PackageStatus",
                columns: new[] { "Id", "Name" },
                values: new object[] { 5, "archived" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PackageStatus",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
