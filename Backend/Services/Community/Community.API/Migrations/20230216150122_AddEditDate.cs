using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Community.API.Migrations {
    /// <inheritdoc />
    public partial class AddEditDate : Migration {
        /// <inheritdoc />
        protected override void Up (MigrationBuilder migrationBuilder) {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EditDate",
                table: "VideoComments",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down (MigrationBuilder migrationBuilder) {
            migrationBuilder.DropColumn(
                name: "EditDate",
                table: "VideoComments");
        }
    }
}
