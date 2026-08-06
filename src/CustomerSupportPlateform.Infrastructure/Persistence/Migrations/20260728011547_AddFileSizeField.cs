using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerSupportPlateform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFileSizeField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Chunks",
                newName: "Chunk");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "KnowledgeDocuments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "KnowledgeDocuments");

            migrationBuilder.RenameColumn(
                name: "Chunk",
                table: "Chunks",
                newName: "Content");
        }
    }
}
