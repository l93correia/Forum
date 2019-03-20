using AutoMapper;
using Forum.API.Dtos;
using Forum.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Discussions, DiscussionToReturnDto>()
                .ForMember(destination => destination.Username, e => e.MapFrom(source => source.User.Name));
            CreateMap<Discussions, DiscussionForListDto>()
                .ForMember(destination => destination.Username, e => e
                    .MapFrom(source => source.User.Name));
            CreateMap<DiscussionResponses, ResponseToReturnDto>()
                .ForMember(destination => destination.Username, e => e.MapFrom(source => source.CreatedBy.Name));
        }
    }
}
