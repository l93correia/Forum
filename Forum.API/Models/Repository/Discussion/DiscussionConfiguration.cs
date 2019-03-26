using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models.Repository.Discussion
{
    /// <summary>
	/// Implements the discussion entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{Discussions}" />
    public class DiscussionConfiguration : IEntityTypeConfiguration<Discussions>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Discussions> builder)
        {
            builder.Property(discussion => discussion.CreatedDate)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            builder.Property(discussion => discussion.Status)
                .IsRequired()
                .HasMaxLength(50)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("'Created'");

            builder.Property(discussion => discussion.Comment)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(discussion => discussion.Subject)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
