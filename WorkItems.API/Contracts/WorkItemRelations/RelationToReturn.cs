﻿using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemRelations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemRelations
{
    public class RelationToReturn
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
        /// Gets or sets the identifier of the related from work item.
        /// </summary>
        public long RelatedFromWorkItemId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the related to work item.
        /// </summary>
        public long RelatedToWorkItemId { get; set; }

        /// <summary>
        /// Gets or sets the relation type.
        /// </summary>
        public RelationType RelationType { get; set; }

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
