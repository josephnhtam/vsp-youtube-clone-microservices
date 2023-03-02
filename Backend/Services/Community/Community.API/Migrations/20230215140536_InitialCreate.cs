using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Community.API.Migrations {
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
                name: "VideoForums",
                columns: table => new {
                    VideoId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    AllowedToComment = table.Column<bool>(type: "boolean", nullable: false),
                    VideoCommentsCount = table.Column<int>(type: "integer", nullable: false),
                    RootVideoCommentsCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_VideoForums", x => x.VideoId);
                    table.ForeignKey(
                        name: "FK_VideoForums_UserProfiles_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoComments",
                columns: table => new {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VideoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentCommentId = table.Column<long>(type: "bigint", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    LikesCount = table.Column<int>(type: "integer", nullable: false),
                    DislikesCount = table.Column<int>(type: "integer", nullable: false),
                    RepliesCount = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_VideoComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideoComments_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoComments_VideoComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "VideoComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoComments_VideoForums_VideoId",
                        column: x => x.VideoId,
                        principalTable: "VideoForums",
                        principalColumn: "VideoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VideoCommentVotes",
                columns: table => new {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    VideoCommentId = table.Column<long>(type: "bigint", nullable: false),
                    VideoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_VideoCommentVotes", x => new { x.UserId, x.VideoCommentId });
                    table.ForeignKey(
                        name: "FK_VideoCommentVotes_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoCommentVotes_VideoComments_VideoCommentId",
                        column: x => x.VideoCommentId,
                        principalTable: "VideoComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoCommentVotes_VideoForums_VideoId",
                        column: x => x.VideoId,
                        principalTable: "VideoForums",
                        principalColumn: "VideoId",
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
                name: "IX_VideoComments_CreateDate",
                table: "VideoComments",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_VideoComments_LikesCount",
                table: "VideoComments",
                column: "LikesCount");

            migrationBuilder.CreateIndex(
                name: "IX_VideoComments_ParentCommentId",
                table: "VideoComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoComments_RepliesCount",
                table: "VideoComments",
                column: "RepliesCount");

            migrationBuilder.CreateIndex(
                name: "IX_VideoComments_UserId",
                table: "VideoComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoComments_VideoId_ParentCommentId",
                table: "VideoComments",
                columns: new[] { "VideoId", "ParentCommentId" });

            migrationBuilder.CreateIndex(
                name: "IX_VideoCommentVotes_UserId",
                table: "VideoCommentVotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoCommentVotes_UserId_VideoId",
                table: "VideoCommentVotes",
                columns: new[] { "UserId", "VideoId" });

            migrationBuilder.CreateIndex(
                name: "IX_VideoCommentVotes_VideoCommentId",
                table: "VideoCommentVotes",
                column: "VideoCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoCommentVotes_VideoId",
                table: "VideoCommentVotes",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoForums_CreatorId",
                table: "VideoForums",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_VideoForums_VideoCommentsCount",
                table: "VideoForums",
                column: "VideoCommentsCount");
        }

        /// <inheritdoc />
        protected override void Down (MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "_IdempotentOperation");

            migrationBuilder.DropTable(
                name: "_TransactionalEvents");

            migrationBuilder.DropTable(
                name: "VideoCommentVotes");

            migrationBuilder.DropTable(
                name: "_TransactionalEventsGroup");

            migrationBuilder.DropTable(
                name: "VideoComments");

            migrationBuilder.DropTable(
                name: "VideoForums");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
