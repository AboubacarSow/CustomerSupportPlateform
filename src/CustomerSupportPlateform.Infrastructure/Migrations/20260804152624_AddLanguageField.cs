using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerSupportPlateform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "KnowledgeDocuments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Chunks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Chunks_Language",
                table: "Chunks",
                column: "Language");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Chunks_Language",
                table: "Chunks");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "KnowledgeDocuments");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Chunks");
        }
    }
}
