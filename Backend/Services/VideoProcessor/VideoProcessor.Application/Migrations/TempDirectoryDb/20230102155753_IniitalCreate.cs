using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoProcessor.Application.Migrations.TempDirectoryDb {
    public partial class IniitalCreate : Migration {
        protected override void Up (MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "TempDirectories",
                columns: table => new {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_TempDirectories", x => x.Id);
                });
        }

        protected override void Down (MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "TempDirectories");
        }
    }
}
