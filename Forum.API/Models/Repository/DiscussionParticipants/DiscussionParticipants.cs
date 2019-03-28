using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models
{
    /// <summary>
	/// Defines the discussion participants entity.
	/// </summary>
    public class DiscussionParticipants
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the DiscussionId.
        /// </summary>
        public long DiscussionId { get; set; }

        /// <summary>
        /// Gets or sets the Discussion.
        /// </summary>
        public Discussion Discussion { get; set; }

        /// <summary>
        /// Gets or sets the OrganizationType.
        /// </summary>
        public ICollection<OrganizationType> OrganizationType { get; set; }
        #endregion
    }
}
