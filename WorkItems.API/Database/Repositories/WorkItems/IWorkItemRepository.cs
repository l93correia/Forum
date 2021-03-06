﻿using Emsa.Mared.Common.Database.Repositories;
using Emsa.Mared.Common.Security;

using System.Threading.Tasks;

namespace Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItems
{
    /// <summary>
	/// Provides CRUD methods over message entities as well as other utility methods.
	/// </summary>
	/// 
	/// <seealso cref="IRepository{Discussions, Long, DiscussionParameters}" />
    public interface IWorkItemRepository : IRepository<WorkItem, long, WorkItemParameters>
    {
        /// <summary>
        /// Check if a work item has specific type.
        /// </summary>
        /// 
        /// <param name="workItemId">The work item id.</param>
        /// <param name="workItemType">The work item type.</param>
        Task<bool> IsType(long workItemId, WorkItemType workItemType);

        /// <summary>
        /// Check if membership is the owner of the work item.
        /// </summary>
        /// 
        /// <param name="id">The work item id.</param>
        /// <param name="membership">The membership.</param>
        Task<bool> IsCreator(long id, UserMembership membership);

        /// <summary>
        /// Check if membership is a participant in the work item.
        /// </summary>
        /// 
        /// <param name="id">The work item id.</param>
        /// <param name="membership">The membership.</param>
        Task<bool> IsParticipant(long id, UserMembership membership);
    }
}