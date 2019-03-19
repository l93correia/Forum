using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.API.Migrations
{
    public partial class updateDB30 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DiscussionParticipantsId",
                table: "OrganizationType",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DiscussionParticipantsId",
                table: "OrganizationType",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
