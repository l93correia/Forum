using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models.Repository.Response
{
    public class ResponseConfiguration : IEntityTypeConfiguration<DiscussionResponses>
    {
        public void Configure(EntityTypeBuilder<DiscussionResponses> builder)
        {
            builder.Property(response => response.CreatedDate)
                .IsRequired()
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("CURRENT_TIMESTAMP(6)");

            builder.Property(response => response.Response)
                .IsRequired()
                .HasMaxLength(500);

            builder
                .HasOne(s => s.Discussion)
                .WithMany(g => g.DiscussionResponses)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
