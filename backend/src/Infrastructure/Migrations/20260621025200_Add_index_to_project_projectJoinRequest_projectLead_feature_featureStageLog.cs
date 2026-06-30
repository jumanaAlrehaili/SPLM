using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_index_to_project_projectJoinRequest_projectLead_feature_featureStageLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProjectJoinRequests_ProjectId",
                schema: "project",
                table: "ProjectJoinRequests");

            migrationBuilder.DropIndex(
                name: "IX_ProjectJoinRequests_UserId",
                schema: "project",
                table: "ProjectJoinRequests");

            migrationBuilder.DropIndex(
                name: "IX_FeatureStageLogs_FeatureId",
                schema: "feature",
                table: "FeatureStageLogs");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedAt_Id",
                schema: "project",
                table: "Projects",
                columns: new[] { "CreatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectLeads_ProjectId_UserId",
                schema: "project",
                table: "ProjectLeads",
                columns: new[] { "ProjectId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectJoinRequests_ProjectId_UserId_StatusId",
                schema: "project",
                table: "ProjectJoinRequests",
                columns: new[] { "ProjectId", "UserId", "StatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectJoinRequests_UserId_CreatedAt",
                schema: "project",
                table: "ProjectJoinRequests",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FeatureStageLogs_FeatureId_Timestamp",
                schema: "feature",
                table: "FeatureStageLogs",
                columns: new[] { "FeatureId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_Features_CreatedAt",
                schema: "feature",
                table: "Features",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_CreatedAt_Id",
                schema: "project",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_ProjectLeads_ProjectId_UserId",
                schema: "project",
                table: "ProjectLeads");

            migrationBuilder.DropIndex(
                name: "IX_ProjectJoinRequests_ProjectId_UserId_StatusId",
                schema: "project",
                table: "ProjectJoinRequests");

            migrationBuilder.DropIndex(
                name: "IX_ProjectJoinRequests_UserId_CreatedAt",
                schema: "project",
                table: "ProjectJoinRequests");

            migrationBuilder.DropIndex(
                name: "IX_FeatureStageLogs_FeatureId_Timestamp",
                schema: "feature",
                table: "FeatureStageLogs");

            migrationBuilder.DropIndex(
                name: "IX_Features_CreatedAt",
                schema: "feature",
                table: "Features");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectJoinRequests_ProjectId",
                schema: "project",
                table: "ProjectJoinRequests",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectJoinRequests_UserId",
                schema: "project",
                table: "ProjectJoinRequests",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FeatureStageLogs_FeatureId",
                schema: "feature",
                table: "FeatureStageLogs",
                column: "FeatureId");
        }
    }
}
