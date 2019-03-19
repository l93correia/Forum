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

        public async Task<Discussions> CreateDiscussion(Discussions discussion, string subject, string comment)
        {
            discussion.Subject = subject;
            discussion.Comment = comment;
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
    }
}
