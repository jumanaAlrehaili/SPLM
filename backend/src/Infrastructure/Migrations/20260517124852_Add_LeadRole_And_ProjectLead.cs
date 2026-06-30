using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_LeadRole_And_ProjectLead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LeadRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadRoles_Stages_StageId",
                        column: x => x.StageId,
                        principalTable: "Stages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectLeads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    LeadRoleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectLeads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectLeads_LeadRoles_LeadRoleId",
                        column: x => x.LeadRoleId,
                        principalTable: "LeadRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectLeads_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectLeads_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectLeads_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectLeads_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "LeadRoles",
                columns: new[] { "Id", "Name", "StageId" },
                values: new object[,]
                {
                    { 1, "BA Lead", 1 },
                    { 2, "SA Lead", 2 },
                    { 3, "UI/UX Lead", 3 },
                    { 4, "Dev Lead", 4 },
                    { 5, "QA Lead", 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeadRoles_StageId",
                table: "LeadRoles",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeads_CreatedByUserId",
                table: "ProjectLeads",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeads_LeadRoleId",
                table: "ProjectLeads",
                column: "LeadRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeads_ProjectId_LeadRoleId",
                table: "ProjectLeads",
                columns: new[] { "ProjectId", "LeadRoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeads_UpdatedByUserId",
                table: "ProjectLeads",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeads_UserId",
                table: "ProjectLeads",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectLeads");

            migrationBuilder.DropTable(
                name: "LeadRoles");
        }
    }
}
