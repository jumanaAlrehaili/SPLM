using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Change_Holiday_To_DateRange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Holidays_Date_Name",
                schema: "admin",
                table: "Holidays");

            migrationBuilder.RenameColumn(
                name: "Date",
                schema: "admin",
                table: "Holidays",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                schema: "admin",
                table: "Holidays",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            // Existing rows were single-day holidays; make them valid one-day ranges.
            migrationBuilder.Sql("UPDATE [admin].[Holidays] SET [EndDate] = [StartDate];");

            migrationBuilder.CreateIndex(
                name: "IX_Holidays_StartDate_EndDate",
                schema: "admin",
                table: "Holidays",
                columns: new[] { "StartDate", "EndDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Holidays_StartDate_EndDate",
                schema: "admin",
                table: "Holidays");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "admin",
                table: "Holidays");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                schema: "admin",
                table: "Holidays",
                newName: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Holidays_Date_Name",
                schema: "admin",
                table: "Holidays",
                columns: new[] { "Date", "Name" },
                unique: true);
        }
    }
}
