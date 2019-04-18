using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants
{
    /// <summary>
	/// Implements the work item entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{WorkItemParticipant}" />
    public class WorkItemParticipantConfiguration : IEntityTypeConfiguration<WorkItemParticipant>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WorkItemParticipant> builder)
        {
			// Name
			builder.ToTable($"{nameof(WorkItemParticipant)}s");

			// Keys
			builder.HasKey(participant => participant.Id);

			// Indices
			builder.HasIndex(participant => participant.WorkItemId);

			// Properties
			builder.Property(participant => participant.UserId).IsRequired();
			builder.Property(participant => participant.EntityId).IsRequired();
			builder.Property(participant => participant.EntityType).IsRequired();
			builder.Property(participant => participant.CreatedAt).IsRequired();
			builder.Property(participant => participant.UpdatedAt);

			// Relationships
			builder
				.HasOne(participant => participant.WorkItem)
				.WithMany(participant => participant.WorkItemParticipants)
				.HasForeignKey(participant => participant.WorkItemId)
				.OnDelete(DeleteBehavior.Cascade);
		}
    }
}