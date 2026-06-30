using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Rename_OverdueNotifiedAt_To_DueSoonNotifiedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OverdueNotifiedAt",
                schema: "release",
                table: "ReleaseStages",
                newName: "DueSoonNotifiedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DueSoonNotifiedAt",
                schema: "release",
                table: "ReleaseStages",
                newName: "OverdueNotifiedAt");
        }
    }
}
