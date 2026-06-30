using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_index_to_ReleasePlans_and_Releases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Releases_ReleasePlanId",
                schema: "release",
                table: "Releases");

            migrationBuilder.DropIndex(
                name: "IX_ReleasePlans_ProjectId",
                schema: "release",
                table: "ReleasePlans");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_ReleasePlanId_Name",
                schema: "release",
                table: "Releases",
                columns: new[] { "ReleasePlanId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReleasePlans_ProjectId_Name",
                schema: "release",
                table: "ReleasePlans",
                columns: new[] { "ProjectId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Releases_ReleasePlanId_Name",
                schema: "release",
                table: "Releases");

            migrationBuilder.DropIndex(
                name: "IX_ReleasePlans_ProjectId_Name",
                schema: "release",
                table: "ReleasePlans");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_ReleasePlanId",
                schema: "release",
                table: "Releases",
                column: "ReleasePlanId");

            migrationBuilder.CreateIndex(
                name: "IX_ReleasePlans_ProjectId",
                schema: "release",
                table: "ReleasePlans",
                column: "ProjectId");
        }
    }
}
