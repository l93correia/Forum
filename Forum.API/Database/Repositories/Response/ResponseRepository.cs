using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
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
        public async Task<Response> Create(Response responseToCreate, UserMembership membership = null)
        {
            var discussion = await _context.Discussions
                .Where(d => d.Participants.Any(p => (membership.UserId == p.EntityId && p.EntityType == EntityType.User)
                || (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group)
                || (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)))
                .FirstOrDefaultAsync(x => x.Id == responseToCreate.DiscussionId);

            if (string.IsNullOrWhiteSpace(responseToCreate.Comment))
                throw new ModelException(responseToCreate.InvalidFieldMessage(p => p.Comment));

            if (discussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);

            responseToCreate.CreatedDate = DateTime.Now;
            responseToCreate.Status = "Created";
            responseToCreate.UserId = membership.UserId;

            await _context.AddAsync(responseToCreate);
            await _context.SaveChangesAsync();

            return responseToCreate;
        }

        /// <inheritdoc />
        public async Task<List<Response>> GetAll(string name = null, UserMembership membership = null)
        {
            var responses = GetQueryable()
                .Include(d => d.Discussion)
                .Where(d => d.Discussion.Participants.Any(p => (membership.UserId == p.EntityId && p.EntityType == EntityType.User)
                || (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group)
                || (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)));

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
        public async Task<List<Response>> GetByDiscussion(long discussionId, ResponseParameters parameters, UserMembership membership = null)
        {
            var discussion = await _context.Discussions
                .Where(d => d.Participants.Any(p => (membership.UserId == p.EntityId && p.EntityType == EntityType.User)
                || (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group)
                || (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)))
                .FirstOrDefaultAsync(x => x.Id == discussionId);

            if (discussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);

            var responses = GetQueryableByDiscussion(discussionId);

            //return await responses.ToListAsync();
            return await PagedList<Response>.CreateAsync(responses, parameters.PageNumber, parameters.PageSize);
        }

        /// <inheritdoc />
        public async Task<Response> Get(long id, UserMembership membership = null)
        {
            var response = GetQueryable()
                .Include(d => d.Discussion)
                .Where(d => d.Discussion.Participants.Any(p => (membership.UserId == p.EntityId && p.EntityType == EntityType.User)
                || (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group)
                || (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)))
                .FirstOrDefaultAsync(x => x.Id == id);

            return await response;
        }

        /// <inheritdoc />
        public async Task<Response> Update(Response updateResponse, UserMembership membership = null)
        {
            var response = await _context.Responses
                .Where(d => d.UserId == membership.UserId)
                .FirstOrDefaultAsync(x => x.Id == updateResponse.Id);

            if (response == null)
                throw new ModelException(Response.DoesNotExist, true);

            if (string.IsNullOrWhiteSpace(updateResponse.Comment))
                throw new ModelException(updateResponse.InvalidFieldMessage(p => p.Comment));

            response.Comment = updateResponse.Comment;
            response.Status = "Updated";
            response.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return response;
        }

        /// <inheritdoc />
        public async Task Delete(long id, UserMembership membership = null)
        {
            var response = await GetQueryable()
                .Where(d => d.UserId == membership.UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (response == null)
                throw new ModelException(Response.DoesNotExist, true);

            response.Status = "Removed";
            response.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<PagedList<Response>> GetAll(ResponseParameters parameters, UserMembership membership = null)
        {
            if (parameters == null)
            {
                parameters = new ResponseParameters();
            }

            var responses = GetQueryable()
                .Include(d => d.Discussion)
                .Where(d => d.Discussion.Participants.Any(p => (membership.UserId == p.EntityId && p.EntityType == EntityType.User)
                || (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group)
                || (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)));

            return await PagedList<Response>.CreateAsync(responses, parameters.PageNumber, parameters.PageSize);
        }

        /// <inheritdoc />
        public async Task<bool> Exists(long entityId, UserMembership membership = null)
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
                .Where(p => p.DiscussionId == discussionId);
        }
        #endregion
    }
}
