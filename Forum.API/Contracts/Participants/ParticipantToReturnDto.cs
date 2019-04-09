using Emsa.Mared.Discussions.API.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discussions.API.Contracts.Participants
{
    public class ParticipantToReturnDto
    {
        /// <summary>
		/// Gets or sets the Id.
		/// </summary>
        public long Id { get; set; }

        /// <summary>
		/// Gets or sets the discussion id.
		/// </summary>
        public long DiscussionId { get; set; }

        /// <summary>
		/// Gets or sets the entity id.
		/// </summary>
        public long EntityId { get; set; }

        /// <summary>
		/// Gets or sets the entity type.
		/// </summary>
        public EntityType EntityType { get; set; }
    }
}
