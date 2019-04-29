using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Emsa.Mared.Common.Database.Utility;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Pagination;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Common.Extensions;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemParticipants;

using Microsoft.EntityFrameworkCore;

namespace Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItems
{
    /// <inheritdoc />
    public class WorkItemRepository : IWorkItemRepository
    {
		#region [Constants]
		/// <summary>
		/// The Work Item does not exist message.
		/// </summary>
		public const string DoesNotExist = "The Work Item does not exist.";

		/// <summary>
		/// Empty Work Item message.
		/// </summary>
		public const string Empty = "No Work Item founded.";
		#endregion

		#region [Properties]
		/// <summary>
		/// Gets or sets the context.
		/// </summary>
		private readonly WorkItemsContext _context;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemRepository"/> class.
        /// </summary>
        /// 
        /// <param name="context">The context.</param>
        public WorkItemRepository(WorkItemsContext context)
        {
            _context = context;
        }
        #endregion

        #region [Methods] IRepository
        /// <inheritdoc />
        public async Task<WorkItem> CreateAsync(WorkItem workItemToCreate, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

			if (string.IsNullOrWhiteSpace(workItemToCreate.Title))
                throw new ModelException(workItemToCreate.InvalidFieldMessage(p => p.Title));
            if (string.IsNullOrWhiteSpace(workItemToCreate.Body))
                throw new ModelException(workItemToCreate.InvalidFieldMessage(p => p.Body));
            if (string.IsNullOrWhiteSpace(workItemToCreate.Summary))
                throw new ModelException(workItemToCreate.InvalidFieldMessage(p => p.Summary));
            if (workItemToCreate.Type == WorkItemType.Default)
                throw new ModelException(workItemToCreate.InvalidFieldMessage(p => p.Type));

            workItemToCreate.UserId = membership.UserId;
            workItemToCreate.Status = WorkItemStatus.Created;
            workItemToCreate.WorkItemParticipants = new List<WorkItemParticipant>
            {
                new WorkItemParticipant
                {
                    EntityId = workItemToCreate.UserId,
                    EntityType = EntityType.User
                }
            };

            await _context.WorkItems.AddAsync(workItemToCreate);
            await _context.SaveChangesAsync();

            return workItemToCreate;
        }

