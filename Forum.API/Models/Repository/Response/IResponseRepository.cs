using Emsa.Mared.Common.Models;
using Forum.API.Models;
using Forum.API.Models.Repository.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Data
{
    public interface IResponseRepository : IRepository<DiscussionResponses, long, ResponseParameters>
    {
        Task<PagedList<DiscussionResponses>> GetByDiscussion(long id, ResponseParameters parameters);
    }
}
