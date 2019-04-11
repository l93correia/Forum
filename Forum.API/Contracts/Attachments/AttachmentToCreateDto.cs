using Emsa.Mared.Discussions.API.Database.Repositories;
using Emsa.Mared.Discussions.API.Database.Repositories.Participants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Discussions.API.Contracts.Attachments
{
    /// <summary>
	/// The request data transfer object to create a participant.
	/// </summary>
    public class AttachmentToCreateDto
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
        public long ExternalId { get; set; }

        /// <summary>
        /// Gets or sets the EntityType.
        /// </summary>
        [Required]
        public string Url { get; set; }
        #endregion
    }
}
