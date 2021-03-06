﻿using Emsa.Mared.Discussions.API.Database.Repositories;
using Emsa.Mared.Discussions.API.Database.Repositories.Participants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Contracts.Participants
{
    /// <summary>
	/// The request data transfer object to create a participant.
	/// </summary>
    public class ParticipantToReturnDto
    {
        #region [Properties]
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
        #endregion
    }
}
