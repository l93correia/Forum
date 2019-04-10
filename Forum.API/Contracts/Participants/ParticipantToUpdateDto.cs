using Emsa.Mared.Discussions.API.Database.Repositories;
using Emsa.Mared.Discussions.API.Database.Repositories.Participants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Contracts.Participants
{
    /// <summary>
	/// The request data transfer object to update a participant.
	/// </summary>
    public class ParticipantToUpdateDto
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the DiscussionId.
        /// </summary>
        [Required]
        public long DiscussionId { get; set; }

        /// <summary>
        /// Gets or sets the EntityId.
        /// </summary>
        [Required]
        public long EntityId { get; set; }

        /// <summary>
        /// Gets or sets the EntityType.
        /// </summary>
        [Required]
        public EntityType EntityType { get; set; }
        #endregion
    }
}
