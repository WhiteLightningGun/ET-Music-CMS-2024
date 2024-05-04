using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UpLoader_For_ET.Data.Migrations
{
    /// <inheritdoc />
    public partial class zeeBigOne : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FrontPageEntries",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    title = table.Column<string>(type: "TEXT", nullable: false),
                    description = table.Column<string>(type: "TEXT", nullable: false),
                    htmlEmbedLink = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrontPageEntries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "MessageDBEntries",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TimeArrived = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageDBEntries", x => x.id);
                });

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
                name: "FrontPageEntries");

            migrationBuilder.DropTable(
                name: "MessageDBEntries");

            migrationBuilder.DropTable(
                name: "UploadDBEntries");
        }
    }
}
