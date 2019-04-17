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
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants;
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
        private readonly WorkItemContext context;

        /// <summary>
        /// Gets or sets the discussion repository.
        /// </summary>
        private readonly IWorkItemRepository repoWorkItem;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemAttachmentRepository"/> class.
        /// </summary>
        /// 
        /// <param name="context">The context.</param>
        public WorkItemAttachmentRepository(WorkItemContext context, IWorkItemRepository repoWorkItem)
        {
            this.context = context;
            this.repoWorkItem = repoWorkItem;
        }
        #endregion

        #region [Methods] IRepository
        /// <inheritdoc />
        public async Task<WorkItemAttachment> CreateAsync(WorkItemAttachment attachmentToCreate, UserMembership membership = null)
        {
            if (membership == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.repoWorkItem.ExistsAsync(attachmentToCreate.WorkItemId))
                throw new ModelException(WorkItem.DoesNotExist, true);
            if (!await this.repoWorkItem.IsCreator(attachmentToCreate.WorkItemId, membership))
                throw new ModelException(string.Empty, unauthorizedResource: true);

            await context.WorkItemAttachments.AddAsync(attachmentToCreate);
            await context.SaveChangesAsync();

            return attachmentToCreate;
        }

        /// <inheritdoc />
        public async Task<WorkItemAttachment> UpdateAsync(WorkItemAttachment attachment, UserMembership membership = null)
        {
            if (membership == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.ExistsAsync(attachment.Id))
                throw new ModelException(WorkItemAttachment.DoesNotExist, missingResource: true);
            if (!await this.IsCreator(attachment.WorkItemId, membership))
                throw new ModelException(string.Empty, unauthorizedResource: true);

            var attachmentToUpdate = await this.context.WorkItemAttachments
                .FirstOrDefaultAsync(x => x.Id == attachment.Id);

            attachmentToUpdate.Url = attachment.Url;
            attachmentToUpdate.ExternalId = attachment.ExternalId;

            await context.SaveChangesAsync();

            return attachmentToUpdate;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(long id, UserMembership membership = null)
        {
            if (membership == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.ExistsAsync(id))
                throw new ModelException(WorkItemAttachment.DoesNotExist, missingResource: true);
            if (!await this.IsCreator(id, membership))
                throw new ModelException(string.Empty, unauthorizedResource: true);

            var attachment = await this.context.WorkItemAttachments
                .FirstOrDefaultAsync(x => x.Id == id);

            context.WorkItemAttachments.Remove(attachment);
            await context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<WorkItemAttachment> GetAsync(long id, UserMembership membership = null)
        {
            if (membership == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.ExistsAsync(id))
                throw new ModelException(WorkItemAttachment.DoesNotExist, missingResource: true);

            var attachment = await this.GetCompleteQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (!await this.repoWorkItem.IsParticipant(attachment.WorkItemId, membership))
                throw new ModelException(string.Empty, unauthorizedResource: true);

            return attachment;
        }

        /// <inheritdoc />
        public async Task<PagedList<WorkItemAttachment>> GetAllAsync(WorkItemAttachmentParameters parameters, UserMembership membership = null)
        {
            if (membership == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (parameters == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(parameters)));

            var attachments = this.GetCompleteQueryable(parameters.workItemId, membership);
            var count = await this.GetParticipantQueryable(parameters.workItemId, membership).CountAsync();

            return await PagedList<WorkItemAttachment>.CreateAsync(attachments, parameters.PageNumber, parameters.PageSize, count);

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

        /// <inheritdoc />
        public async Task<bool> BelongsToWorkItem(long workItemId, long attachmentId)
        {
            return await this.GetCompleteQueryable()
                .Where(x => x.WorkItemId == workItemId)
                .AnyAsync(x => x.Id == attachmentId);
        }
        #endregion

        #region [Methods] Utility
        /// <summary>
        /// Gets the queryable.
        /// </summary>
        private IQueryable<WorkItemAttachment> GetQueryable()
        {
            return context.WorkItemAttachments;
        }

        /// <summary>
        /// Gets the basic queryable and filters it by user.
        /// </summary>
        private IQueryable<WorkItemAttachment> GetParticipantQueryable(long? workItemId = null, UserMembership membership = null)
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
        /// Gets the attachment queryable and includes sub-entities.
        /// </summary>
        private IQueryable<WorkItemAttachment> GetCompleteQueryable(long? workItemId = null, UserMembership membership = null)
        {
            var queryable = this.GetParticipantQueryable(workItemId, membership)
                .Include(x => x.WorkItem);

            return queryable;
        }
        #endregion
    }
}
