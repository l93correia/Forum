using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models
{
    /// <summary>
	/// Defines the organization type entity.
	/// </summary>
    public class OrganizationType
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the discussion participants id.
        /// </summary>
        public long DiscussionParticipantsId { get; set; }

        /// <summary>
        /// Gets or sets the discussion participants.
        /// </summary>
        public DiscussionParticipants DiscussionParticipants { get; set; }
        #endregion
    }
}
