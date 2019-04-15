using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using Emsa.Mared.Common.Database.Utility;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Pagination;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Common.Utility;
using Emsa.Mared.Discussions.API.Database.Repositories.Discussions;
using Emsa.Mared.Discussions.API.Database.Repositories.Participants;
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

		/// <summary>
		/// Gets or sets the discussion repository.
		/// </summary>
		private readonly IDiscussionRepository _repoDiscussion;
		#endregion

		#region [Constructors]
		/// <summary>
		/// Initializes a new instance of the <see cref="IResponseRepository"/> class.
		/// </summary>
		/// 
		/// <param name="context">The context.</param>
		/// <param name="repoDiscussion">The discussion repositoy.</param>
		public ResponseRepository(DiscussionContext context, IDiscussionRepository repoDiscussion)
		{
			_context = context;
			_repoDiscussion = repoDiscussion;
		}
		#endregion

		#region [Methods] IRepository
		/// <inheritdoc />
		public async Task<Response> CreateAsync(Response responseToCreate, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var discussion = await _repoDiscussion.GetAsync(responseToCreate.DiscussionId, membership);

			if (discussion == null)
				throw new ModelException(Discussion.DoesNotExist, true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			if (string.IsNullOrWhiteSpace(responseToCreate.Comment))
				throw new ModelException(responseToCreate.InvalidFieldMessage(p => p.Comment));

			responseToCreate.CreatedDate = DateTime.Now;
			responseToCreate.Status = "Created";
			responseToCreate.UserId = membership.UserId;

			await _context.AddAsync(responseToCreate);
			await _context.SaveChangesAsync();

			return responseToCreate;
		}

		/// <inheritdoc />
		public async Task<Response> GetAsync(long id, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var response = await GetCompleteQueryable()
				.FirstOrDefaultAsync(x => x.Id == id);

			if (response == null)
				throw new ModelException(Response.DoesNotExist, missingResource: true);

			var discussion = await _context.Discussions
				.FirstOrDefaultAsync(x => x.Id == response.DiscussionId);

			if (discussion == null)
				throw new ModelException(Discussion.DoesNotExist, missingResource: true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			return response;
		}

		/// <inheritdoc />
		public async Task<Response> UpdateAsync(Response updateResponse, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var response = await GetBasicQueryable()
				.FirstOrDefaultAsync(x => x.Id == updateResponse.Id);

			if (response == null)
				throw new ModelException(Response.DoesNotExist, missingResource: true);

			var discussion = await _context.Discussions
				.FirstOrDefaultAsync(x => x.Id == response.DiscussionId);

			if (discussion == null)
				throw new ModelException(Discussion.DoesNotExist, missingResource: true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			if (string.IsNullOrWhiteSpace(updateResponse.Comment))
				throw new ModelException(updateResponse.InvalidFieldMessage(p => p.Comment));

			response.Comment = updateResponse.Comment;
			response.Status = "Updated";
			response.UpdatedDate = DateTime.Now;

			await _context.SaveChangesAsync();

			return response;
		}

		/// <inheritdoc />
		public async Task DeleteAsync(long id, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var response = await GetBasicQueryable()
				.FirstOrDefaultAsync(x => x.Id == id);

			if (response == null)
				throw new ModelException(Response.DoesNotExist, missingResource: true);

			var discussion = await _repoDiscussion.GetAsync(response.DiscussionId, membership);

			if (discussion == null)
				throw new ModelException(Discussion.DoesNotExist, missingResource: true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			response.Status = "Removed";
			response.UpdatedDate = DateTime.Now;

			await _context.SaveChangesAsync();
		}

		/// <inheritdoc />
		public async Task<PagedList<Response>> GetAllAsync(ResponseParameters parameters, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			if (parameters == null)
			{
				parameters = new ResponseParameters();
			}

			var responses = GetCompleteQueryable();
			var count = await GetBasicQueryable().CountAsync();

			return await PagedList<Response>.CreateAsync(responses, parameters.PageNumber, parameters.PageSize, count);
		}

		/// <inheritdoc />
		public async Task<bool> ExistsAsync(long entityId, UserMembership membership = null)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region [Methods] IResponseRepository
		/// <inheritdoc />
		public async Task<List<Response>> GetByDiscussion(long discussionId, ResponseParameters parameters, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
            if (parameters == null)
            {
                parameters = new ResponseParameters();
            }

            var discussion = await _repoDiscussion.GetAsync(discussionId, membership);

			if (discussion == null)
				throw new ModelException(Discussion.DoesNotExist, missingResource: true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			var responses = GetCompleteQueryable()
				.Where(p => p.DiscussionId == discussionId);
			var count = await GetBasicQueryable()
				.Where(p => p.DiscussionId == discussionId).CountAsync();

			return await PagedList<Response>.CreateAsync(responses, parameters.PageNumber, parameters.PageSize, count);
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
		/// Gets the basic queryable.
		/// </summary>
		private IQueryable<Response> GetBasicQueryable(UserMembership membership = null)
		{
			var queryable = GetQueryable();

			if (membership != null)
			{
				queryable = queryable
					.Where(d => d.Discussion.Participants.Any(p => (membership.UserId == p.EntityId && p.EntityType == EntityType.User)
					|| (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group)
					|| (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)));
			}

			return queryable;
		}

		/// <summary>
		/// Gets the complete queryable.
		/// </summary>
		private IQueryable<Response> GetCompleteQueryable(UserMembership membership = null)
		{
			return GetBasicQueryable(membership)
				.Include(d => d.Discussion);
		}
		#endregion
	}
}
