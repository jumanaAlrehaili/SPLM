using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Update_FeatureAssignment_StageBasedAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeatureAssignments_Roles_AssignedRoleId",
                table: "FeatureAssignments");

            migrationBuilder.DropIndex(
                name: "IX_FeatureAssignments_AssignedRoleId",
                table: "FeatureAssignments");

            migrationBuilder.DropIndex(
                name: "IX_FeatureAssignments_FeatureId_AssignedRoleId_AssignedUserId",
                table: "FeatureAssignments");

            migrationBuilder.DropColumn(
                name: "AssignedRoleId",
                table: "FeatureAssignments");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "Features",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "AssignedUserId",
                table: "FeatureAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StageId",
                table: "FeatureAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FeatureAssignments_FeatureId_StageId",
                table: "FeatureAssignments",
                columns: new[] { "FeatureId", "StageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeatureAssignments_StageId",
                table: "FeatureAssignments",
                column: "StageId");

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureAssignments_Stages_StageId",
                table: "FeatureAssignments",
                column: "StageId",
                principalTable: "Stages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeatureAssignments_Stages_StageId",
                table: "FeatureAssignments");

            migrationBuilder.DropIndex(
                name: "IX_FeatureAssignments_FeatureId_StageId",
                table: "FeatureAssignments");

            migrationBuilder.DropIndex(
                name: "IX_FeatureAssignments_StageId",
                table: "FeatureAssignments");

            migrationBuilder.DropColumn(
                name: "StageId",
                table: "FeatureAssignments");

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "Features",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AssignedUserId",
                table: "FeatureAssignments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AssignedRoleId",
                table: "FeatureAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeatureAssignments_AssignedRoleId",
                table: "FeatureAssignments",
                column: "AssignedRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureAssignments_FeatureId_AssignedRoleId_AssignedUserId",
                table: "FeatureAssignments",
                columns: new[] { "FeatureId", "AssignedRoleId", "AssignedUserId" },
                unique: true,
                filter: "[AssignedRoleId] IS NOT NULL AND [AssignedUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureAssignments_Roles_AssignedRoleId",
                table: "FeatureAssignments",
                column: "AssignedRoleId",
                principalSchema: "identity",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
