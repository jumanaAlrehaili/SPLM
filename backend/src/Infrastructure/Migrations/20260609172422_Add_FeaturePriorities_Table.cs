using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_FeaturePriorities_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Features",
                newName: "PriorityId");

            migrationBuilder.CreateTable(
                name: "FeaturePriorities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturePriorities", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FeaturePriorities",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Low" },
                    { 2, "Medium" },
                    { 3, "High" },
                    { 4, "Critical" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Features_PriorityId",
                table: "Features",
                column: "PriorityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Features_FeaturePriorities_PriorityId",
                table: "Features",
                column: "PriorityId",
                principalTable: "FeaturePriorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Features_FeaturePriorities_PriorityId",
                table: "Features");

            migrationBuilder.DropTable(
                name: "FeaturePriorities");

            migrationBuilder.DropIndex(
                name: "IX_Features_PriorityId",
                table: "Features");

            migrationBuilder.RenameColumn(
                name: "PriorityId",
                table: "Features",
                newName: "Priority");
        }
    }
}
