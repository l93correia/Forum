using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Contracts.WorkItemParticipants
{
    public class ParticipantToReturn
    {
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
}
