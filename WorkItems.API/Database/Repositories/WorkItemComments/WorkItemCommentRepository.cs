using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using Emsa.Mared.Common.Database.Utility;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Pagination;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Common.Utility;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemComments
{
	/// <inheritdoc />
	public class WorkItemCommentRepository : IWorkItemCommentRepository
    {
		#region [Properties]
		/// <summary>
		/// Gets or sets the context.
		/// </summary>
		private readonly WorkItemContext _context;

		/// <summary>
		/// Gets or sets the discussion repository.
		/// </summary>
		private readonly IWorkItemRepository _repoDiscussion;
		#endregion

		#region [Constructors]
		/// <summary>
		/// Initializes a new instance of the <see cref="IWorkItemCommentRepository"/> class.
		/// </summary>
		/// 
		/// <param name="context">The context.</param>
		/// <param name="repoDiscussion">The discussion repositoy.</param>
		public WorkItemCommentRepository(WorkItemContext context, IWorkItemRepository repoDiscussion)
		{
			_context = context;
			_repoDiscussion = repoDiscussion;
		}
		#endregion

		#region [Methods] IRepository
		/// <inheritdoc />
		public async Task<WorkItemComment> CreateAsync(WorkItemComment responseToCreate, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var discussion = await _repoDiscussion.GetAsync(responseToCreate.WorkItemId, membership);

			if (discussion == null)
				throw new ModelException(WorkItem.DoesNotExist, true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			if (string.IsNullOrWhiteSpace(responseToCreate.Comment))
				throw new ModelException(responseToCreate.InvalidFieldMessage(p => p.Comment));

			responseToCreate.CreatedDate = DateTime.Now;
			responseToCreate.Status = Status.Created;
			responseToCreate.UserId = membership.UserId;

			await _context.AddAsync(responseToCreate);
			await _context.SaveChangesAsync();

			return responseToCreate;
		}

		/// <inheritdoc />
		public async Task<WorkItemComment> GetAsync(long id, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var response = await GetCompleteQueryable()
				.FirstOrDefaultAsync(x => x.Id == id);

			if (response == null)
				throw new ModelException(WorkItemComment.DoesNotExist, missingResource: true);

			var discussion = await _context.WorkItems
				.FirstOrDefaultAsync(x => x.Id == response.WorkItemId);

			if (discussion == null)
				throw new ModelException(WorkItem.DoesNotExist, missingResource: true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			return response;
		}

		/// <inheritdoc />
		public async Task<WorkItemComment> UpdateAsync(WorkItemComment updateResponse, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var response = await GetBasicQueryable()
				.FirstOrDefaultAsync(x => x.Id == updateResponse.Id);

			if (response == null)
				throw new ModelException(WorkItemComment.DoesNotExist, missingResource: true);

			var discussion = await _context.WorkItems
				.FirstOrDefaultAsync(x => x.Id == response.WorkItemId);

			if (discussion == null)
				throw new ModelException(WorkItem.DoesNotExist, missingResource: true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			if (string.IsNullOrWhiteSpace(updateResponse.Comment))
				throw new ModelException(updateResponse.InvalidFieldMessage(p => p.Comment));

			response.Comment = updateResponse.Comment;
			response.Status = Status.Updated;
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
				throw new ModelException(WorkItemComment.DoesNotExist, missingResource: true);

			var discussion = await _repoDiscussion.GetAsync(response.WorkItemId, membership);

			if (discussion == null)
				throw new ModelException(WorkItem.DoesNotExist, missingResource: true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			response.Status = Status.Removed;
			response.UpdatedDate = DateTime.Now;

			await _context.SaveChangesAsync();
		}

		/// <inheritdoc />
		public async Task<PagedList<WorkItemComment>> GetAllAsync(WorkItemCommentParameters parameters, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			if (parameters == null)
			{
				parameters = new WorkItemCommentParameters();
			}

			var responses = GetCompleteQueryable();
			var count = await GetBasicQueryable().CountAsync();

			return await PagedList<WorkItemComment>.CreateAsync(responses, parameters.PageNumber, parameters.PageSize, count);
		}

		/// <inheritdoc />
		public async Task<bool> ExistsAsync(long entityId, UserMembership membership = null)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region [Methods] IResponseRepository
		/// <inheritdoc />
		public async Task<List<WorkItemComment>> GetByDiscussion(long discussionId, WorkItemCommentParameters parameters, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
            if (parameters == null)
            {
                parameters = new WorkItemCommentParameters();
            }

            var discussion = await _repoDiscussion.GetAsync(discussionId, membership);

			if (discussion == null)
				throw new ModelException(WorkItem.DoesNotExist, missingResource: true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			var responses = GetCompleteQueryable()
				.Where(p => p.WorkItemId == discussionId);
			var count = await GetBasicQueryable()
				.Where(p => p.WorkItemId == discussionId).CountAsync();

			return await PagedList<WorkItemComment>.CreateAsync(responses, parameters.PageNumber, parameters.PageSize, count);
		}
		#endregion
		
		#region [Methods] Utility
		/// <summary>
		/// Gets the queryable.
		/// </summary>
		private IQueryable<WorkItemComment> GetQueryable()
		{
			return _context.Responses
				.Where(s => s.Status != Status.Removed);
		}

		/// <summary>
		/// Gets the basic queryable.
		/// </summary>
		private IQueryable<WorkItemComment> GetBasicQueryable(UserMembership membership = null)
		{
			var queryable = GetQueryable();

			if (membership != null)
			{
				queryable = queryable
					.Where(d => d.WorkItem.WorkItemParticipants.Any(p => (membership.UserId == p.EntityId && p.EntityType == EntityType.User)
					|| (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group)
					|| (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)));
			}

			return queryable;
		}

		/// <summary>
		/// Gets the complete queryable.
		/// </summary>
		private IQueryable<WorkItemComment> GetCompleteQueryable(UserMembership membership = null)
		{
			return GetBasicQueryable(membership)
				.Include(d => d.WorkItem);
		}
		#endregion
	}
}
