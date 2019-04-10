using Microsoft.EntityFrameworkCore.Migrations;

namespace Discussions.API.Migrations
{
    public partial class update20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Responses_ResponseId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_ResponseId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "AttachmentId",
                table: "Responses");

            migrationBuilder.DropColumn(
                name: "ResponseId",
                table: "Attachments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AttachmentId",
                table: "Responses",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ResponseId",
                table: "Attachments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ResponseId",
                table: "Attachments",
                column: "ResponseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Responses_ResponseId",
                table: "Attachments",
                column: "ResponseId",
                principalTable: "Responses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
