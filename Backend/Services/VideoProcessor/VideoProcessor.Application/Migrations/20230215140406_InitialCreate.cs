using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VideoProcessor.Application.Migrations {
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
                name: "Videos",
                columns: table => new {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    OriginalFileName = table.Column<string>(type: "text", nullable: false),
                    VideoFileUrl = table.Column<string>(type: "text", nullable: false),
                    VideoInfoWidth = table.Column<int>(name: "VideoInfo_Width", type: "integer", nullable: true),
                    VideoInfoHeight = table.Column<int>(name: "VideoInfo_Height", type: "integer", nullable: true),
                    VideoInfoSize = table.Column<long>(name: "VideoInfo_Size", type: "bigint", nullable: true),
                    VideoInfoLengthSeconds = table.Column<int>(name: "VideoInfo_LengthSeconds", type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AvailableDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProcessedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    PreviewThumbnailImageFileId = table.Column<Guid>(type: "uuid", nullable: true),
                    PreviewThumbnailWidth = table.Column<int>(type: "integer", nullable: true),
                    PreviewThumbnailHeight = table.Column<int>(type: "integer", nullable: true),
                    PreviewThumbnailLengthSeconds = table.Column<float>(type: "real", nullable: true),
                    PreviewThumbnailLengthUrl = table.Column<string>(type: "text", nullable: true),
                    LockVersion = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Videos", x => x.Id);
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
                name: "ProcessedVideos",
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
                    table.PrimaryKey("PK_ProcessedVideos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessedVideos_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoProcessingStep",
                columns: table => new {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Label = table.Column<string>(type: "text", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Complete = table.Column<bool>(type: "boolean", nullable: false),
                    VideoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_VideoProcessingStep", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoProcessingStep_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoThumbnails",
                columns: table => new {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImageFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    VideoId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_VideoThumbnails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoThumbnails_Videos_VideoId",
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
                name: "IX_ProcessedVideos_VideoId",
                table: "ProcessedVideos",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoProcessingStep_VideoId",
                table: "VideoProcessingStep",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_AvailableDate",
                table: "Videos",
                column: "AvailableDate");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_CreatorId",
                table: "Videos",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ProcessedDate",
                table: "Videos",
                column: "ProcessedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_Status",
                table: "Videos",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VideoThumbnails_VideoId",
                table: "VideoThumbnails",
                column: "VideoId");
        }

        /// <inheritdoc />
        protected override void Down (MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "_IdempotentOperation");

            migrationBuilder.DropTable(
                name: "_TransactionalEvents");

            migrationBuilder.DropTable(
                name: "ProcessedVideos");

            migrationBuilder.DropTable(
                name: "VideoProcessingStep");

            migrationBuilder.DropTable(
                name: "VideoThumbnails");

            migrationBuilder.DropTable(
                name: "_TransactionalEventsGroup");

            migrationBuilder.DropTable(
                name: "Videos");
        }
    }
}
