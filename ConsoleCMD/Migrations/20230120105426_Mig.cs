using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleCMD.Migrations
{
    /// <inheritdoc />
    public partial class Mig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Icon",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<byte[]>(type: "BLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Icon", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Directory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    IconId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Directory_Directory",
                        column: x => x.ParentId,
                        principalTable: "Directory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Directory_Icon",
                        column: x => x.IconId,
                        principalTable: "Icon",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Extension",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IconId = table.Column<int>(type: "INTEGER", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Extension", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Extension_Icon",
                        column: x => x.IconId,
                        principalTable: "Icon",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ExtensionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                    table.ForeignKey(
                        name: "FK_File_Directory",
                        column: x => x.ParentId,
                        principalTable: "Directory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_File_Extension",
                        column: x => x.ExtensionId,
                        principalTable: "Extension",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Directory_IconId",
                table: "Directory",
                column: "IconId");

            migrationBuilder.CreateIndex(
                name: "IX_Directory_ParentId",
                table: "Directory",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Extension_IconId",
                table: "Extension",
                column: "IconId");

            migrationBuilder.CreateIndex(
                name: "IX_File_ExtensionId",
                table: "File",
                column: "ExtensionId");

            migrationBuilder.CreateIndex(
                name: "IX_File_ParentId",
                table: "File",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropTable(
                name: "Directory");

            migrationBuilder.DropTable(
                name: "Extension");

            migrationBuilder.DropTable(
                name: "Icon");
        }
    }
}
