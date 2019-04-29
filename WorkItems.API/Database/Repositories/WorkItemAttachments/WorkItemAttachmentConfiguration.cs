using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemAttachments
{
    /// <summary>
	/// Implements the attachment entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{WorkItemAttachment}" />
    public class WorkItemAttachmentConfiguration : IEntityTypeConfiguration<WorkItemAttachment>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WorkItemAttachment> builder)
		{
			// Name
			builder.ToTable($"{nameof(WorkItemAttachment)}s");

			// Keys
			builder.HasKey(attachment => attachment.Id);

			// Indices
			builder.HasIndex(attachment => attachment.WorkItemId);

			// Properties
			builder.Property(attachment => attachment.ExternalId).IsRequired().HasMaxLength(500);
			builder.Property(attachment => attachment.Url).IsRequired().HasMaxLength(500);
			builder.Property(attachment => attachment.UserId).IsRequired();
			builder.Property(attachment => attachment.CreatedAt).IsRequired();
			builder.Property(attachment => attachment.UpdatedAt);

			// Relationships
			builder
				.HasOne(attachment => attachment.WorkItem)
				.WithMany(attachment => attachment.WorkItemAttachments)
				.HasForeignKey(attachment => attachment.WorkItemId)
				.OnDelete(DeleteBehavior.Cascade);
		}
    }
}