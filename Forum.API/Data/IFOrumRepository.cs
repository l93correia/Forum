using Forum.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Data
{
    public interface IForumRepository
    {
        Task<Discussions> CreateDiscussion(Discussions discussion);
        Task<List<Discussions>> GetDiscussions();
        Task<Discussions> GetDiscussion(int id);
        Task<DiscussionResponses> AddResponse(DiscussionResponses response);
        //Task<DiscussionResponses> GetResponses(int discussionId);
    }
}
