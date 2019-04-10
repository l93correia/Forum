using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Database.Repositories
{
    /// <summary>
	/// Defines the discussion participants entity.
	/// </summary>
    public class Participant
    {
        #region [Constants]
        /// <summary>
        /// The Participant does not exist message.
        /// </summary>
        public const string DoesNotExist = "The Participant does not exist.";
        #endregion

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
        /// Gets or sets the EntityId.
        /// </summary>
        public long EntityId { get; set; }

        /// <summary>
        /// Gets or sets the EntityType.
        /// </summary>
        public EntityType EntityType { get; set; }
        #endregion
    }

    /// <summary>
	/// Defines the entity type.
	/// </summary>
    public enum EntityType
    {
        User = 1,
        Group = 2,
        Organization = 3
    }
}
