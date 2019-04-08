using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Database.Repositories.Responses
{
    /// <summary>
	/// Implements the response entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{Response}" />
    public class ResponseConfiguration : IEntityTypeConfiguration<Response>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Response> builder)
        {
            builder.Property(response => response.Comment)
                .IsRequired()
                .HasMaxLength(500);

            builder
                .HasOne(s => s.Discussion)
                .WithMany(g => g.Responses)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
