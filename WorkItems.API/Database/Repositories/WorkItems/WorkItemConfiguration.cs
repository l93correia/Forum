using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems
{
    /// <summary>
	/// Implements the discussion entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{WorkItem}" />
    public class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WorkItem> builder)
		{
			// Name
			builder.ToTable($"{nameof(WorkItem)}s");

			// Keys
			builder.HasKey(workItem => workItem.Id);

			// Indices
			builder.HasIndex(workItem => workItem.Type);
			builder.HasIndex(workItem => workItem.IsPublic);
			builder.HasIndex(workItem => workItem.Status);

			// Properties
			builder.Property(workItem => workItem.Type).IsRequired();
			builder.Property(workItem => workItem.Title).IsRequired().HasMaxLength(100);
			builder.Property(workItem => workItem.Summary).IsRequired().HasMaxLength(500);
			builder.Property(workItem => workItem.Body).IsRequired().HasMaxLength(2500);
			builder.Property(workItem => workItem.Location).IsRequired().HasMaxLength(500);
			builder.Property(workItem => workItem.Status).IsRequired();
			builder.Property(workItem => workItem.IsPublic).IsRequired();
			builder.Property(workItem => workItem.CreatedAt).IsRequired();
			builder.Property(workItem => workItem.UpdatedAt);
			builder.Property(workItem => workItem.ClosedAt);
			builder.Property(workItem => workItem.StartsAt);
			builder.Property(workItem => workItem.EndsAt);
		}
    }
}