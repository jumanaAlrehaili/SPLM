using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_RequestStatus_and_ProjectMembershipStatus_and_DurationUnit_and_ReleaseStageChangeType_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DurationUnit",
                table: "Projects",
                newName: "DurationUnitId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "ProjectJoinRequests",
                newName: "StatusId");

            migrationBuilder.CreateTable(
                name: "DurationUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DurationUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectMembershipStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectMembershipStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReleaseStageChangeTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseStageChangeTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatuses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DurationUnits",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Days" },
                    { 2, "Weeks" },
                    { 3, "Months" }
                });

            migrationBuilder.InsertData(
                table: "ProjectMembershipStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "None" },
                    { 2, "Pending" },
                    { 3, "Member" },
                    { 4, "Owner" }
                });

            migrationBuilder.InsertData(
                table: "ReleaseStageChangeTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Created" },
                    { 2, "Updated" },
                    { 3, "Started" },
                    { 4, "Completed" },
                    { 5, "Delayed" },
                    { 6, "Reopened" },
                    { 7, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "RequestStatuses",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Approved" },
                    { 3, "Rejected" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_DurationUnitId",
                table: "Projects",
                column: "DurationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectJoinRequests_StatusId",
                table: "ProjectJoinRequests",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectJoinRequests_RequestStatuses_StatusId",
                table: "ProjectJoinRequests",
                column: "StatusId",
                principalTable: "RequestStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_DurationUnits_DurationUnitId",
                table: "Projects",
                column: "DurationUnitId",
                principalTable: "DurationUnits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectJoinRequests_RequestStatuses_StatusId",
                table: "ProjectJoinRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_DurationUnits_DurationUnitId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "DurationUnits");

            migrationBuilder.DropTable(
                name: "ProjectMembershipStatuses");

            migrationBuilder.DropTable(
                name: "ReleaseStageChangeTypes");

            migrationBuilder.DropTable(
                name: "RequestStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Projects_DurationUnitId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_ProjectJoinRequests_StatusId",
                table: "ProjectJoinRequests");

            migrationBuilder.RenameColumn(
                name: "DurationUnitId",
                table: "Projects",
                newName: "DurationUnit");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "ProjectJoinRequests",
                newName: "Status");
        }
    }
}
