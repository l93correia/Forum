﻿using Emsa.Mared.Common.Database.Repositories;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItems;
using System;

namespace Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemRelations
{
	/// <summary>
	/// Defines the work item relation entity.
	/// </summary>
	/// 
	/// <seealso cref="IEntity" />
	public class WorkItemRelation : IEntity
	{
        #region [Constants]
        /// <summary>
        /// The relation does not exist message.
        /// </summary>
        public const string DoesNotExist = "The Relation does not exist.";

        /// <summary>
        /// Empty relation message.
        /// </summary>
        public const string Empty = "No Relation founded.";
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
        /// Gets or sets the identifier of the related from work item.
        /// </summary>
        public long RelatedFromWorkItemId { get; set; }

        /// <summary>
        /// Gets or sets the related from from work item.
        /// </summary>
        public WorkItem RelatedFromWorkItem { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the related to work item.
        /// </summary>
        public long RelatedToWorkItemId { get; set; }

        /// <summary>
        /// Gets or sets the related to work item.
        /// </summary>
        public WorkItem RelatedToWorkItem { get; set; }

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

    #region [Enum]
    /// <summary>
	/// Defines the relation type.
	/// </summary>
    public enum RelationType
    {
        Related = 0
    }
    #endregion
}
