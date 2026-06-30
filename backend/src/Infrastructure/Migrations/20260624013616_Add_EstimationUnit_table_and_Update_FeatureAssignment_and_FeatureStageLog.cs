using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_EstimationUnit_table_and_Update_FeatureAssignment_and_FeatureStageLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Estimation",
                schema: "feature",
                table: "FeatureAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstimationUnitId",
                schema: "feature",
                table: "FeatureAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                schema: "feature",
                table: "FeatureAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EstimationUnits",
                schema: "lookup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimationUnits", x => x.Id);
                });

            migrationBuilder.InsertData(
                schema: "lookup",
                table: "EstimationUnits",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Hours" },
                    { 2, "Days" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeatureAssignments_EstimationUnitId",
                schema: "feature",
                table: "FeatureAssignments",
                column: "EstimationUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureAssignments_EstimationUnits_EstimationUnitId",
                schema: "feature",
                table: "FeatureAssignments",
                column: "EstimationUnitId",
                principalSchema: "lookup",
                principalTable: "EstimationUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeatureAssignments_EstimationUnits_EstimationUnitId",
                schema: "feature",
                table: "FeatureAssignments");

            migrationBuilder.DropTable(
                name: "EstimationUnits",
                schema: "lookup");

            migrationBuilder.DropIndex(
                name: "IX_FeatureAssignments_EstimationUnitId",
                schema: "feature",
                table: "FeatureAssignments");

            migrationBuilder.DropColumn(
                name: "Estimation",
                schema: "feature",
                table: "FeatureAssignments");

            migrationBuilder.DropColumn(
                name: "EstimationUnitId",
                schema: "feature",
                table: "FeatureAssignments");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                schema: "feature",
                table: "FeatureAssignments");
        }
    }
}
