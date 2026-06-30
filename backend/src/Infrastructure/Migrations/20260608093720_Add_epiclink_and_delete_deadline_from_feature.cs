using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_epiclink_and_delete_deadline_from_feature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "Features");

            migrationBuilder.AddColumn<string>(
                name: "EpicLink",
                table: "Features",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EpicLink",
                table: "Features");

            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "Features",
                type: "datetime2",
                nullable: true);
        }
    }
}
