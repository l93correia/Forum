﻿using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemAttachments;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemComments;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemParticipants;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemRelations;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemDiscussions
{
    public class DiscussionToReturn
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
		public WorkItemStatus Status { get; set; }

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
		public List<AttachmentToReturn> WorkItemAttachments { get; set; }

		/// <summary>
		/// Gets or sets the Responses.
		/// </summary>
		public List<CommentToReturn> WorkItemComments { get; set; }

		/// <summary>
		/// Gets or sets the Participants.
		/// </summary>
		public List<ParticipantToReturn> WorkItemParticipants { get; set; }

		/// <summary>
		/// Gets or sets the related to work items.
		/// </summary>
		public List<RelationToReturn> RelatedToWorkItems { get; set; }

		/// <summary>
		/// Gets or sets the related from work items.
		/// </summary>
		public List<RelationToReturn> RelatedFromWorkItems { get; set; }
		#endregion
	}
}
