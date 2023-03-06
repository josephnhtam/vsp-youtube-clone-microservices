using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Community.API.Migrations {
    /// <inheritdoc />
    public partial class ModelUpdate : Migration {
        /// <inheritdoc />
        protected override void Up (MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateIndex(
                name: "IX_VideoComments_VideoId",
                table: "VideoComments",
                column: "VideoId");
        }

        /// <inheritdoc />
        protected override void Down (MigrationBuilder migrationBuilder) {
            migrationBuilder.DropIndex(
                name: "IX_VideoComments_VideoId",
                table: "VideoComments");
        }
    }
}
