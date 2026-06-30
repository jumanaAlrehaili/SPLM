using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDatabaseSchemas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "lookup");

            migrationBuilder.EnsureSchema(
                name: "feature");

            migrationBuilder.EnsureSchema(
                name: "admin");

            migrationBuilder.EnsureSchema(
                name: "notification");

            migrationBuilder.EnsureSchema(
                name: "project");

            migrationBuilder.EnsureSchema(
                name: "release");

            migrationBuilder.RenameTable(
                name: "Statuses",
                newName: "Statuses",
                newSchema: "lookup");

            migrationBuilder.RenameTable(
                name: "Stages",
                newName: "Stages",
                newSchema: "lookup");

            migrationBuilder.RenameTable(
                name: "RequestStatuses",
                newName: "RequestStatuses",
                newSchema: "lookup");

            migrationBuilder.RenameTable(
                name: "ReleaseStageStatuses",
                newName: "ReleaseStageStatuses",
                newSchema: "lookup");

            migrationBuilder.RenameTable(
                name: "ReleaseStages",
                newName: "ReleaseStages",
                newSchema: "release");

            migrationBuilder.RenameTable(
                name: "ReleaseStagePrerequisites",
                newName: "ReleaseStagePrerequisites",
                newSchema: "release");

            migrationBuilder.RenameTable(
                name: "ReleaseStageHistories",
                newName: "ReleaseStageHistories",
                newSchema: "release");

            migrationBuilder.RenameTable(
                name: "ReleaseStageChangeTypes",
                newName: "ReleaseStageChangeTypes",
                newSchema: "lookup");

            migrationBuilder.RenameTable(
                name: "Releases",
                newName: "Releases",
                newSchema: "release");

            migrationBuilder.RenameTable(
                name: "ReleasePlans",
                newName: "ReleasePlans",
                newSchema: "release");

            migrationBuilder.RenameTable(
                name: "Projects",
                newName: "Projects",
                newSchema: "project");

            migrationBuilder.RenameTable(
                name: "ProjectResources",
                newName: "ProjectResources",
                newSchema: "project");

            migrationBuilder.RenameTable(
                name: "ProjectMembershipStatuses",
                newName: "ProjectMembershipStatuses",
                newSchema: "lookup");

            migrationBuilder.RenameTable(
                name: "ProjectLeads",
                newName: "ProjectLeads",
                newSchema: "project");

            migrationBuilder.RenameTable(
                name: "ProjectJoinRequests",
                newName: "ProjectJoinRequests",
                newSchema: "project");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notifications",
                newSchema: "notification");

            migrationBuilder.RenameTable(
                name: "LeadRoles",
                newName: "LeadRoles",
                newSchema: "lookup");

            migrationBuilder.RenameTable(
                name: "Holidays",
                newName: "Holidays",
                newSchema: "admin");

            migrationBuilder.RenameTable(
                name: "FeatureStageLogs",
                newName: "FeatureStageLogs",
                newSchema: "feature");

            migrationBuilder.RenameTable(
                name: "Features",
                newName: "Features",
                newSchema: "feature");

            migrationBuilder.RenameTable(
                name: "FeaturePriorities",
                newName: "FeaturePriorities",
                newSchema: "lookup");

            migrationBuilder.RenameTable(
                name: "FeatureAssignments",
                newName: "FeatureAssignments",
                newSchema: "feature");

            migrationBuilder.RenameTable(
                name: "DurationUnits",
                newName: "DurationUnits",
                newSchema: "lookup");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Statuses",
                schema: "lookup",
                newName: "Statuses");

            migrationBuilder.RenameTable(
                name: "Stages",
                schema: "lookup",
                newName: "Stages");

            migrationBuilder.RenameTable(
                name: "RequestStatuses",
                schema: "lookup",
                newName: "RequestStatuses");

            migrationBuilder.RenameTable(
                name: "ReleaseStageStatuses",
                schema: "lookup",
                newName: "ReleaseStageStatuses");

            migrationBuilder.RenameTable(
                name: "ReleaseStages",
                schema: "release",
                newName: "ReleaseStages");

            migrationBuilder.RenameTable(
                name: "ReleaseStagePrerequisites",
                schema: "release",
                newName: "ReleaseStagePrerequisites");

            migrationBuilder.RenameTable(
                name: "ReleaseStageHistories",
                schema: "release",
                newName: "ReleaseStageHistories");

            migrationBuilder.RenameTable(
                name: "ReleaseStageChangeTypes",
                schema: "lookup",
                newName: "ReleaseStageChangeTypes");

            migrationBuilder.RenameTable(
                name: "Releases",
                schema: "release",
                newName: "Releases");

            migrationBuilder.RenameTable(
                name: "ReleasePlans",
                schema: "release",
                newName: "ReleasePlans");

            migrationBuilder.RenameTable(
                name: "Projects",
                schema: "project",
                newName: "Projects");

            migrationBuilder.RenameTable(
                name: "ProjectResources",
                schema: "project",
                newName: "ProjectResources");

            migrationBuilder.RenameTable(
                name: "ProjectMembershipStatuses",
                schema: "lookup",
                newName: "ProjectMembershipStatuses");

            migrationBuilder.RenameTable(
                name: "ProjectLeads",
                schema: "project",
                newName: "ProjectLeads");

            migrationBuilder.RenameTable(
                name: "ProjectJoinRequests",
                schema: "project",
                newName: "ProjectJoinRequests");

            migrationBuilder.RenameTable(
                name: "Notifications",
                schema: "notification",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "LeadRoles",
                schema: "lookup",
                newName: "LeadRoles");

            migrationBuilder.RenameTable(
                name: "Holidays",
                schema: "admin",
                newName: "Holidays");

            migrationBuilder.RenameTable(
                name: "FeatureStageLogs",
                schema: "feature",
                newName: "FeatureStageLogs");

            migrationBuilder.RenameTable(
                name: "Features",
                schema: "feature",
                newName: "Features");

            migrationBuilder.RenameTable(
                name: "FeaturePriorities",
                schema: "lookup",
                newName: "FeaturePriorities");

            migrationBuilder.RenameTable(
                name: "FeatureAssignments",
                schema: "feature",
                newName: "FeatureAssignments");

            migrationBuilder.RenameTable(
                name: "DurationUnits",
                schema: "lookup",
                newName: "DurationUnits");
        }
    }
}
