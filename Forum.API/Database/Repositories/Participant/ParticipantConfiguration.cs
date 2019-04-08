using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Database.Repositories
{
    /// <summary>
	/// Implements the discussion entity framework configuration.
	/// </summary>
	/// 
	/// <seealso cref="IEntityTypeConfiguration{Participant}" />
    public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Participant> builder)
        {
        }
    }
}