		/// <inheritdoc />
		public async Task<WorkItem> UpdateAsync(WorkItem updateWorkItem, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

			if (!await this.ExistsAsync(updateWorkItem.Id))
				throw new ModelException(WorkItem.DoesNotExist, missingResource: true);
			if (!await this.IsCreator(updateWorkItem.Id, membership))
				throw new ModelException(string.Empty, unauthorizedResource: true);

			if (string.IsNullOrWhiteSpace(updateWorkItem.Title))
				throw new ModelException(updateWorkItem.InvalidFieldMessage(p => p.Title));
			if (string.IsNullOrWhiteSpace(updateWorkItem.Body))
				throw new ModelException(updateWorkItem.InvalidFieldMessage(p => p.Body));
			if (string.IsNullOrWhiteSpace(updateWorkItem.Summary))
				throw new ModelException(updateWorkItem.InvalidFieldMessage(p => p.Summary));
			if (updateWorkItem.Type == WorkItemType.Default)
				throw new ModelException(updateWorkItem.InvalidFieldMessage(p => p.Type));

			var databaseworkItem = await _context.WorkItems
				.FirstOrDefaultAsync(x => x.Id == updateWorkItem.Id);

			if(databaseworkItem.Type != updateWorkItem.Type)
			{
				throw new ModelException(WorkItem.CannotChangeType);
			}

			databaseworkItem.Title = updateWorkItem.Title;
			databaseworkItem.Summary = updateWorkItem.Summary;
			databaseworkItem.Body = updateWorkItem.Body;

			switch (databaseworkItem.Status)
			{
				case WorkItemStatus.Created when updateWorkItem.Status == WorkItemStatus.Default:
				case WorkItemStatus.Created when updateWorkItem.Status == WorkItemStatus.Created:
					databaseworkItem.Status = WorkItemStatus.Updated;
					break;

				case WorkItemStatus.Updated when updateWorkItem.Status == WorkItemStatus.Default:
				case WorkItemStatus.Updated when updateWorkItem.Status == WorkItemStatus.Updated:
					databaseworkItem.Status = WorkItemStatus.Updated;
					break;

				case WorkItemStatus.Closed when updateWorkItem.Status == WorkItemStatus.Default:
				case WorkItemStatus.Closed when updateWorkItem.Status == WorkItemStatus.Closed:
					databaseworkItem.Status = WorkItemStatus.Closed;
					break;

				case WorkItemStatus.Removed when updateWorkItem.Status == WorkItemStatus.Default:
				case WorkItemStatus.Removed when updateWorkItem.Status == WorkItemStatus.Removed:
					databaseworkItem.Status = WorkItemStatus.Removed;
					break;

				case WorkItemStatus.Closed when updateWorkItem.Status == WorkItemStatus.Created:
				case WorkItemStatus.Closed when updateWorkItem.Status == WorkItemStatus.Updated:
					databaseworkItem.Status = WorkItemStatus.Updated;
					break;

				case WorkItemStatus.Created when updateWorkItem.Status == WorkItemStatus.Closed:
				case WorkItemStatus.Updated when updateWorkItem.Status == WorkItemStatus.Closed:
					databaseworkItem.Status = WorkItemStatus.Closed;
					break;

				default:
					throw new ModelException(string.Format(WorkItem.InvalidStatusTransition, 
						databaseworkItem.Status.ToString(), updateWorkItem.Status.ToString()));
			}

			if(databaseworkItem.Status == WorkItemStatus.Closed)
			{
				databaseworkItem.ClosedAt = DateTime.UtcNow;
			}

            await _context.SaveChangesAsync();

            return databaseworkItem;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(long id, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.ExistsAsync(id))
                throw new ModelException(WorkItem.DoesNotExist, missingResource: true);
            if (!await this.IsCreator(id, membership))
                throw new ModelException(string.Empty, unauthorizedResource: true);

            var databaseworkItem = await _context.WorkItems
                .FirstOrDefaultAsync(x => x.Id == id);

            databaseworkItem.Status = WorkItemStatus.Removed;

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<WorkItem> GetAsync(long id, UserMembership membership = null)
        {
            if (membership == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.ExistsAsync(id))
                throw new ModelException(WorkItem.DoesNotExist, missingResource: true);
            if (!await this.IsParticipant(id, membership))
                throw new ModelException(string.Empty, unauthorizedResource: true);

            var workItem = await this.GetCompleteQueryable(membership)
                .FirstOrDefaultAsync(x => x.Id == id);

            return workItem;
        }

        /// <inheritdoc />
        public async Task<PagedList<WorkItem>> GetAllAsync(WorkItemParameters parameters, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

			if (parameters == null)
            {
                parameters = new WorkItemParameters();
            }

            var workItems = this.GetCompleteQueryable(membership);
            var count = this.GetParticipantQueryable(membership);

            if (parameters.WorkItemType != null)
            {
                workItems = this.GetCompleteQueryable(membership)
                    .Where(x => x.Type == parameters.WorkItemType);
                count = this.GetParticipantQueryable(membership)
                    .Where(x => x.Type == parameters.WorkItemType);
            }

            return await PagedList<WorkItem>.CreateAsync(workItems, parameters.PageNumber, parameters.PageSize, await count.CountAsync());
        }
        
        /// <inheritdoc />
        public async Task<bool> ExistsAsync(long id, UserMembership membership = null)
        {
            return await this.GetQueryable().AnyAsync(x => x.Id == id);
        }
        #endregion

        #region [Methods] IWorkItemRepository
        /// <inheritdoc />
        public async Task<bool> IsType(long id, WorkItemType type)
        {
            return await this.GetQueryable()
                .Where(x => x.Type == type)
                .AnyAsync(x => x.Id == id);
        }

        /// <inheritdoc />
        public async Task<bool> IsCreator(long id, UserMembership membership)
        {
            return await GetQueryable()
                .AnyAsync(x => x.Id == id && x.UserId == membership.UserId);
        }

        /// <inheritdoc />
        public async Task<bool> IsParticipant(long id, UserMembership membership)
        {
            return await GetQueryable()
                .AnyAsync(x => x.Id == id && x.WorkItemParticipants.Any(p => 
                       (membership.UserId == p.EntityId && p.EntityType == EntityType.User)
                    || (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group)
                    || (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)));
        }
        #endregion

        #region [Methods] Utility
        /// <summary>
        /// Gets the basic queryable, without removed work items.
        /// </summary>
        private IQueryable<WorkItem> GetQueryable()
		{
			return _context.WorkItems
				.Where(s => s.Status != WorkItemStatus.Removed);
		}

        /// <summary>
        /// Gets the basic queryable and filters it by user.
        /// </summary>
        private IQueryable<WorkItem> GetParticipantQueryable(UserMembership membership)
		{
			var queryable = this.GetQueryable();

			if (membership != null)
			{
				queryable = queryable
					.Where(d => d.WorkItemParticipants.Any(p => (membership.UserId == p.EntityId && p.EntityType == EntityType.User)
					|| (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group)
					|| (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)));
			}

			return queryable;
        }

        /// <summary>
        /// Gets the participant queryable and includes sub-entities.
        /// </summary>
        private IQueryable<WorkItem> GetCompleteQueryable(UserMembership membership)
        {
            var queryable = this.GetParticipantQueryable(membership)
                .Include(x => x.WorkItemAttachments)
                .Include(x => x.WorkItemComments)
                .Include(x => x.RelatedFromWorkItems)
                .Include(x => x.RelatedToWorkItems)
                .Include(x => x.WorkItemParticipants);

            return queryable;
        }
        #endregion
    }
}