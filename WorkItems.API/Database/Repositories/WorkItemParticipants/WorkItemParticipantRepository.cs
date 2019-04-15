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
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using Microsoft.EntityFrameworkCore;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants
{
    /// <inheritdoc />
    public class WorkItemParticipantRepository : IWorkItemParticipantRepository
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        private readonly WorkItemContext _context;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemParticipantRepository"/> class.
        /// </summary>
        /// 
        /// <param name="context">The context.</param>
        public WorkItemParticipantRepository(WorkItemContext context)
        {
            _context = context;
        }
        #endregion

        #region [Methods] IRepository
        /// <inheritdoc />
        public async Task<WorkItemParticipant> CreateAsync(WorkItemParticipant participantToCreate, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
            if (participantToCreate.EntityType == EntityType.Default)
                throw new ModelException(participantToCreate.InvalidFieldMessage(p => p.EntityType));
            if (participantToCreate.EntityId <= 0)
                throw new ModelException(participantToCreate.InvalidFieldMessage(p => p.EntityType));

            var workItem = await _context.WorkItems
                .FirstOrDefaultAsync(x => x.Id == participantToCreate.WorkItemId);

            if (workItem == null)
                throw new ModelException(WorkItem.DoesNotExist, true);
            if (workItem.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			await _context.WorkItemParticipants.AddAsync(participantToCreate);
            await _context.SaveChangesAsync();

            return participantToCreate;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(long participantId, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var participant = await GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == participantId);

			if (participant == null)
				throw new ModelException(WorkItemParticipant.DoesNotExist, missingResource: true);

            if (participant.EntityId == membership.UserId && participant.EntityType == EntityType.User)
                throw new ModelException(string.Empty, unauthorizedResource: true);

            var workItem = await _context.WorkItems
                .FirstOrDefaultAsync(x => x.Id == participant.WorkItemId);

			if (workItem == null)
				throw new ModelException(WorkItem.DoesNotExist, missingResource: true);
			if (workItem.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			_context.WorkItemParticipants.Remove(participant);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task<bool> ExistsAsync(long entityId, UserMembership membership = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<WorkItemParticipant> GetAsync(long participantId, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var participant = await GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == participantId);

            if (participant == null)
                throw new ModelException(WorkItemParticipant.DoesNotExist, missingResource: true);

            var workItem = await _context.WorkItems
                .FirstOrDefaultAsync(x => x.Id == participant.WorkItemId);

			if (workItem == null)
				throw new ModelException(WorkItem.DoesNotExist, missingResource: true);
			if (workItem.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			return participant;
        }

        /// <inheritdoc />
        public async Task<PagedList<WorkItemParticipant>> GetAllAsync(WorkItemParticipantParameters parameters, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			if (parameters == null)
            {
                parameters = new WorkItemParticipantParameters();
            }
			var participants = GetCompleteQueryable();
			var count = await GetBasicQueryable().CountAsync();

			return await PagedList<WorkItemParticipant>.CreateAsync(participants, parameters.PageNumber, parameters.PageSize, count);
        }

        /// <inheritdoc />
        public async Task<WorkItemParticipant> UpdateAsync(WorkItemParticipant participant, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var participantToUpdate = await GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == participant.Id);

            if (participantToUpdate == null)
                throw new ModelException(WorkItemParticipant.DoesNotExist, true);

            var workItem = await _context.WorkItems
                .FirstOrDefaultAsync(x => x.Id == participant.WorkItemId);
			if (workItem == null)
				throw new ModelException(WorkItem.DoesNotExist, missingResource: true);
			if (workItem.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			participantToUpdate.EntityId = participant.EntityId;
            participantToUpdate.EntityType = participant.EntityType;

            await _context.SaveChangesAsync();

            return participantToUpdate;
        }
		#endregion

		#region [Methods] IParticipantRepository
		/// <inheritdoc />
		public async Task<List<WorkItemParticipant>> GetByWorkItem(long workItemId, WorkItemParticipantParameters parameters, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
            if (parameters == null)
            {
                parameters = new WorkItemParticipantParameters();
            }

            var workItem = await _context.WorkItems
				.FirstOrDefaultAsync(x => x.Id == workItemId);

			if (workItem == null)
				throw new ModelException(WorkItem.DoesNotExist, missingResource: true);
			if (workItem.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			var participants = GetQueryableByWorkItem(workItemId);
			var count = await participants.CountAsync();

			return await PagedList<WorkItemParticipant>.CreateAsync(participants, parameters.PageNumber, parameters.PageSize, count);
		}

		#endregion

		#region [Methods] Utility
		/// <summary>
		/// Gets the queryable.
		/// </summary>
		private IQueryable<WorkItemParticipant> GetQueryable()
        {
            return _context.WorkItemParticipants;
        }

		/// <summary>
		/// Gets the basic queryable.
		/// </summary>
		private IQueryable<WorkItemParticipant> GetBasicQueryable(UserMembership membership = null)
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
		private IQueryable<WorkItemParticipant> GetCompleteQueryable(UserMembership membership = null)
		{
			return GetBasicQueryable(membership)
				.Include(d => d.WorkItem);
		}

		/// <summary>
		/// Gets the queryable by work item.
		/// </summary>
		/// 
		/// <param name="workItemId">The work item id.</param>
		private IQueryable<WorkItemParticipant> GetQueryableByWorkItem(long workItemId)
        {
            return GetQueryable()
                .Where(p => p.WorkItemId == workItemId);
        }
        #endregion
    }
}