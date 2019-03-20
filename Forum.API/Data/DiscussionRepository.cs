using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Forum.API.Data
{
    public class DiscussionRepository : IResponseRepository
    {
        private readonly DataContext _context;

        public DiscussionRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Discussions> CreateDiscussion(string subject, string comment, int userId)
        {
            var discussion = new Discussions
            {
                CreatedDate = DateTime.Now,
                IsClosed = false,
                UserId = userId,
                Subject = subject,
                Comment = comment
            };
            
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
                .Include(d => d.DiscussionResponses)
                .Include(d => d.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            return discusssion;
        }

        

    }
}