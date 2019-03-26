using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models.Repository.Organization
{
    /// <summary>
	/// Implements the organization type entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{OrganizationType}" />
    public class OrganizationTypeConfiguration : IEntityTypeConfiguration<OrganizationType>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<OrganizationType> builder)
        {
            builder
                .HasOne(s => s.DiscussionParticipants)
                .WithMany(g => g.OrganizationType)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
