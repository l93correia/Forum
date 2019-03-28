using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models.Repository.Discussions
{
    /// <summary>
	/// Implements the discussion entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{Discussion}" />
    public class DiscussionConfiguration : IEntityTypeConfiguration<Discussion>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Discussion> builder)
        {
            builder.Property(discussion => discussion.Comment)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(discussion => discussion.Subject)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
