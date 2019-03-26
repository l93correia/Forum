using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models.Repository.Discussion
{
    public class DiscussionConfiguration : IEntityTypeConfiguration<Discussions>
    {
        public void Configure(EntityTypeBuilder<Discussions> builder)
        {
            builder.Property(discussion => discussion.CreatedDate)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");
            //.HasDefaultValueSql("GetUtcDate()");

            builder.Property(discussion => discussion.Comment)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(discussion => discussion.Subject)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}
