using AutoMapper;
using Emsa.Mared.WorkItems.API.Contracts.WorkItemAttachments;
using Emsa.Mared.WorkItems.API.Contracts.WorkItemComments;
using Emsa.Mared.WorkItems.API.Contracts.WorkItemDiscussions;
using Emsa.Mared.WorkItems.API.Contracts.WorkItemDocuments;
using Emsa.Mared.WorkItems.API.Contracts.WorkItemEvents;
using Emsa.Mared.WorkItems.API.Contracts.WorkItemParticipants;
using Emsa.Mared.WorkItems.API.Contracts.WorkItemRelations;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemAttachments;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemComments;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemRelations;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using System;
using System.Linq;

using WorkItemStatus = Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems.Status;

namespace Emsa.Mared.WorkItems.API.Helpers
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
