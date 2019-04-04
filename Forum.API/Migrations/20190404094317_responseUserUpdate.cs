using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.API.Migrations
{
    public partial class responseUserUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionResponses_User_CreatedById",
                table: "DiscussionResponses");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "DiscussionResponses",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionResponses_CreatedById",
                table: "DiscussionResponses",
                newName: "IX_DiscussionResponses_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionResponses_User_UserId",
                table: "DiscussionResponses",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionResponses_User_UserId",
                table: "DiscussionResponses");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "DiscussionResponses",
                newName: "CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_DiscussionResponses_UserId",
                table: "DiscussionResponses",
                newName: "IX_DiscussionResponses_CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionResponses_User_CreatedById",
                table: "DiscussionResponses",
                column: "CreatedById",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
