using Emsa.Mared.Common.Models;
using Forum.API.Dtos;
using Forum.API.Models;
using Forum.API.Models.Repository.Discussion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Data
{
    public interface IDiscussionRepository : IRepository<Discussions, long, DiscussionParameters>
    {
    }
}
