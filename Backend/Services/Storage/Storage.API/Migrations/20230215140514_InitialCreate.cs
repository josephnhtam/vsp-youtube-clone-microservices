using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Storage.API.Migrations {
    /// <inheritdoc />
    public partial class InitialCreate : Migration {
        /// <inheritdoc />
        protected override void Up (MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateSequence(
                name: "file_property_seq",
                incrementBy: 10);

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
                name: "Files",
                columns: table => new {
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    TrackingId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: true),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    OriginalFileName = table.Column<string>(type: "text", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Files", x => x.FileId);
                });

            migrationBuilder.CreateTable(
                name: "FileTrackings",
                columns: table => new {
                    TrackingId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    FileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Category = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "text", nullable: true),
                    FileName = table.Column<string>(type: "text", nullable: false),
                    OriginalFileName = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    RemovalRetryCount = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RemovalDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_FileTrackings", x => x.TrackingId);
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
                name: "FileProperty",
                columns: table => new {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    StoredFileFileId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table => {
                    table.PrimaryKey("PK_FileProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileProperty_Files_StoredFileFileId",
                        column: x => x.StoredFileFileId,
                        principalTable: "Files",
                        principalColumn: "FileId",
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
                name: "IX_FileProperty_StoredFileFileId",
                table: "FileProperty",
                column: "StoredFileFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_Category",
                table: "Files",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Files_ContentType",
                table: "Files",
                column: "ContentType");

            migrationBuilder.CreateIndex(
                name: "IX_Files_CreateDate",
                table: "Files",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_Files_FileSize",
                table: "Files",
                column: "FileSize");

            migrationBuilder.CreateIndex(
                name: "IX_Files_GroupId",
                table: "Files",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_TrackingId",
                table: "Files",
                column: "TrackingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_UserId",
                table: "Files",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FileTrackings_CreateDate",
                table: "FileTrackings",
                column: "CreateDate");

            migrationBuilder.CreateIndex(
                name: "IX_FileTrackings_FileId",
                table: "FileTrackings",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_FileTrackings_GroupId",
                table: "FileTrackings",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_FileTrackings_RemovalDate",
                table: "FileTrackings",
                column: "RemovalDate");

            migrationBuilder.CreateIndex(
                name: "IX_FileTrackings_Status",
                table: "FileTrackings",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down (MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "_IdempotentOperation");

            migrationBuilder.DropTable(
                name: "_TransactionalEvents");

            migrationBuilder.DropTable(
                name: "FileProperty");

            migrationBuilder.DropTable(
                name: "FileTrackings");

            migrationBuilder.DropTable(
                name: "_TransactionalEventsGroup");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropSequence(
                name: "file_property_seq");
        }
    }
}
