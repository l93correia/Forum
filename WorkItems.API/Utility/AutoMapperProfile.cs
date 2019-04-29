using AutoMapper;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemAttachments;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemComments;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemDiscussions;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemDocuments;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemEvents;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemParticipants;
using Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemRelations;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemAttachments;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemComments;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemRelations;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItems;
using System;
using System.Linq;

using WorkItemStatus = Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItems.WorkItemStatus;

namespace Emsa.Mared.ContentManagement.WorkItems.Utility
{
    /// <summary>
	/// Implements the auto mapper profile.
	/// </summary>
	/// 
	/// <seealso cref="Profile" />
    public class AutoMapperProfile : Profile
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="AutoMapperProfile"/> class.
		/// </summary>
        public AutoMapperProfile()
        {
			//WorkItemDiscussion
			CreateMap<DiscussionToCreate, WorkItem>()
				.ForMember(destination => destination.Type, e => e
					.MapFrom(source => WorkItemType.Discussion));

			CreateMap<DiscussionToUpdate, WorkItem>();
            CreateMap<WorkItem, DiscussionToReturn>()
                .ForMember(destination => destination.Status, e => e.MapFrom
                (
                    source => source.ClosedAt.HasValue && DateTime.UtcNow >= source.ClosedAt.Value
                    ? WorkItemStatus.Closed
                    : source.Status
                ));

            CreateMap<WorkItem, DiscussionToList>()
                .ForMember(destination => destination.WorkItemCommentsCount, e => e
                    .MapFrom(source => source.WorkItemComments.Count()))
                .ForMember(destination => destination.WorkItemAttachmentsCount, e => e
                    .MapFrom(source => source.WorkItemAttachments.Count()))
                .ForMember(destination => destination.WorkItemParticipantsCount, e => e
                    .MapFrom(source => source.WorkItemParticipants.Count()))
                .ForMember(destination => destination.RelatedFromWorkItemsCount, e => e
                    .MapFrom(source => source.RelatedFromWorkItems.Count()))
                .ForMember(destination => destination.RelatedToWorkItemsCount, e => e
                    .MapFrom(source => source.RelatedToWorkItems.Count()));

            //WorkItemDocument
            CreateMap<DocumentToCreate, WorkItem>()
				.ForMember(destination => destination.Type, e => e
					.MapFrom(source => WorkItemType.Document));

			CreateMap<DocumentToUpdate, WorkItem>();
            CreateMap<WorkItem, DocumentToReturn>()
                .ForMember(destination => destination.Status, e => e.MapFrom
                (
                    source => source.ClosedAt.HasValue && DateTime.UtcNow >= source.ClosedAt.Value
                    ? WorkItemStatus.Closed
                    : source.Status
                ));

            CreateMap<WorkItem, DocumentToList>()
                .ForMember(destination => destination.WorkItemCommentsCount, e => e
                    .MapFrom(source => source.WorkItemComments.Count()))
                .ForMember(destination => destination.WorkItemAttachmentsCount, e => e
                    .MapFrom(source => source.WorkItemAttachments.Count()))
                .ForMember(destination => destination.WorkItemParticipantsCount, e => e
                    .MapFrom(source => source.WorkItemParticipants.Count()))
                .ForMember(destination => destination.RelatedFromWorkItemsCount, e => e
                    .MapFrom(source => source.RelatedFromWorkItems.Count()))
                .ForMember(destination => destination.RelatedToWorkItemsCount, e => e
                    .MapFrom(source => source.RelatedToWorkItems.Count()));

            //WorkItemEvent
            CreateMap<EventToCreate, WorkItem>()
				.ForMember(destination => destination.Type, e => e
					.MapFrom(source => WorkItemType.Event));

			CreateMap<EventToUpdate, WorkItem>();
            CreateMap<WorkItem, EventToReturn>()
                .ForMember(destination => destination.Status, e => e.MapFrom
                (
                    source => source.ClosedAt.HasValue && DateTime.UtcNow >= source.ClosedAt.Value
                    ? WorkItemStatus.Closed
                    : source.Status
                ));

            CreateMap<WorkItem, EventToList>()
                .ForMember(destination => destination.WorkItemCommentsCount, e => e
                    .MapFrom(source => source.WorkItemComments.Count()))
                .ForMember(destination => destination.WorkItemAttachmentsCount, e => e
                    .MapFrom(source => source.WorkItemAttachments.Count()))
                .ForMember(destination => destination.WorkItemParticipantsCount, e => e
                    .MapFrom(source => source.WorkItemParticipants.Count()))
                .ForMember(destination => destination.RelatedFromWorkItemsCount, e => e
                    .MapFrom(source => source.RelatedFromWorkItems.Count()))
                .ForMember(destination => destination.RelatedToWorkItemsCount, e => e
                    .MapFrom(source => source.RelatedToWorkItems.Count()));

            //WorkItemComment
            CreateMap<CommentToCreate, WorkItemComment>();
            CreateMap<CommentToUpdate, WorkItemComment>();
            CreateMap<WorkItemComment, CommentToReturn>();

            //WorkItemParticipant
            CreateMap<ParticipantToCreate, WorkItemParticipant>();
            CreateMap<ParticipantToUpdate, WorkItemParticipant>();
            CreateMap<WorkItemParticipant, ParticipantToReturn>();

            //WorkItemAttachment
            CreateMap<AttachmentToCreate, WorkItemAttachment>();
            CreateMap<AttachmentToUpdate, WorkItemAttachment>();
            CreateMap<WorkItemAttachment, AttachmentToReturn>();

            //WorkItemRelation
            CreateMap<RelationToCreate, WorkItemRelation>();
            CreateMap<RelationToUpdate, WorkItemRelation>();
            CreateMap<WorkItemRelation, RelationToReturn>();
        }
    }
}
