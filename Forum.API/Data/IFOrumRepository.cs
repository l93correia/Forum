using Forum.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Data
{
    interface IForumRepository
    {
        Task<Discussions> CreateDiscussion(Discussions discussion, string subject, string comment);
        Task<List<Discussions>> GetDiscussions();
    }
}
