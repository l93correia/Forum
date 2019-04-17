using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Contracts.WorkItemParticipants
{
    public class ParticipantToCreate
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
