using Microsoft.EntityFrameworkCore.Migrations;

namespace Discussions.API.Migrations
{
    public partial class updateAttachment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Discussions_DiscussionId",
                table: "Attachments");

            migrationBuilder.AlterColumn<long>(
                name: "DiscussionId",
                table: "Attachments",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ExternalId",
                table: "Attachments",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Attachments",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Discussions_DiscussionId",
                table: "Attachments",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Discussions_DiscussionId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Attachments");

            migrationBuilder.AlterColumn<long>(
                name: "DiscussionId",
                table: "Attachments",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Discussions_DiscussionId",
                table: "Attachments",
                column: "DiscussionId",
                principalTable: "Discussions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
