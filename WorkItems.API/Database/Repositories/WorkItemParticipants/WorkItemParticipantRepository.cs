using System;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly WorkItemContext context;

        /// <summary>
		/// Gets or sets the discussion repository.
		/// </summary>
		private readonly IWorkItemRepository repoWorkItem;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemParticipantRepository"/> class.
        /// </summary>
        /// 
        /// <param name="context">The context.</param>
		/// <param name="repoWorkItem">The work item repositoy.</param>
        public WorkItemParticipantRepository(WorkItemContext context, IWorkItemRepository repoWorkItem)
        {
            this.context = context;
            this.repoWorkItem = repoWorkItem;
        }
        #endregion

        #region [Methods] IRepository
        /// <inheritdoc />
        public async Task<WorkItemParticipant> CreateAsync(WorkItemParticipant participantToCreate, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.repoWorkItem.ExistsAsync(participantToCreate.WorkItemId))
                throw new ModelException(WorkItem.DoesNotExist, true);
            if (!await this.IsCreator(participantToCreate.WorkItemId, membership))
                throw new ModelException(string.Empty, unauthorizedResource: true);

            if (participantToCreate.EntityType == EntityType.Default)
                throw new ModelException(participantToCreate.InvalidFieldMessage(p => p.EntityType));
            if (participantToCreate.EntityId <= 0)
                throw new ModelException(participantToCreate.InvalidFieldMessage(p => p.EntityType));

			await context.WorkItemParticipants.AddAsync(participantToCreate);
            await context.SaveChangesAsync();

            return participantToCreate;
        }

        /// <inheritdoc />
        public async Task<WorkItemParticipant> UpdateAsync(WorkItemParticipant participant, UserMembership membership = null)
        {
            if (membership == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.ExistsAsync(participant.Id))
                throw new ModelException(WorkItemParticipant.DoesNotExist, missingResource: true);
            if (!await this.IsCreator(participant.WorkItemId, membership))
                throw new ModelException(string.Empty, unauthorizedResource: true);

            var participantToUpdate = await this.context.WorkItemParticipants
                .FirstOrDefaultAsync(x => x.Id == participant.Id);

            participantToUpdate.EntityId = participant.EntityId;
            participantToUpdate.EntityType = participant.EntityType;

            await context.SaveChangesAsync();

            return participantToUpdate;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(long id, UserMembership membership = null)
        {
            if (membership == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.ExistsAsync(id))
                throw new ModelException(WorkItemParticipant.DoesNotExist, missingResource: true);
            if (!await this.IsCreator(id, membership))
                throw new ModelException(string.Empty, unauthorizedResource: true);

            var participant = await this.context.WorkItemParticipants
                .FirstOrDefaultAsync(x => x.Id == id);

            if (participant.EntityId == membership.UserId && participant.EntityType == EntityType.User)
                throw new ModelException(string.Empty, unauthorizedResource: true);

			context.WorkItemParticipants.Remove(participant);
            await context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<WorkItemParticipant> GetAsync(long id, UserMembership membership = null)
        {
            if (membership == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.ExistsAsync(id))
                throw new ModelException(WorkItemParticipant.DoesNotExist, missingResource: true);

            var participant = await this.GetCompleteQueryable(membership: membership)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (!await this.repoWorkItem.IsParticipant(participant.WorkItemId, membership))
                throw new ModelException(string.Empty, unauthorizedResource: true);

            return participant;
        }

        /// <inheritdoc />
        public async Task<PagedList<WorkItemParticipant>> GetAllAsync(WorkItemParticipantParameters parameters, UserMembership membership = null)
        {
            if (membership == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (parameters == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(parameters)));

            var participants = this.GetCompleteQueryable(parameters.workItemId, membership);
			var count = await this.GetParticipantQueryable(parameters.workItemId, membership).CountAsync();

            return await PagedList<WorkItemParticipant>.CreateAsync(participants, parameters.PageNumber, parameters.PageSize, count);
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
        /// Gets the queryable.
        /// </summary>
        private IQueryable<WorkItemParticipant> GetQueryable()
        {
            return context.WorkItemParticipants;
        }

        /// <summary>
        /// Gets the basic queryable and filters it by user.
        /// </summary>
        private IQueryable<WorkItemParticipant> GetParticipantQueryable(long? workItemId = null, UserMembership membership = null)
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
        private IQueryable<WorkItemParticipant> GetCompleteQueryable(long? workItemId = null, UserMembership membership = null)
        {
            var queryable = this.GetParticipantQueryable(workItemId, membership)
                .Include(x => x.WorkItem);

            return queryable;
        }
        #endregion
    }
}