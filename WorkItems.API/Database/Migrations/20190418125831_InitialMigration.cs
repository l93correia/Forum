using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Emsa.Mared.ContentManagement.WorkItems.Database.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WorkItemType = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    Summary = table.Column<string>(maxLength: 500, nullable: false),
                    Body = table.Column<string>(maxLength: 2500, nullable: false),
                    Location = table.Column<string>(maxLength: 500, nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    ClosedAt = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    StartsAt = table.Column<DateTime>(nullable: true),
                    EndsAt = table.Column<DateTime>(nullable: true),
                    IsPublic = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkItemAttachments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    WorkItemId = table.Column<long>(nullable: false),
                    ExternalId = table.Column<long>(maxLength: 500, nullable: false),
                    Url = table.Column<string>(maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItemAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItemAttachments_WorkItems_WorkItemId",
                        column: x => x.WorkItemId,
                        principalTable: "WorkItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkItemComments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    WorkItemId = table.Column<long>(nullable: false),
                    Comment = table.Column<string>(maxLength: 500, nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItemComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItemComments_WorkItems_WorkItemId",
                        column: x => x.WorkItemId,
                        principalTable: "WorkItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkItemParticipants",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    WorkItemId = table.Column<long>(nullable: false),
                    EntityId = table.Column<long>(nullable: false),
                    EntityType = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItemParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItemParticipants_WorkItems_WorkItemId",
                        column: x => x.WorkItemId,
                        principalTable: "WorkItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkItemRelations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    RelatedFromWorkItemId = table.Column<long>(nullable: false),
                    RelatedToWorkItemId = table.Column<long>(nullable: false),
                    RelationType = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItemRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItemRelations_WorkItems_RelatedFromWorkItemId",
                        column: x => x.RelatedFromWorkItemId,
                        principalTable: "WorkItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkItemRelations_WorkItems_RelatedToWorkItemId",
                        column: x => x.RelatedToWorkItemId,
                        principalTable: "WorkItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemAttachments_WorkItemId",
                table: "WorkItemAttachments",
                column: "WorkItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemComments_WorkItemId",
                table: "WorkItemComments",
                column: "WorkItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemParticipants_WorkItemId",
                table: "WorkItemParticipants",
                column: "WorkItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemRelations_RelatedFromWorkItemId",
                table: "WorkItemRelations",
                column: "RelatedFromWorkItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemRelations_RelatedToWorkItemId",
                table: "WorkItemRelations",
                column: "RelatedToWorkItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemRelations_RelatedFromWorkItemId_RelatedToWorkItemId_~",
                table: "WorkItemRelations",
                columns: new[] { "RelatedFromWorkItemId", "RelatedToWorkItemId", "RelationType" });

            migrationBuilder.CreateIndex(
                name: "IX_WorkItems_IsPublic",
                table: "WorkItems",
                column: "IsPublic");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItems_Status",
                table: "WorkItems",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItems_WorkItemType",
                table: "WorkItems",
                column: "WorkItemType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkItemAttachments");

            migrationBuilder.DropTable(
                name: "WorkItemComments");

            migrationBuilder.DropTable(
                name: "WorkItemParticipants");

            migrationBuilder.DropTable(
                name: "WorkItemRelations");

            migrationBuilder.DropTable(
                name: "WorkItems");
        }
    }
}
