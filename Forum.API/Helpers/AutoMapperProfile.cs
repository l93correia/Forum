using AutoMapper;
using Forum.API.Dtos;
using Forum.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Helpers
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
                .ForMember(destination => destination.Username, e => e
                    .MapFrom(source => source.User.Name))
                .ForMember(destination => destination.Status, e => e
                    .MapFrom(source => source.Status != "Deleted" && source.EndDate.HasValue && DateTime.Now >= source.EndDate.Value ? "Closed" : source.Status
                    ));

            CreateMap<Discussion, DiscussionForListDto>()
                .ForMember(destination => destination.Username, e => e
                    .MapFrom(source => source.User.Name))
                .ForMember(destination => destination.ResponsesCount, e => e
                    .MapFrom(source => source.DiscussionResponses.Count()));

            //Response
            CreateMap<ResponseToCreateDto, DiscussionResponse>();
            CreateMap<UpdateResponseDto, DiscussionResponse>();
            CreateMap<DiscussionResponse, ResponseToReturnDto>()
                .ForMember(destination => destination.Username, e => e
                    .MapFrom(source => source.CreatedBy.Name));

            
        }
    }
}
