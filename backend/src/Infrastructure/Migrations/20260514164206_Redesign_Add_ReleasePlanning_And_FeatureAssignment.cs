using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Redesign_Add_ReleasePlanning_And_FeatureAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Features_Stages_CurrentStageId",
                table: "Features");

            migrationBuilder.DropTable(
                name: "StageHistories");

            migrationBuilder.RenameColumn(
                name: "CurrentStageId",
                table: "Features",
                newName: "ReleaseId");

            migrationBuilder.RenameIndex(
                name: "IX_Features_CurrentStageId",
                table: "Features",
                newName: "IX_Features_ReleaseId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Stages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deadline",
                table: "Features",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateTable(
                name: "FeatureAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeatureId = table.Column<int>(type: "int", nullable: false),
                    AssignedRoleId = table.Column<int>(type: "int", nullable: true),
                    AssignedUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeatureAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeatureAssignments_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeatureAssignments_Roles_AssignedRoleId",
                        column: x => x.AssignedRoleId,
                        principalSchema: "identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeatureAssignments_Users_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeatureAssignments_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeatureAssignments_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReleasePlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleasePlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReleasePlans_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReleasePlans_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReleasePlans_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Releases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReleasePlanId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Releases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Releases_ReleasePlans_ReleasePlanId",
                        column: x => x.ReleasePlanId,
                        principalTable: "ReleasePlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Releases_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Releases_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Releases_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReleaseStages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReleaseId = table.Column<int>(type: "int", nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    StageName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    AssignedRoleId = table.Column<int>(type: "int", nullable: true),
                    AssignedUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseStages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReleaseStages_Releases_ReleaseId",
                        column: x => x.ReleaseId,
                        principalTable: "Releases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReleaseStages_Roles_AssignedRoleId",
                        column: x => x.AssignedRoleId,
                        principalSchema: "identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReleaseStages_Stages_StageId",
                        column: x => x.StageId,
                        principalTable: "Stages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReleaseStages_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReleaseStages_Users_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReleaseStages_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReleaseStages_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReleaseStageHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReleaseStageId = table.Column<int>(type: "int", nullable: false),
                    OldStatusId = table.Column<int>(type: "int", nullable: true),
                    NewStatusId = table.Column<int>(type: "int", nullable: true),
                    OldStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NewStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OldEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NewEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChangeType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseStageHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReleaseStageHistories_ReleaseStages_ReleaseStageId",
                        column: x => x.ReleaseStageId,
                        principalTable: "ReleaseStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReleaseStageHistories_Statuses_NewStatusId",
                        column: x => x.NewStatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReleaseStageHistories_Statuses_OldStatusId",
                        column: x => x.OldStatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReleaseStageHistories_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReleaseStagePrerequisites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReleaseStageId = table.Column<int>(type: "int", nullable: false),
                    PrerequisiteReleaseStageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReleaseStagePrerequisites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReleaseStagePrerequisites_ReleaseStages_PrerequisiteReleaseStageId",
                        column: x => x.PrerequisiteReleaseStageId,
                        principalTable: "ReleaseStages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ReleaseStagePrerequisites_ReleaseStages_ReleaseStageId",
                        column: x => x.ReleaseStageId,
                        principalTable: "ReleaseStages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Stages",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsDefault",
                value: true);

            migrationBuilder.UpdateData(
                table: "Stages",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsDefault",
                value: true);

            migrationBuilder.UpdateData(
                table: "Stages",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsDefault",
                value: true);

            migrationBuilder.UpdateData(
                table: "Stages",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsDefault",
                value: true);

            migrationBuilder.UpdateData(
                table: "Stages",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsDefault",
                value: true);

            migrationBuilder.UpdateData(
                table: "Stages",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsDefault",
                value: true);

            migrationBuilder.UpdateData(
                table: "Stages",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsDefault",
                value: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeatureAssignments_AssignedRoleId",
                table: "FeatureAssignments",
                column: "AssignedRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureAssignments_AssignedUserId",
                table: "FeatureAssignments",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureAssignments_CreatedByUserId",
                table: "FeatureAssignments",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureAssignments_FeatureId_AssignedRoleId_AssignedUserId",
                table: "FeatureAssignments",
                columns: new[] { "FeatureId", "AssignedRoleId", "AssignedUserId" },
                unique: true,
                filter: "[AssignedRoleId] IS NOT NULL AND [AssignedUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureAssignments_UpdatedByUserId",
                table: "FeatureAssignments",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleasePlans_CreatedByUserId",
                table: "ReleasePlans",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleasePlans_ProjectId",
                table: "ReleasePlans",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleasePlans_UpdatedByUserId",
                table: "ReleasePlans",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_CreatedByUserId",
                table: "Releases",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_ReleasePlanId",
                table: "Releases",
                column: "ReleasePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_StatusId",
                table: "Releases",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_UpdatedByUserId",
                table: "Releases",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStageHistories_ChangedByUserId",
                table: "ReleaseStageHistories",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStageHistories_NewStatusId",
                table: "ReleaseStageHistories",
                column: "NewStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStageHistories_OldStatusId",
                table: "ReleaseStageHistories",
                column: "OldStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStageHistories_ReleaseStageId",
                table: "ReleaseStageHistories",
                column: "ReleaseStageId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStagePrerequisites_PrerequisiteReleaseStageId",
                table: "ReleaseStagePrerequisites",
                column: "PrerequisiteReleaseStageId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStagePrerequisites_ReleaseStageId_PrerequisiteReleaseStageId",
                table: "ReleaseStagePrerequisites",
                columns: new[] { "ReleaseStageId", "PrerequisiteReleaseStageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStages_AssignedRoleId",
                table: "ReleaseStages",
                column: "AssignedRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStages_AssignedUserId",
                table: "ReleaseStages",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStages_CreatedByUserId",
                table: "ReleaseStages",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStages_ReleaseId_Sequence",
                table: "ReleaseStages",
                columns: new[] { "ReleaseId", "Sequence" });

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStages_StageId",
                table: "ReleaseStages",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStages_StatusId",
                table: "ReleaseStages",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleaseStages_UpdatedByUserId",
                table: "ReleaseStages",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Features_Releases_ReleaseId",
                table: "Features",
                column: "ReleaseId",
                principalTable: "Releases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Features_Releases_ReleaseId",
                table: "Features");

            migrationBuilder.DropTable(
                name: "FeatureAssignments");

            migrationBuilder.DropTable(
                name: "ReleaseStageHistories");

            migrationBuilder.DropTable(
                name: "ReleaseStagePrerequisites");

            migrationBuilder.DropTable(
                name: "ReleaseStages");

            migrationBuilder.DropTable(
                name: "Releases");

            migrationBuilder.DropTable(
                name: "ReleasePlans");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Stages");

            migrationBuilder.RenameColumn(
                name: "ReleaseId",
                table: "Features",
                newName: "CurrentStageId");

            migrationBuilder.RenameIndex(
                name: "IX_Features_ReleaseId",
                table: "Features",
                newName: "IX_Features_CurrentStageId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Deadline",
                table: "Features",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "StageHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignedToUserId = table.Column<int>(type: "int", nullable: true),
                    FeatureId = table.Column<int>(type: "int", nullable: false),
                    StageId = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimatedDaysForThisStage = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StageHistories_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StageHistories_Stages_StageId",
                        column: x => x.StageId,
                        principalTable: "Stages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StageHistories_Users_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StageHistories_AssignedToUserId",
                table: "StageHistories",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StageHistories_FeatureId",
                table: "StageHistories",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_StageHistories_StageId",
                table: "StageHistories",
                column: "StageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Features_Stages_CurrentStageId",
                table: "Features",
                column: "CurrentStageId",
                principalTable: "Stages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
