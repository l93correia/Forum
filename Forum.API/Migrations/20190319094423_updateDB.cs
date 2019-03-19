using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.API.Migrations
{
    public partial class updateDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionsParticipants_Discussions_DiscussionId",
                table: "DiscussionsParticipants");

            migrationBuilder.AlterColumn<int>(
                name: "DiscussionId",
                table: "DiscussionsParticipants",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionsParticipants_Discussions_DiscussionId",
                table: "DiscussionsParticipants",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscussionsParticipants_Discussions_DiscussionId",
                table: "DiscussionsParticipants");

            migrationBuilder.AlterColumn<int>(
                name: "DiscussionId",
                table: "DiscussionsParticipants",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_DiscussionsParticipants_Discussions_DiscussionId",
                table: "DiscussionsParticipants",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
