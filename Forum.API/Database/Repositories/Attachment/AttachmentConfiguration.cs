using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Database.Repositories.Attachments
{
    /// <summary>
	/// Implements the attachment entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{Attachment}" />
    public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Attachment> builder)
        {
            builder
                .HasOne(s => s.Discussion)
                .WithMany(g => g.Attachments)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
