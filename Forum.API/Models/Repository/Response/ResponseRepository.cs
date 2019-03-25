using Emsa.Mared.Common.Models;
using Forum.API.Models;
using Forum.API.Models.Repository.Response;
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

        public async Task<DiscussionResponses> Create(DiscussionResponses responseToCreate)
        {
            if (string.IsNullOrWhiteSpace(responseToCreate.Response))
                throw new ModelException(responseToCreate.InvalidFieldMessage(p => p.Response));
            if (string.IsNullOrWhiteSpace(responseToCreate.CreatedById.ToString()))
                throw new ModelException(responseToCreate.InvalidFieldMessage(p => p.CreatedById));

            await _context.AddAsync(responseToCreate);
            await _context.SaveChangesAsync();

            return responseToCreate;
        }

        public async Task<List<DiscussionResponses>> GetAll()
        {
            var responses = this.GetQueryable();

            return await responses.ToListAsync();
        }

        public async Task<DiscussionResponses> Get(long id)
        {
            var response = await _context.DiscussionResponses
                .Include(d => d.CreatedBy)
                .FirstOrDefaultAsync(x => x.Id == id);

            return response;
        }

        public async Task<DiscussionResponses> Update(DiscussionResponses updateResponse)
        {
            var databaseResponse = await _context.DiscussionResponses.FindAsync(updateResponse.Id);
            if (databaseResponse == null)
                throw new ModelException(DiscussionResponses.DoesNotExist, true);

            if (string.IsNullOrWhiteSpace(updateResponse.Response))
                throw new ModelException(updateResponse.InvalidFieldMessage(p => p.Response));

            var response = await _context.DiscussionResponses
                .FirstOrDefaultAsync(x => x.Id == updateResponse.Id);

            response.Response = updateResponse.Response;

            await _context.SaveChangesAsync();

            return response;
        }

        public async Task Delete(long id)
        {
            var databaseResponse = await _context.DiscussionResponses.FindAsync(id);
            if (databaseResponse == null)
                throw new ModelException(DiscussionResponses.DoesNotExist, true);

            var response = await _context.DiscussionResponses.FindAsync(id);

            _context.DiscussionResponses.Remove(response);

            await _context.SaveChangesAsync();
        }

        public async Task<PagedList<DiscussionResponses>> GetAll(ResponseParameters parameters)
        {
            var responses = this.GetQueryable();

            return await PagedList<DiscussionResponses>.CreateAsync(responses, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<bool> Exists(long entityId)
        {
            throw new NotImplementedException();
        }

        private IQueryable<DiscussionResponses> GetQueryable()
        {
            return _context.DiscussionResponses;
        }

        private IQueryable<DiscussionResponses> GetQueryableByDiscussion(long discussionId)
        {
            return this.GetQueryable()
                .Where(p => p.DiscussionId == discussionId);
        }

        public async Task<PagedList<DiscussionResponses>> GetByDiscussion(long discussionId, ResponseParameters parameters)
        {
            var responses = this.GetQueryableByDiscussion(discussionId);

            return await PagedList<DiscussionResponses>.CreateAsync(responses, parameters.PageNumber, parameters.PageSize);
        }
    }
}
