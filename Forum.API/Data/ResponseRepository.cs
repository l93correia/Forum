using Forum.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Data
{
    public class ResponseRepository : IResponseRepository
    {
        private readonly DataContext _context;

        public ResponseRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<DiscussionResponses> AddResponse(string response, int userId, int discussionId)
        {
            var discussionResponse = new DiscussionResponses
            {
                DiscussionId = discussionId,
                CreatedById = userId,
                Response = response,
                CreatedDate = DateTime.Now
            };

            await _context.DiscussionResponses.AddAsync(discussionResponse);
            await _context.SaveChangesAsync();

            return discussionResponse;
        }


        //public async task<list<discussionresponses>> getresponses(int discussionid)
        //{
        //    var responses = await _context.discussionresponses
        //            .where(u => u.discussionid == discussionid).tolistasync();

        //    return responses;
        //}

    }
}
