using Microsoft.EntityFrameworkCore.Migrations;

namespace Discussions.API.Migrations
{
    public partial class updateV1k : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Discussions_DiscussionId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Discussions_DiscussionId",
                table: "Participants");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Discussions_DiscussionId",
                table: "Attachments",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Discussions_DiscussionId",
                table: "Participants",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Discussions_DiscussionId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Discussions_DiscussionId",
                table: "Participants");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Discussions_DiscussionId",
                table: "Attachments",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Participants_Discussions_DiscussionId",
                table: "Participants",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
