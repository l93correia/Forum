using Emsa.Mared.Discussions.API.Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Database.Repositories.Participants
{
    /// <summary>
	/// Defines the discussion participants entity.
	/// </summary>
    public class Participant
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
        /// Gets or sets the DiscussionId.
        /// </summary>
        public long EntityId { get; set; }

        /// <summary>
        /// Gets or sets the Discussion.
        /// </summary>
        public EntityType EntityType { get; set; }
        #endregion
    }

    public enum EntityType
    {
        User = 1,
        Group = 2,
        Organization = 3
    }
}
