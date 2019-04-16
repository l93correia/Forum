using Emsa.Mared.Common.Database.Utility;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Pagination;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Common.Utility;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using Microsoft.EntityFrameworkCore;
using System;
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
		private readonly WorkItemContext context;

		/// <summary>
		/// Gets or sets the work item repository.
		/// </summary>
		private readonly IWorkItemRepository repoWorkItem;
		#endregion

		#region [Constructors]
		/// <summary>
		/// Initializes a new instance of the <see cref="IWorkItemCommentRepository"/> class.
		/// </summary>
		/// 
		/// <param name="context">The context.</param>
		/// <param name="repoWorkItem">The work item repositoy.</param>
		public WorkItemCommentRepository(WorkItemContext context, IWorkItemRepository repoWorkItem)
		{
			this.context = context;
			this.repoWorkItem = repoWorkItem;
		}
		#endregion

		#region [Methods] IRepository
		/// <inheritdoc />
		public async Task<WorkItemComment> CreateAsync(WorkItemComment commentToCreate, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.repoWorkItem.ExistsAsync(commentToCreate.WorkItemId))
                throw new ModelException(WorkItem.DoesNotExist, true);
			if (!await this.repoWorkItem.IsParticipant(commentToCreate.WorkItemId, membership))
				throw new ModelException(string.Empty, unauthorizedResource: true);

			if (string.IsNullOrWhiteSpace(commentToCreate.Comment))
				throw new ModelException(commentToCreate.InvalidFieldMessage(p => p.Comment));

            // If we need to do any extra validation according to the work item type
            // then just add the necessary logic beneath.

            //var workItem = await _repoWorkItem.GetAsync(commentToCreate.WorkItemId, membership);

            //switch (workItem.WorkItemType)
            //{
            //    case WorkItemType.Default:
            //        break;
            //    case WorkItemType.Event:
            //        break;
            //    case WorkItemType.Document:
            //        break;
            //    case WorkItemType.Discussion:
            //        break;
            //    case WorkItemType.WorkingSheet:
            //        break;
            //    case WorkItemType.Recommendation:
            //        break;
            //    case WorkItemType.News:
            //        break;
            //    default:
            //        break;
            //}

            commentToCreate.CreatedDate = DateTime.Now;
			commentToCreate.Status = Status.Created;
			commentToCreate.UserId = membership.UserId;

			await this.context.AddAsync(commentToCreate);
			await this.context.SaveChangesAsync();

			return commentToCreate;
		}

		/// <inheritdoc />
		public async Task<WorkItemComment> UpdateAsync(WorkItemComment updateComment, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

			if (!await this.ExistsAsync(updateComment.Id))
				throw new ModelException(WorkItemComment.DoesNotExist, missingResource: true);
			if (!await this.IsCreator(updateComment.Id, membership))
				throw new ModelException(string.Empty, unauthorizedResource: true);

			if (string.IsNullOrWhiteSpace(updateComment.Comment))
				throw new ModelException(updateComment.InvalidFieldMessage(p => p.Comment));

            var workItemComment = await this.context.WorkItemComments
                .FirstOrDefaultAsync(x => x.Id == updateComment.Id);

            workItemComment.Comment = updateComment.Comment;
            workItemComment.Status = workItemComment.Status != Status.Default ? updateComment.Status : workItemComment.Status;
            workItemComment.UpdatedDate = DateTime.Now;

			await this.context.SaveChangesAsync();

			return workItemComment;
		}

		/// <inheritdoc />
		public async Task DeleteAsync(long id, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.ExistsAsync(id))
                throw new ModelException(WorkItemComment.DoesNotExist, missingResource: true);
            if (!await this.IsCreator(id, membership))
                throw new ModelException(string.Empty, unauthorizedResource: true);

            var workItemComment = await this.context.WorkItemComments
                .FirstOrDefaultAsync(x => x.Id == id);

            workItemComment.Status = Status.Removed;
			workItemComment.UpdatedDate = DateTime.Now;

			await this.context.SaveChangesAsync();
		}

        /// <inheritdoc />
        public async Task<WorkItemComment> GetAsync(long id, UserMembership membership = null)
        {
            if (membership == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.ExistsAsync(id))
                throw new ModelException(WorkItemComment.DoesNotExist, missingResource: true);

            var comment = await this.GetCompleteQueryable(membership: membership)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (!await this.repoWorkItem.IsParticipant(comment.WorkItemId, membership))
                throw new ModelException(string.Empty, unauthorizedResource: true);

            return comment;
        }

        /// <inheritdoc />
        public async Task<PagedList<WorkItemComment>> GetAllAsync(WorkItemCommentParameters parameters, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

			if (parameters == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(parameters)));

			var workItemComments = this.GetCompleteQueryable(parameters.workItemId, membership);
			var count = await this.GetParticipantQueryable(parameters.workItemId, membership).CountAsync();

			return await PagedList<WorkItemComment>.CreateAsync(workItemComments, parameters.PageNumber, parameters.PageSize, count);
		}

		/// <inheritdoc />
		public async Task<bool> ExistsAsync(long id, UserMembership membership = null)
		{
            return await this.GetQueryable().AnyAsync(x => x.Id == id);
        }
        #endregion

        #region [Methods] IWorkItemCommentRepository
        /// <inheritdoc />
        public async Task<bool> IsCreator(long id, UserMembership membership)
        {
            return await this.GetQueryable()
                .AnyAsync(x => x.Id == id && x.UserId == membership.UserId);
        }
        #endregion

        #region [Methods] Utility
        /// <summary>
        /// Gets the basic queryable, without removed work item comments.
        /// </summary>
        private IQueryable<WorkItemComment> GetQueryable()
		{
			return this.context.WorkItemComments
				.Where(s => s.Status != Status.Removed);
		}

        /// <summary>
        /// Gets the basic queryable and filters it by user.
        /// </summary>
        private IQueryable<WorkItemComment> GetParticipantQueryable(long? workItemId = null, UserMembership membership = null)
		{
			var queryable = this.GetQueryable();

            if (workItemId != null)
            {
                queryable = queryable
                    .Where(d => d.WorkItemId == workItemId);
            }

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
        /// Gets the participant queryable and includes sub-entities.
        /// </summary>
        private IQueryable<WorkItemComment> GetCompleteQueryable(long? workItemId = null, UserMembership membership = null)
        {
            var queryable = this.GetParticipantQueryable(workItemId, membership)
                .Include(x => x.WorkItem);

            return queryable;
		}
		#endregion
	}
}
