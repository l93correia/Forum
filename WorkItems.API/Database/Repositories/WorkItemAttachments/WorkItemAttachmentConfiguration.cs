﻿using Microsoft.EntityFrameworkCore;
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
	/// <seealso cref="IEntityTypeConfiguration{WorkItemAttachment}" />
    public class WorkItemAttachmentConfiguration : IEntityTypeConfiguration<WorkItemAttachment>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<WorkItemAttachment> builder)
        {
            builder
                .HasOne(s => s.WorkItem)
                .WithMany(g => g.WorkItemAttachments)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}