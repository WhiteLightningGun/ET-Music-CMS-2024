using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UpLoader_For_ET.Migrations
{
    /// <inheritdoc />
    public partial class MyBaseMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UploadDBEntries",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    userHash = table.Column<string>(type: "TEXT", nullable: false),
                    userFileTitle = table.Column<string>(type: "TEXT", maxLength: 28, nullable: false),
                    userDescription = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    fileHash = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadDBEntries", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UploadDBEntries");
        }
    }
}
