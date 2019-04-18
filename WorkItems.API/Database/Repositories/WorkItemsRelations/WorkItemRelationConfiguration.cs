using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemRelations
{
    /// <summary>
	/// Implements the work item relation entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{WorkItemRelation}" />
    public class WorkItemRelationConfiguration : IEntityTypeConfiguration<WorkItemRelation>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WorkItemRelation> builder)
        {
			// Name
			builder.ToTable($"{nameof(WorkItemRelation)}s");

			// Keys
			builder.HasKey(relation => relation.Id);

			// Indices
			builder.HasIndex(relation => new { relation.RelatedFromWorkItemId, relation.RelatedToWorkItemId, relation.RelationType });
			builder.HasIndex(relation => relation.RelatedFromWorkItemId).IsUnique();
			builder.HasIndex(relation => relation.RelatedToWorkItemId).IsUnique();

			// Properties
			builder.Property(relation => relation.UserId).IsRequired();
			builder.Property(relation => relation.RelationType).IsRequired();
			builder.Property(relation => relation.CreatedAt).IsRequired();
			builder.Property(relation => relation.UpdatedAt);

			// Relationships
			builder
				.HasOne(relation => relation.RelatedFromWorkItem)
				.WithMany(relation => relation.RelatedToWorkItems)
				.HasForeignKey(relation => relation.RelatedFromWorkItemId)
				.OnDelete(DeleteBehavior.Cascade);

			builder
				.HasOne(relation => relation.RelatedToWorkItem)
				.WithMany(relation => relation.RelatedFromWorkItems)
				.HasForeignKey(relation => relation.RelatedToWorkItemId)
				.OnDelete(DeleteBehavior.Cascade);
		}
    }
}