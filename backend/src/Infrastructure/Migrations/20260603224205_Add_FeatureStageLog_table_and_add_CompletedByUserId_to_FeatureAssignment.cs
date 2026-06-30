using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_FeatureStageLog_table_and_add_CompletedByUserId_to_FeatureAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "FeatureAssignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompletedByUserId",
                table: "FeatureAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FeatureStageLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeatureId = table.Column<int>(type: "int", nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureStageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeatureStageLogs_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeatureStageLogs_Stages_StageId",
                        column: x => x.StageId,
                        principalTable: "Stages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeatureStageLogs_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeatureAssignments_CompletedByUserId",
                table: "FeatureAssignments",
                column: "CompletedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureStageLogs_FeatureId",
                table: "FeatureStageLogs",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureStageLogs_StageId",
                table: "FeatureStageLogs",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureStageLogs_UserId",
                table: "FeatureStageLogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureAssignments_Users_CompletedByUserId",
                table: "FeatureAssignments",
                column: "CompletedByUserId",
                principalSchema: "identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeatureAssignments_Users_CompletedByUserId",
                table: "FeatureAssignments");

            migrationBuilder.DropTable(
                name: "FeatureStageLogs");

            migrationBuilder.DropIndex(
                name: "IX_FeatureAssignments_CompletedByUserId",
                table: "FeatureAssignments");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "FeatureAssignments");

            migrationBuilder.DropColumn(
                name: "CompletedByUserId",
                table: "FeatureAssignments");
        }
    }
}
