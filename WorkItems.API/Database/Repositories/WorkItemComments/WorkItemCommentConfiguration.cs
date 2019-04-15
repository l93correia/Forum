using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemComments
{
    /// <summary>
	/// Implements the response entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{Response}" />
    public class WorkItemCommentConfiguration : IEntityTypeConfiguration<WorkItemComment>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WorkItemComment> builder)
        {
            builder.Property(response => response.Comment)
                .IsRequired()
                .HasMaxLength(500);

            builder
                .HasOne(s => s.WorkItem)
                .WithMany(g => g.WorkItemComments)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
