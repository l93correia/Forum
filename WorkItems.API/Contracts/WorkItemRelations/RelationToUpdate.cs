﻿using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemRelations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemRelations
{
    public class RelationToUpdate
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the identifier of the related to work item.
        /// </summary>
        public long RelatedToWorkItemId { get; set; }

        /// <summary>
        /// Gets or sets the relation type.
        /// </summary>
        public RelationType RelationType { get; set; }
        #endregion
    }
}
