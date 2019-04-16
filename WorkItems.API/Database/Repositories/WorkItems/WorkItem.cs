using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemComments;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants;
using System;
using System.Collections.Generic;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemAttachments;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemsRelations;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems
{ 
    /// <summary>
	/// Defines the work item entity.
	/// </summary>
    public class WorkItem
    {
        #region [Constants]
        /// <summary>
        /// The Work Item does not exist message.
        /// </summary>
        public const string DoesNotExist = "The Work Item does not exist.";

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
        public WorkItemType WorkItemType { get; set; }

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
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the UpdatedDate.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the ClosedDate.
        /// </summary>
        public DateTime? ClosedDate { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// Gets or sets the StartDate.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the EndDate.
        /// </summary>
        public DateTime? EndDate { get; set; }

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
    public enum Status
    {
        Default = 0,
        Created = 1,
        Updated = 2,
        Closed = 3,
        Removed = 4
    }
    #endregion
}
