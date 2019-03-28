using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.API.Migrations
{
    public partial class setDefaultValues2Repo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Discussions",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldDefaultValueSql: "'Created'");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Discussions",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "CURRENT_TIMESTAMP(6)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "DiscussionResponses",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldDefaultValueSql: "'Created'");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "DiscussionResponses",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldDefaultValueSql: "CURRENT_TIMESTAMP(6)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Discussions",
                maxLength: 50,
                nullable: false,
                defaultValueSql: "'Created'",
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "Discussions",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)",
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "DiscussionResponses",
                maxLength: 50,
                nullable: false,
                defaultValueSql: "'Created'",
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "DiscussionResponses",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP(6)",
                oldClrType: typeof(DateTime));
        }
    }
}
