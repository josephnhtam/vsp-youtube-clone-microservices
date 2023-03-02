using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VideoStore.API.Migrations {
    /// <inheritdoc />
    public partial class InitialCreate : Migration {
        /// <inheritdoc />
        protected override void Up (MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "_IdempotentOperation",
                columns: table => new {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK__IdempotentOperation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "_TransactionalEventsGroup",
                columns: table => new {
                    Id = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    AvailableDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK__TransactionalEventsGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Handle = table.Column<string>(type: "text", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: true),
                    PrimaryVersion = table.Column<long>(type: "bigint", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "_TransactionalEvents",
                columns: table => new {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupId = table.Column<string>(type: "text", nullable: false),
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK__TransactionalEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK__TransactionalEvents__TransactionalEventsGroup_GroupId",
                        column: x => x.GroupId,
                        principalTable: "_TransactionalEventsGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Tags = table.Column<string>(type: "text", nullable: false),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: true),
                    PreviewThumbnailUrl = table.Column<string>(type: "text", nullable: true),
                    Visibility = table.Column<int>(type: "integer", nullable: false),
                    AllowedToPublish = table.Column<bool>(type: "boolean", nullable: false),
                    PublishedWithPublicVisibility = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ViewsCount = table.Column<long>(type: "bigint", nullable: false),
                    ViewsCountUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StatusUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    InfoVersion = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Videos_UserProfiles_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcessedVideo",
                columns: table => new {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VideoFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false),
                    LengthSeconds = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    VideoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_ProcessedVideo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessedVideo_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX__TransactionalEvents_Category",
                table: "_TransactionalEvents",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX__TransactionalEvents_GroupId",
                table: "_TransactionalEvents",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX__TransactionalEvents_SequenceNumber",
                table: "_TransactionalEvents",
                column: "SequenceNumber");

            migrationBuilder.CreateIndex(
                name: "IX__TransactionalEventsGroup_AvailableDate",
                table: "_TransactionalEventsGroup",
                column: "AvailableDate");

            migrationBuilder.CreateIndex(
                name: "IX__TransactionalEventsGroup_CreateDate",
                table: "_TransactionalEventsGroup",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX__TransactionalEventsGroup_LastSequenceNumber",
                table: "_TransactionalEventsGroup",
                column: "LastSequenceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedVideo_VideoId",
                table: "ProcessedVideo",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_CreateDate",
                table: "Videos",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_CreatorId",
                table: "Videos",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_Visibility",
                table: "Videos",
                column: "Visibility");
        }

        /// <inheritdoc />
        protected override void Down (MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "_IdempotentOperation");

            migrationBuilder.DropTable(
                name: "_TransactionalEvents");

            migrationBuilder.DropTable(
                name: "ProcessedVideo");

            migrationBuilder.DropTable(
                name: "_TransactionalEventsGroup");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
