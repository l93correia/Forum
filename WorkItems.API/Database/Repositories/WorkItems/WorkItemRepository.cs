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
using Emsa.Mared.Common.Utility;
using Emsa.Mared.WorkItems.API.Database;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants;
using Microsoft.EntityFrameworkCore;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems
{
    /// <inheritdoc />
    public class WorkItemRepository : IWorkItemRepository
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        private readonly WorkItemContext _context;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemRepository"/> class.
        /// </summary>
        /// 
        /// <param name="context">The context.</param>
        public WorkItemRepository(WorkItemContext context)
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
            if (workItemToCreate.CreationDate != null)
                throw new ModelException(workItemToCreate.InvalidFieldMessage(p => p.CreationDate));
            if (workItemToCreate.Status == Status.Default)
                throw new ModelException(workItemToCreate.InvalidFieldMessage(p => p.Status));
            if (workItemToCreate.WorkItemType == WorkItemType.Default)
                throw new ModelException(workItemToCreate.InvalidFieldMessage(p => p.WorkItemType));

            workItemToCreate.UserId = membership.UserId;

            var participants = new List<WorkItemParticipant>();
            var participant = new WorkItemParticipant
            {
                EntityId = workItemToCreate.UserId,
                EntityType = EntityType.User
            };

            participants.Add(participant);

            workItemToCreate.StartDate = DateTime.Now;
            workItemToCreate.Status = Status.Created;
            workItemToCreate.WorkItemParticipants = participants;

            await _context.WorkItems.AddAsync(workItemToCreate);
            await _context.SaveChangesAsync();

            return workItemToCreate;
        }

        /// <inheritdoc />
        public async Task<WorkItem> GetAsync(long id, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var workItem = await GetBasicQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (workItem == null)
                throw new ModelException(WorkItem.DoesNotExist, missingResource: true);
			if (workItem.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			return workItem;
        }

        /// <inheritdoc />
        public async Task<WorkItem> UpdateAsync(WorkItem updateWorkItem, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var databaseworkItem = await _context.WorkItems
                .FirstOrDefaultAsync(x => x.Id == updateWorkItem.Id);

			if (databaseworkItem == null)
				throw new ModelException(WorkItem.DoesNotExist, missingResource: true);
			if (databaseworkItem.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			if (string.IsNullOrWhiteSpace(updateWorkItem.Title))
                throw new ModelException(updateWorkItem.InvalidFieldMessage(p => p.Title));
            if (string.IsNullOrWhiteSpace(updateWorkItem.Body))
                throw new ModelException(updateWorkItem.InvalidFieldMessage(p => p.Body));
            if (string.IsNullOrWhiteSpace(updateWorkItem.Summary))
                throw new ModelException(updateWorkItem.InvalidFieldMessage(p => p.Summary));
            if (updateWorkItem.CreationDate != null)
                throw new ModelException(updateWorkItem.InvalidFieldMessage(p => p.CreationDate));
            if (updateWorkItem.Status == Status.Default)
                throw new ModelException(updateWorkItem.InvalidFieldMessage(p => p.Status));
            if (updateWorkItem.WorkItemType == WorkItemType.Default)
                throw new ModelException(updateWorkItem.InvalidFieldMessage(p => p.WorkItemType));

            databaseworkItem.Title = updateWorkItem.Title;
            databaseworkItem.Body = updateWorkItem.Body;
            databaseworkItem.UpdatedDate = DateTime.Now;
            databaseworkItem.Status = Status.Updated;

            await _context.SaveChangesAsync();

            return databaseworkItem;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(long id, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var workItem = await GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);

			if (workItem == null)
				throw new ModelException(WorkItem.DoesNotExist, missingResource: true);
			if (workItem.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			workItem.Status = Status.Removed;

            await _context.SaveChangesAsync();
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

			var workItems = GetBasicQueryable();
			var count = await workItems.CountAsync();

			return await PagedList<WorkItem>.CreateAsync(workItems, parameters.PageNumber, parameters.PageSize, count);
        }

        /// <inheritdoc />
        public Task<bool> ExistsAsync(long entityId, UserMembership membership = null)
        {
            throw new NotImplementedException();
        }
		#endregion

		#region [Methods] Utility
		/// <summary>
		/// Gets the queryable.
		/// </summary>
		private IQueryable<WorkItem> GetQueryable()
		{
			return _context.WorkItems
				.Where(s => s.Status != Status.Removed);
		}

		/// <summary>
		/// Gets the basic queryable.
		/// </summary>
		private IQueryable<WorkItem> GetBasicQueryable(UserMembership membership = null)
		{
			var queryable = GetQueryable();

			if (membership != null)
			{
				queryable = queryable
					.Where(d => d.WorkItemParticipants.Any(p => (membership.UserId == p.EntityId && p.EntityType == EntityType.User)
					|| (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group)
					|| (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)));
			}

			return queryable;
		}
		#endregion
	}
}