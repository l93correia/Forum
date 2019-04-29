using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemParticipants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemParticipants
{
    public class ParticipantToUpdate
    {
        #region [Properties]
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
}
