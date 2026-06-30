using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReleaseStageStatusWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseStageHistories_Statuses_NewStatusId",
                table: "ReleaseStageHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseStageHistories_Statuses_OldStatusId",
                table: "ReleaseStageHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseStages_Statuses_StatusId",
                table: "ReleaseStages");

            migrationBuilder.CreateTable(
                name: "ReleaseStageStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseStageStatuses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "ReleaseStageStatuses",
                columns: new[] { "Id", "StatusName" },
                values: new object[,]
                {
                    { 1, "Not Started" },
                    { 2, "In Progress" },
                    { 3, "Completed" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseStageHistories_ReleaseStageStatuses_NewStatusId",
                table: "ReleaseStageHistories",
                column: "NewStatusId",
                principalTable: "ReleaseStageStatuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseStageHistories_ReleaseStageStatuses_OldStatusId",
                table: "ReleaseStageHistories",
                column: "OldStatusId",
                principalTable: "ReleaseStageStatuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseStages_ReleaseStageStatuses_StatusId",
                table: "ReleaseStages",
                column: "StatusId",
                principalTable: "ReleaseStageStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseStageHistories_ReleaseStageStatuses_NewStatusId",
                table: "ReleaseStageHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseStageHistories_ReleaseStageStatuses_OldStatusId",
                table: "ReleaseStageHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseStages_ReleaseStageStatuses_StatusId",
                table: "ReleaseStages");

            migrationBuilder.DropTable(
                name: "ReleaseStageStatuses");

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseStageHistories_Statuses_NewStatusId",
                table: "ReleaseStageHistories",
                column: "NewStatusId",
                principalTable: "Statuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseStageHistories_Statuses_OldStatusId",
                table: "ReleaseStageHistories",
                column: "OldStatusId",
                principalTable: "Statuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseStages_Statuses_StatusId",
                table: "ReleaseStages",
                column: "StatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
