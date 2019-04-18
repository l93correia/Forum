using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using System;

namespace Emsa.Mared.WorkItems.API.Contracts.WorkItemDiscussions
{
    public class EventToList
    {
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
		public DateTime CreatedAt { get; set; }

		/// <summary>
		/// Gets or sets the UpdatedDate.
		/// </summary>
		public DateTime UpdatedAt { get; set; }

		/// <summary>
		/// Gets or sets the ClosedDate.
		/// </summary>
		public DateTime ClosedAt { get; set; }

		/// <summary>
		/// Gets or sets the Status.
		/// </summary>
		public Status Status { get; set; }

		/// <summary>
		/// Gets or sets the StartDate.
		/// </summary>
		public DateTime StartsAt { get; set; }

		/// <summary>
		/// Gets or sets the EndDate.
		/// </summary>
		public DateTime EndsAt { get; set; }

		/// <summary>
		/// Gets or sets the IsPublic.
		/// </summary>
		public bool IsPublic { get; set; }

		/// <summary>
		/// Gets or sets the Document.
		/// </summary>
		public long WorkItemAttachmentsCount { get; set; }

		/// <summary>
		/// Gets or sets the Responses.
		/// </summary>
		public long WorkItemCommentsCount { get; set; }

		/// <summary>
		/// Gets or sets the Participants.
		/// </summary>
		public long WorkItemParticipantsCount { get; set; }

		/// <summary>
		/// Gets or sets the related to work items.
		/// </summary>
		public long RelatedToWorkItemsCount { get; set; }

		/// <summary>
		/// Gets or sets the related from work items.
		/// </summary>
		public long RelatedFromWorkItemsCount { get; set; }
		#endregion
	}
}
