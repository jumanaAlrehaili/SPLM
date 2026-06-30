using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserAssignmentFromReleaseStage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseStages_Roles_AssignedRoleId",
                table: "ReleaseStages");

            migrationBuilder.DropForeignKey(
                name: "FK_ReleaseStages_Users_AssignedUserId",
                table: "ReleaseStages");

            migrationBuilder.DropIndex(
                name: "IX_ReleaseStages_AssignedRoleId",
                table: "ReleaseStages");

            migrationBuilder.DropIndex(
                name: "IX_ReleaseStages_AssignedUserId",
                table: "ReleaseStages");

            migrationBuilder.DropColumn(
                name: "AssignedRoleId",
                table: "ReleaseStages");

            migrationBuilder.DropColumn(
                name: "AssignedUserId",
                table: "ReleaseStages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedRoleId",
                table: "ReleaseStages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignedUserId",
                table: "ReleaseStages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStages_AssignedRoleId",
                table: "ReleaseStages",
                column: "AssignedRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStages_AssignedUserId",
                table: "ReleaseStages",
                column: "AssignedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseStages_Roles_AssignedRoleId",
                table: "ReleaseStages",
                column: "AssignedRoleId",
                principalSchema: "identity",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReleaseStages_Users_AssignedUserId",
                table: "ReleaseStages",
                column: "AssignedUserId",
                principalSchema: "identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
