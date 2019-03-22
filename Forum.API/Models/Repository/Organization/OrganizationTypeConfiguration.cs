using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models.Repository.Organization
{
    public class OrganizationTypeConfiguration : IEntityTypeConfiguration<OrganizationType>
    {
        public void Configure(EntityTypeBuilder<OrganizationType> builder)
        {
            builder
                .HasOne(s => s.DiscussionParticipants)
                .WithMany(g => g.OrganizationType)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
