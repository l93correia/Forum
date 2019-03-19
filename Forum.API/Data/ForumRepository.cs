using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Forum.API.Data
{
    public class ForumRepository : IForumRepository
    {
        private readonly DataContext _context;

        public ForumRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Discussions> CreateDiscussion(Discussions discussion)
        {
            discussion.CreatedDate = DateTime.Now;
            discussion.IsClosed = false;
            discussion.UserId = 1; //getUserId

            await _context.Discussions.AddAsync(discussion);
            await _context.SaveChangesAsync();

            return discussion;
        }

        public async Task<List<Discussions>> GetDiscussions()
        {
            var discusssions = await _context.Discussions.ToListAsync();

            return discusssions;
        }

        public async Task<Discussions> GetDiscussion(int id)
        {
            var discusssion = await _context.Discussions
                .Include("DiscussionResponses")
                .FirstOrDefaultAsync(x => x.Id == id);

            return discusssion;
        }

        public async Task<DiscussionResponses> AddResponse(DiscussionResponses response)
        {
            response.CreatedDate = DateTime.Now;
            response.CreatedById = 1; // getUserId

            await _context.DiscussionResponses.AddAsync(response);
            await _context.SaveChangesAsync();

            return response;
        }


        //public async Task<List<DiscussionResponses>> GetResponses(int discussionId)
        //{
        //    var responses = await _context.DiscussionResponses
        //            .Where(u => u.DiscussionId == discussionId).ToListAsync();

        //    return responses;
        //}
    }
}