using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            builder.Property(workItem => workItem.Body)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(workItem => workItem.Title)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
