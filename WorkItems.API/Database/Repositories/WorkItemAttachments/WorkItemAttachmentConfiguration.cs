using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemAttachments
{
    /// <summary>
	/// Implements the attachment entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{Attachment}" />
    public class WorkItemAttachmentConfiguration : IEntityTypeConfiguration<WorkItemAttachment>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WorkItemAttachment> builder)
        {
            builder
                .HasOne(s => s.Discussion)
                .WithMany(g => g.Attachments)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
