using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants
{
    /// <summary>
	/// Implements the discussion entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{Participant}" />
    public class WorkItemParticipantConfiguration : IEntityTypeConfiguration<WorkItemParticipant>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WorkItemParticipant> builder)
        {
            builder
                .HasOne(s => s.WorkItem)
                .WithMany(g => g.WorkItemParticipants)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
