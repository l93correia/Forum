using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using Emsa.Mared.Common.Database.Utility;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Pagination;
using Emsa.Mared.Common.Security;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using Microsoft.EntityFrameworkCore;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemAttachments
{
    /// <inheritdoc />
    public class WorkItemAttachmentRepository : IWorkItemAttachmentRepository
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        private readonly WorkItemContext _context;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemAttachmentRepository"/> class.
        /// </summary>
        /// 
        /// <param name="context">The context.</param>
        public WorkItemAttachmentRepository(WorkItemContext context)
        {
            _context = context;
        }
        #endregion

        #region [Methods] IRepository
        /// <inheritdoc />
        public async Task<WorkItemAttachment> CreateAsync(WorkItemAttachment attachmentToCreate, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == attachmentToCreate.WorkItemId);
            if (discussion == null)
                throw new ModelException(WorkItem.DoesNotExist, true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			await _context.Attachments.AddAsync(attachmentToCreate);
            await _context.SaveChangesAsync();

            return attachmentToCreate;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(long attachmentId, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var attachment = await GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == attachmentId);

			if (attachment == null)
				throw new ModelException(WorkItemAttachment.DoesNotExist, missingResource: true);

			var discussion = await _context.Discussions
				.FirstOrDefaultAsync(x => x.Id == attachment.WorkItemId);

            if (discussion.UserId != membership.UserId)
                throw new ModelException(string.Empty, unauthorizedResource: true);

            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task<bool> ExistsAsync(long entityId, UserMembership membership = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<WorkItemAttachment> GetAsync(long attachmentId, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var attachment = await GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == attachmentId);

            if (attachment == null)
                throw new ModelException(WorkItemAttachment.DoesNotExist, missingResource: true);

            var discussion = await _context.Discussions
				.FirstOrDefaultAsync(x => x.Id == attachment.WorkItemId);

			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

            return attachment;
        }

        /// <inheritdoc />
        public async Task<PagedList<WorkItemAttachment>> GetAllAsync(WorkItemAttachmentParameters parameters, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			if (parameters == null)
            {
                parameters = new WorkItemAttachmentParameters();
            }
            var attachments = GetCompleteQueryable();
			var count = await GetBasicQueryable().CountAsync();

            return await PagedList<WorkItemAttachment>.CreateAsync(attachments, parameters.PageNumber, parameters.PageSize, count);

        }

        /// <inheritdoc />
        public async Task<WorkItemAttachment> UpdateAsync(WorkItemAttachment attachment, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var attachmentToUpdate = await GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == attachment.Id);

            if (attachmentToUpdate == null)
                throw new ModelException(WorkItemAttachment.DoesNotExist, missingResource: true);

            var discussion = await _context.Discussions
				.FirstOrDefaultAsync(x => x.Id == attachment.WorkItemId);

			
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			attachmentToUpdate = attachment;

            await _context.SaveChangesAsync();

            return attachmentToUpdate;
        }
		#endregion

		#region [Methods] IAttachmentRepository
		/// <inheritdoc />
		public async Task<List<WorkItemAttachment>> GetByDiscussion(long discussionId, WorkItemAttachmentParameters parameters, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
            if (parameters == null)
            {
                parameters = new WorkItemAttachmentParameters();
            }

            var discussion = await _context.Discussions
				.FirstOrDefaultAsync(x => x.Id == discussionId);

			if (discussion == null)
				throw new ModelException(WorkItem.DoesNotExist, true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			var attachments = GetQueryableByDiscussion(discussionId);
			var count = await attachments.CountAsync();

			return await PagedList<WorkItemAttachment>.CreateAsync(attachments, parameters.PageNumber, parameters.PageSize, count);
		}

		#endregion

		#region [Methods] Utility
		/// <summary>
		/// Gets the queryable.
		/// </summary>
		private IQueryable<WorkItemAttachment> GetQueryable()
        {
            return _context.Attachments;
        }

		/// <summary>
		/// Gets the basic queryable.
		/// </summary>
		private IQueryable<WorkItemAttachment> GetBasicQueryable(UserMembership membership = null)
		{
			var queryable = GetQueryable();

			if (membership != null)
			{
				queryable = queryable
					.Where(i => i.WorkItemId == i.WorkItem.Id
					&& i.WorkItem.UserId == membership.UserId);
			}

			return queryable;
		}

		/// <summary>
		/// Gets the complete queryable.
		/// </summary>
		private IQueryable<WorkItemAttachment> GetCompleteQueryable(UserMembership membership = null)
		{
			return GetBasicQueryable(membership)
				.Include(d => d.WorkItem);
		}

		/// <summary>
		/// Gets the queryable by discussion.
		/// </summary>
		/// 
		/// <param name="discussionId">The discussion id.</param>
		private IQueryable<WorkItemAttachment> GetQueryableByDiscussion(long discussionId)
        {
            return GetQueryable()
                .Where(p => p.WorkItemId == discussionId);
        }
        #endregion
    }
}
