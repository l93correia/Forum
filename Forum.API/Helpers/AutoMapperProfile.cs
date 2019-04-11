using AutoMapper;
using Discussions.API.Contracts.Attachments;
using Emsa.Mared.Discussions.API.Contracts;
using Emsa.Mared.Discussions.API.Contracts.Participants;
using Emsa.Mared.Discussions.API.Database.Repositories;
using Emsa.Mared.Discussions.API.Database.Repositories.Attachments;
using Emsa.Mared.Discussions.API.Database.Repositories.Discussions;
using Emsa.Mared.Discussions.API.Database.Repositories.Participants;
using Emsa.Mared.Discussions.API.Database.Repositories.Responses;
using System;
using System.Linq;

namespace Emsa.Mared.Discussions.API.Helpers
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
            //Discussion
            CreateMap<DiscussionToCreateDto, Discussion>();
            CreateMap<UpdateDiscussionDto, Discussion>();
            CreateMap<Discussion, DiscussionToReturnDto>()
                .ForMember(destination => destination.Status, e => e
                    .MapFrom(source => source.Status != "Deleted" && source.EndDate.HasValue && DateTime.Now >= source.EndDate.Value ? "Closed" : source.Status
                    ));

            CreateMap<Discussion, DiscussionForListDto>()
                .ForMember(destination => destination.ResponsesCount, e => e
                    .MapFrom(source => source.Responses.Count()));

            //Response
            CreateMap<ResponseToCreateDto, Response>();
            CreateMap<UpdateResponseDto, Response>();
            CreateMap<Response, ResponseToReturnDto>();

            //Participant
            CreateMap<ParticipantToCreateDto, Participant>();
            CreateMap<ParticipantToUpdateDto, Participant>();
            CreateMap<Participant, ParticipantToReturnDto>();

            //Attachment
            CreateMap<AttachmentToCreateDto, Attachment>();
            CreateMap<AttachmentToUpdateDto, Attachment>();
            CreateMap<Attachment, AttachmentToReturnDto>();
        }
    }
}
