using Forum.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Data
{
    interface IResponseRepository
    {
        Task<DiscussionResponses> AddResponse(string response, int userId, int discussionId);
        //Task<DiscussionResponses> GetResponses(int discussionId);
    }
}
