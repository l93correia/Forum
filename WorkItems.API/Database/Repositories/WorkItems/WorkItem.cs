using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItems;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemComments;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemParticipants;
using System;
using System.Collections.Generic;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemAttachments;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemRelations;
using Emsa.Mared.Common.Database.Repositories;

namespace Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItems
{
	/// <summary>
	/// Defines the work item entity.
	/// </summary>
	/// 
	/// <seealso cref="IEntity" />
	public class WorkItem : IEntity
    {
        #region [Constants]
        /// <summary>
        /// The Work Item does not exist message.
        /// </summary>
        public const string DoesNotExist = "The Work Item does not exist.";

		/// <summary>
		/// The Work Item invalid status transition message.
		/// </summary>
		public const string InvalidStatusTransition = "Invalid status transition from {0} to {1}.";

		/// <summary>
		/// The Work Item cannot change type message.
		/// </summary>
		public const string CannotChangeType = "Invalid type change. Work items cannot change type";

		/// <summary>
		/// Empty Work Item message.
		/// </summary>
		public const string Empty = "No Work Item founded.";
        #endregion

        #region [Properties]
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the work item type.
        /// </summary>
        public WorkItemType Type { get; set; }

        /// <summary>
        /// Gets or sets the work item sub type.
        /// </summary>
        //public WorkItemSubType WorkItemSubType { get; set; }

        /// <summary>
        /// Gets or sets the Subject.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the Summary.
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the CreationDate.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the UpdatedDate.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the ClosedDate.
        /// </summary>
        public DateTime? ClosedAt { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        public WorkItemStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the StartDate.
        /// </summary>
        public DateTime? StartsAt { get; set; }

        /// <summary>
        /// Gets or sets the EndDate.
        /// </summary>
        public DateTime? EndsAt { get; set; }

        /// <summary>
        /// Gets or sets the IsPublic.
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets the Document.
        /// </summary>
        public ICollection<WorkItemAttachment> WorkItemAttachments { get; set; }

        /// <summary>
        /// Gets or sets the Responses.
        /// </summary>
        public ICollection<WorkItemComment> WorkItemComments { get; set; }

        /// <summary>
        /// Gets or sets the Participants.
        /// </summary>
        public ICollection<WorkItemParticipant> WorkItemParticipants { get; set; }

        /// <summary>
        /// Gets or sets the related to work items.
        /// </summary>
        public ICollection<WorkItemRelation> RelatedToWorkItems { get; set; }

        /// <summary>
        /// Gets or sets the related from work items.
        /// </summary>
        public ICollection<WorkItemRelation> RelatedFromWorkItems { get; set; }
        #endregion
    }

    #region [Enum]
    /// <summary>
	/// Defines the WorkItemType.
	/// </summary>
    public enum WorkItemType
    {
        Default = 0,
        Event = 1,
        Document = 2,
        Discussion = 3,
        WorkingSheet = 4,
        Recommendation = 5,
        News = 6,
    }

    /// <summary>
	/// Defines the WorkItemSubType.
	/// </summary>
    public enum WorkItemSubType
    {
        Default = 0,
        EventMeeting = 1,
        Event = 2,
        Recommendation = 3
    }

    /// <summary>
	/// Defines the status.
	/// </summary>
    public enum WorkItemStatus
    {
        Default = 0,
        Created = 1,
        Updated = 2,
        Closed = 3,
        Removed = 4
    }
    #endregion
}
