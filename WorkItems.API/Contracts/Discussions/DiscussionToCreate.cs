using Emsa.Mared.WorkItems.API.Contracts.WorkItemAttachments;
using Emsa.Mared.WorkItems.API.Contracts.WorkItemComments;
using Emsa.Mared.WorkItems.API.Contracts.WorkItemParticipants;
using Emsa.Mared.WorkItems.API.Contracts.WorkItemRelations;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using System;
using System.Collections.Generic;

namespace Emsa.Mared.WorkItems.API.Contracts.WorkItemDiscussions
{
    public class DiscussionToCreate
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
