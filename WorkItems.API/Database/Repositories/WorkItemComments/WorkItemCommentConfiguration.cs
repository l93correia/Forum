using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemComments
{
    /// <summary>
	/// Implements the response entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{WorkItemComment}" />
    public class WorkItemCommentConfiguration : IEntityTypeConfiguration<WorkItemComment>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WorkItemComment> builder)
        {
			// Name
			builder.ToTable($"{nameof(WorkItemComment)}s");

			// Keys
			builder.HasKey(comment => comment.Id);

			// Indices
			builder.HasIndex(comment => comment.WorkItemId);

			// Properties
			builder.Property(comment => comment.Status).IsRequired();
			builder.Property(comment => comment.Comment).IsRequired().HasMaxLength(500);
			builder.Property(comment => comment.UserId).IsRequired();
			builder.Property(comment => comment.CreatedAt).IsRequired();
			builder.Property(comment => comment.UpdatedAt);

			// Relationships
			builder
				.HasOne(comment => comment.WorkItem)
				.WithMany(comment => comment.WorkItemComments)
				.HasForeignKey(comment => comment.WorkItemId)
				.OnDelete(DeleteBehavior.Cascade);
		}
    }
}