using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Subscriptions.API.Migrations {
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
                    Description = table.Column<string>(type: "text", nullable: false),
                    Handle = table.Column<string>(type: "text", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "text", nullable: true),
                    SubscribersCount = table.Column<long>(type: "bigint", nullable: false),
                    SubscriptionsCount = table.Column<int>(type: "integer", nullable: false),
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
                name: "Subscriptions",
                columns: table => new {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    TargetId = table.Column<string>(type: "text", nullable: false),
                    NotificationType = table.Column<int>(type: "integer", nullable: false),
                    SubscriptionDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_Subscriptions", x => new { x.UserId, x.TargetId });
                    table.ForeignKey(
                        name: "FK_Subscriptions_UserProfiles_TargetId",
                        column: x => x.TargetId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscriptions_UserProfiles_UserId",
                        column: x => x.UserId,
                        principalTable: "UserProfiles",
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
                name: "IX_Subscriptions_TargetId",
                table: "Subscriptions",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_Handle",
                table: "UserProfiles",
                column: "Handle");
        }

        /// <inheritdoc />
        protected override void Down (MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "_IdempotentOperation");

            migrationBuilder.DropTable(
                name: "_TransactionalEvents");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "_TransactionalEventsGroup");

            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
