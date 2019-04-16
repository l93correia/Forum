using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emsa.Mared.Common.Database.Utility;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Pagination;
using Emsa.Mared.Common.Security;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using Microsoft.EntityFrameworkCore;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemsRelations
{
    /// <inheritdoc />
    public class WorkItemRelationRepository : IWorkItemRelationRepository
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
        /// Initializes a new instance of the <see cref="WorkItemRelationRepository"/> class.
        /// </summary>
        /// 
        /// <param name="context">The context.</param>
		/// <param name="repoWorkItem">The work item repositoy.</param>
        public WorkItemRelationRepository(WorkItemContext context, IWorkItemRepository repoWorkItem)
        {
            this.context = context;
            this.repoWorkItem = repoWorkItem;
        }
        #endregion

        #region [Methods] IRepository
        /// <inheritdoc />
        public async Task<WorkItemRelation> CreateAsync(WorkItemRelation relation, UserMembership membership = null)
        {
            if (membership == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.repoWorkItem.ExistsAsync(relation.RelatedToWorkItemId))
                throw new ModelException(WorkItem.DoesNotExist, true);
            if (!await this.repoWorkItem.ExistsAsync(relation.RelatedFromWorkItemId))
                throw new ModelException(WorkItem.DoesNotExist, true);

            relation.RelationType = RelationType.Related;

            await context.WorkItemRelations.AddAsync(relation);
            await context.SaveChangesAsync();

            return relation;
        }

        /// <inheritdoc />
        public async Task<WorkItemRelation> UpdateAsync(WorkItemRelation relation, UserMembership membership = null)
        {
            if (membership == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.ExistsAsync(relation.Id))
                throw new ModelException(WorkItemRelation.DoesNotExist, missingResource: true);
            if (!await this.IsCreator(relation.RelatedToWorkItemId, membership))
                throw new ModelException(string.Empty, unauthorizedResource: true);

            var relationToUpdate = await this.context.WorkItemRelations
                .FirstOrDefaultAsync(x => x.Id == relation.Id);

            relationToUpdate.RelatedToWorkItemId = relation.RelatedToWorkItemId;
            relationToUpdate.RelatedFromWorkItemId = relation.RelatedFromWorkItemId;
            relationToUpdate.RelationType = relation.RelationType;

            await context.SaveChangesAsync();

            return relationToUpdate;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(long id, UserMembership membership = null)
        {
            if (membership == null)
                throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));

            if (!await this.ExistsAsync(id))
                throw new ModelException(WorkItemRelation.DoesNotExist, missingResource: true);
            if (!await this.IsCreator(id, membership))
                throw new ModelException(string.Empty, unauthorizedResource: true);

            var relation = await this.context.WorkItemRelations
                .FirstOrDefaultAsync(x => x.Id == id);

            context.WorkItemRelations.Remove(relation);
            await context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<WorkItemRelation> GetAsync(long id, UserMembership membership = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<PagedList<WorkItemRelation>> GetAllAsync(WorkItemRelationParameters parameters, UserMembership membership = null)
        {
            throw new NotImplementedException();
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
        private IQueryable<WorkItemRelation> GetQueryable()
        {
            return this.context.WorkItemRelations;
        }

        /// <summary>
        /// Gets the basic queryable and filters it by user.
        /// </summary>
        //private IQueryable<WorkItemRelation> GetParticipantQueryable(long? workItemId = null, UserMembership membership = null)
        //{
        //    var queryable = this.GetQueryable();

        //    if (workItemId != null)
        //    {
        //        queryable = queryable
        //            .Where(d => d.RelatedToWorkItemId == workItemId);
        //    }

        //    if (membership != null)
        //    {
        //        queryable = queryable
        //            .Where(d => d.WorkItem.WorkItemParticipants.Any(p => (membership.UserId == p.EntityId && p.EntityType == EntityType.User)
        //            || (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group)
        //            || (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)));
        //    }

        //    return queryable;
        //}

        /// <summary>
        /// Gets the participant queryable and includes sub-entities.
        /// </summary>

        #endregion
    }
}
