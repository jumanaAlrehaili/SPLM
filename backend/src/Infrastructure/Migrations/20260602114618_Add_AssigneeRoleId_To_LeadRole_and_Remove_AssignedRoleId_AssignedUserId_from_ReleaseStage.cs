using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_AssigneeRoleId_To_LeadRole_and_Remove_AssignedRoleId_AssignedUserId_from_ReleaseStage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssigneeRoleId",
                table: "LeadRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "LeadRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "AssigneeRoleId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "LeadRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "AssigneeRoleId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "LeadRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "AssigneeRoleId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "LeadRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "AssigneeRoleId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "LeadRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "AssigneeRoleId",
                value: 6);

            migrationBuilder.InsertData(
                table: "LeadRoles",
                columns: new[] { "Id", "AssigneeRoleId", "Name", "StageId" },
                values: new object[] { 6, 1, "UAT Lead", 6 });

            migrationBuilder.CreateIndex(
                name: "IX_LeadRoles_AssigneeRoleId",
                table: "LeadRoles",
                column: "AssigneeRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_LeadRoles_Roles_AssigneeRoleId",
                table: "LeadRoles",
                column: "AssigneeRoleId",
                principalSchema: "identity",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeadRoles_Roles_AssigneeRoleId",
                table: "LeadRoles");

            migrationBuilder.DropIndex(
                name: "IX_LeadRoles_AssigneeRoleId",
                table: "LeadRoles");

            migrationBuilder.DeleteData(
                table: "LeadRoles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DropColumn(
                name: "AssigneeRoleId",
                table: "LeadRoles");
        }
    }
}
