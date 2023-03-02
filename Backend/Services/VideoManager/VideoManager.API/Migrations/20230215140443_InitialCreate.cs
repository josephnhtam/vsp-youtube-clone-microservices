using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace VideoManager.API.Migrations {
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
                    ThumbnailId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Visibility = table.Column<int>(type: "integer", nullable: false),
                    AllowedToPublish = table.Column<bool>(type: "boolean", nullable: false),
                    ProcessingStatus = table.Column<int>(type: "integer", nullable: false),
                    OriginalVideoFileId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalVideoFileName = table.Column<string>(type: "text", nullable: true),
                    OriginalVideoUrl = table.Column<string>(type: "text", nullable: true),
                    ThumbnailStatus = table.Column<int>(type: "integer", nullable: false),
                    PreviewThumbnailImageFileId = table.Column<Guid>(type: "uuid", nullable: true),
                    PreviewThumbnailWidth = table.Column<int>(type: "integer", nullable: true),
                    PreviewThumbnailHeight = table.Column<int>(type: "integer", nullable: true),
                    PreviewThumbnailLengthSeconds = table.Column<float>(type: "real", nullable: true),
                    PreviewThumbnailLengthUrl = table.Column<string>(type: "text", nullable: true),
                    ViewsCount = table.Column<long>(type: "bigint", nullable: false),
                    CommentsCount = table.Column<long>(type: "bigint", nullable: false),
                    LikesCount = table.Column<long>(type: "bigint", nullable: false),
                    DislikesCount = table.Column<long>(type: "bigint", nullable: false),
                    ViewsCountUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CommentsCountUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    VotesCountUpdateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PublishDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UnpublishDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    InfoVersion = table.Column<long>(type: "bigint", nullable: false),
                    PublishStatusVersion = table.Column<long>(type: "bigint", nullable: false)
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
                name: "IX_Videos_AllowedToPublish",
                table: "Videos",
                column: "AllowedToPublish");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_CommentsCount",
                table: "Videos",
                column: "CommentsCount");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_CreateDate",
                table: "Videos",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_CreatorId",
                table: "Videos",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_DislikesCount",
                table: "Videos",
                column: "DislikesCount");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_LikesCount",
                table: "Videos",
                column: "LikesCount");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_OriginalVideoFileId",
                table: "Videos",
                column: "OriginalVideoFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ProcessingStatus",
                table: "Videos",
                column: "ProcessingStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_PublishDate",
                table: "Videos",
                column: "PublishDate");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_Status",
                table: "Videos",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ThumbnailStatus",
                table: "Videos",
                column: "ThumbnailStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_ViewsCount",
                table: "Videos",
                column: "ViewsCount");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_Visibility",
                table: "Videos",
                column: "Visibility");

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
                name: "VideoThumbnails");

            migrationBuilder.DropTable(
                name: "_TransactionalEventsGroup");

            migrationBuilder.DropTable(
                name: "Videos");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
