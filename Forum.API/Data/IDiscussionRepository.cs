using Forum.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Data
{
    public interface IResponseRepository
    {
        Task<Discussions> CreateDiscussion(string subject, string comment, int userId);
        Task<List<Discussions>> GetDiscussions();
        Task<Discussions> GetDiscussion(int id);
    }
}
