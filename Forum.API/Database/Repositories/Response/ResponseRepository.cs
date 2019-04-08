using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using Emsa.Mared.Discussions.API.Database.Repositories.Users;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Database.Repositories.Responses
{
    /// <inheritdoc />
    public class ResponseRepository : IResponseRepository
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        private readonly DiscussionContext _context;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="IResponseRepository"/> class.
        /// </summary>
        /// 
        /// <param name="context">The context.</param>
        public ResponseRepository(DiscussionContext context)
        {
            _context = context;
        }
        #endregion

        #region [Methods] IRepository
        /// <inheritdoc />
        public async Task<Response> Create(Response responseToCreate)
        {
            if (string.IsNullOrWhiteSpace(responseToCreate.Comment))
                throw new ModelException(responseToCreate.InvalidFieldMessage(p => p.Comment));

            var user = await _context.User
                .FirstOrDefaultAsync(x => x.Id == responseToCreate.UserId);
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
        public async Task<List<Response>> GetAll()
        {
            var responses = GetQueryable();

            return await responses.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<Response>> GetByDiscussion(long discussionId)
        {
            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == discussionId);
            if (discussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);

            var responses = GetQueryableByDiscussion(discussionId);

            return await responses.ToListAsync();
            //return await PagedList<Response>.CreateAsync(responses, parameters.PageNumber, parameters.PageSize);
        }

        /// <inheritdoc />
        public async Task<List<Response>> GetByDiscussion(long discussionId, ResponseParameters parameters)
        {
            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == discussionId);
            if (discussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);

            var responses = GetQueryableByDiscussion(discussionId);

            //return await responses.ToListAsync();
            return await PagedList<Response>.CreateAsync(responses, parameters.PageNumber, parameters.PageSize);
        }

        /// <inheritdoc />
        public async Task<Response> Get(long id)
        {
            var response = await _context.Responses
                .Where(s => s.Status != "Removed")
                .Include(d => d.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (response == null)
            {
                throw new ModelException(Response.DoesNotExist, true);
            }

            return response;
        }

        /// <inheritdoc />
        public async Task<Response> Update(Response updateResponse)
        {
            var databaseResponse = await _context.Responses.FindAsync(updateResponse.Id);
            if (databaseResponse == null)
                throw new ModelException(Response.DoesNotExist, true);

            if (string.IsNullOrWhiteSpace(updateResponse.Comment))
                throw new ModelException(updateResponse.InvalidFieldMessage(p => p.Comment));

            var response = await _context.Responses
                .FirstOrDefaultAsync(x => x.Id == updateResponse.Id);

            response.Comment = updateResponse.Comment;
            response.Status = "Updated";
            response.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return response;
        }

        /// <inheritdoc />
        public async Task Delete(long id)
        {
            var databaseResponse = await _context.Responses.FindAsync(id);
            if (databaseResponse == null)
                throw new ModelException(Response.DoesNotExist, true);

            var response = await _context.Responses.FindAsync(id);

            response.Status = "Removed";

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<PagedList<Response>> GetAll(ResponseParameters parameters)
        {
            var responses = GetQueryable();

            return await PagedList<Response>.CreateAsync(responses, parameters.PageNumber, parameters.PageSize);
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
        private IQueryable<Response> GetQueryable()
        {
            return _context.Responses
                .Where(s => s.Status != "Removed");
        }

        /// <summary>
        /// Gets the queryable by discussion.
        /// </summary>
        /// 
		/// <param name="discussionId">The discussion id.</param>
        private IQueryable<Response> GetQueryableByDiscussion(long discussionId)
        {
            return GetQueryable()
                .Where(s => s.Status != "Removed")
                .Where(p => p.DiscussionId == discussionId);
        }
        #endregion
    }
}
