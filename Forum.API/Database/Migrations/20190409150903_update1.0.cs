using Microsoft.EntityFrameworkCore.Migrations;

namespace Discussions.API.Migrations
{
    public partial class update10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DocumentId",
                table: "Discussions",
                newName: "AttachmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AttachmentId",
                table: "Discussions",
                newName: "DocumentId");
        }
    }
}
