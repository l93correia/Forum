using Emsa.Mared.Common.Database.Repositories;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants
{
	/// <summary>
	/// Defines the work item participants entity.
	/// </summary>
	/// 
	/// <seealso cref="IEntity" />
	public class WorkItemParticipant : IEntity
	{
        #region [Constants]
        /// <summary>
        /// The Work Item Participant does not exist message.
        /// </summary>
        public const string DoesNotExist = "The Work Item Participant does not exist.";
        #endregion

        #region [Properties]
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the WorkItemId.
        /// </summary>
        public long WorkItemId { get; set; }

        /// <summary>
        /// Gets or sets the WorkItem.
        /// </summary>
        public WorkItem WorkItem { get; set; }

        /// <summary>
        /// Gets or sets the EntityId.
        /// </summary>
        public long EntityId { get; set; }

        /// <summary>
        /// Gets or sets the EntityType.
        /// </summary>
        public EntityType EntityType { get; set; }

		/// <summary>
		/// Gets or sets the created date.
		/// </summary>
		public DateTime CreatedAt { get; set; }

		/// <summary>
		/// Gets or sets the updated date.
		/// </summary>
		public DateTime? UpdatedAt { get; set; }
		#endregion
	}

    #region [Enum]
    /// <summary>
	/// Defines the entity type.
	/// </summary>
    public enum EntityType
    {
        Default = 0,
        User = 1,
        Group = 2,
        Organization = 3
    }
    #endregion
}
