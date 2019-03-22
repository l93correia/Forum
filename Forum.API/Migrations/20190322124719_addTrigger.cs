using Microsoft.EntityFrameworkCore.Migrations;

namespace Forum.API.Migrations
{
    public partial class addTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql
            (@"
				GO
				CREATE TRIGGER [dbo].[TR_DiscussionResponses_Update]
				ON [dbo].[DiscussionResponses]
					AFTER UPDATE
				AS
				BEGIN
					SET NOCOUNT ON;

					IF ((SELECT TRIGGER_NESTLEVEL()) > 1) RETURN;

					DECLARE @Id BIGINT

					SELECT @Id = INSERTED.Id
					FROM INSERTED

					UPDATE [dbo].[DiscussionResponses]
					SET [UpdatedDate] = GETUTCDATE()
					WHERE Id = @Id
				END
				GO
			");

            migrationBuilder.Sql
            (@"
				GO
				CREATE TRIGGER [dbo].[TR_Discussions_Update]
				ON [dbo].[Discussions]
					AFTER UPDATE
				AS
				BEGIN
					SET NOCOUNT ON;

					IF ((SELECT TRIGGER_NESTLEVEL()) > 1) RETURN;

					DECLARE @Id BIGINT

					SELECT @Id = INSERTED.Id
					FROM INSERTED

					UPDATE [dbo].[Discussions]
					SET [UpdatedDate] = GETUTCDATE()
					WHERE Id = @Id
				END
				GO
			");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql
            (@"
				DROP TRIGGER [dbo].[TR_DiscussionResponses_Update]
				GO
			");
            migrationBuilder.Sql
            (@"
				DROP TRIGGER [dbo].[TR_Discussions_Update]
				GO
			");
        }
    }
}
