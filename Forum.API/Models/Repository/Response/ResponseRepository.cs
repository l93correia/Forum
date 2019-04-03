using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using Forum.API.Models;
using Forum.API.Models.Repository.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Data
{
    /// <inheritdoc />
    public class ResponseRepository : IResponseRepository
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        private readonly DataContext _context;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="IResponseRepository"/> class.
        /// </summary>
        /// 
        /// <param name="context">The context.</param>
        public ResponseRepository(DataContext context)
        {
            _context = context;
        }
        #endregion

        #region [Methods] IRepository
        /// <inheritdoc />
        public async Task<DiscussionResponse> Create(DiscussionResponse responseToCreate)
        {
            if (string.IsNullOrWhiteSpace(responseToCreate.Response))
                throw new ModelException(responseToCreate.InvalidFieldMessage(p => p.Response));

            var user = await _context.User
                .FirstOrDefaultAsync(x => x.Id == responseToCreate.CreatedById);
            if (user == null)
                throw new ModelException(User.DoesNotExist, true);

            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == responseToCreate.DiscussionId);
            if (discussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);

            responseToCreate.CreatedDate = DateTime.Now;
            responseToCreate.Status = "Created";

            await _context.AddAsync(responseToCreate);
            await _context.SaveChangesAsync();

            return responseToCreate;
        }

        /// <inheritdoc />
        public async Task<List<DiscussionResponse>> GetAll()
        {
            var responses = GetQueryable();

            return await responses.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<DiscussionResponse>> GetByDiscussion(long discussionId)
        {
            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == discussionId);
            if (discussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);

            var responses = GetQueryableByDiscussion(discussionId);

            return await responses.ToListAsync();
            //return await PagedList<DiscussionResponse>.CreateAsync(responses, parameters.PageNumber, parameters.PageSize);
        }

        /// <inheritdoc />
        public async Task<DiscussionResponse> Get(long id)
        {
            var response = await _context.DiscussionResponses
                .Include(d => d.CreatedBy)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (response == null)
            {
                throw new ModelException(DiscussionResponse.DoesNotExist, true);
            }

            return response;
        }

        /// <inheritdoc />
        public async Task<DiscussionResponse> Update(DiscussionResponse updateResponse)
        {
            var databaseResponse = await _context.DiscussionResponses.FindAsync(updateResponse.Id);
            if (databaseResponse == null)
                throw new ModelException(DiscussionResponse.DoesNotExist, true);

            if (string.IsNullOrWhiteSpace(updateResponse.Response))
                throw new ModelException(updateResponse.InvalidFieldMessage(p => p.Response));

            var response = await _context.DiscussionResponses
                .FirstOrDefaultAsync(x => x.Id == updateResponse.Id);

            response.Response = updateResponse.Response;
            response.Status = "Updated";
            response.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return response;
        }

        /// <inheritdoc />
        public async Task Delete(long id)
        {
            var databaseResponse = await _context.DiscussionResponses.FindAsync(id);
            if (databaseResponse == null)
                throw new ModelException(DiscussionResponse.DoesNotExist, true);

            var response = await _context.DiscussionResponses.FindAsync(id);

            response.Status = "Removed";

            //_context.DiscussionResponses.Remove(response);

            //await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<PagedList<DiscussionResponse>> GetAll(ResponseParameters parameters)
        {
            var responses = this.GetQueryable();

            return await PagedList<DiscussionResponse>.CreateAsync(responses, parameters.PageNumber, parameters.PageSize);
        }

        /// <inheritdoc />
        public async Task<bool> Exists(long entityId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region [Methods] Utility
        /// <summary>
        /// Gets the queryable.
        /// </summary>
        private IQueryable<DiscussionResponse> GetQueryable()
        {
            return _context.DiscussionResponses
                .Where(s => s.Status != "Removed");
        }

        /// <summary>
        /// Gets the queryable by discussion.
        /// </summary>
        /// 
		/// <param name="discussionId">The discussion id.</param>
        private IQueryable<DiscussionResponse> GetQueryableByDiscussion(long discussionId)
        {
            return GetQueryable()
                .Where(s => s.Status != "Removed")
                .Where(p => p.DiscussionId == discussionId);
        }
        #endregion
    }
}
