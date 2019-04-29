using Microsoft.EntityFrameworkCore.Migrations;

namespace Emsa.Mared.ContentManagement.WorkItems.Database.Migrations
{
	public partial class RenamedColumns : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "IX_WorkItems_WorkItemType",
				table: "WorkItems");

			migrationBuilder.DropColumn(
				name: "WorkItemType",
				table: "WorkItems");

			migrationBuilder.AddColumn<int>(
				name: "Type",
				table: "WorkItems",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.CreateIndex(
				name: "IX_WorkItems_Type",
				table: "WorkItems",
				column: "Type");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropIndex(
				name: "IX_WorkItems_Type",
				table: "WorkItems");

			migrationBuilder.DropColumn(
				name: "Type",
				table: "WorkItems");

			migrationBuilder.AddColumn<int>(
				name: "WorkItemType",
				table: "WorkItems",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.CreateIndex(
				name: "IX_WorkItems_WorkItemType",
				table: "WorkItems",
				column: "WorkItemType");
		}
	}
}
