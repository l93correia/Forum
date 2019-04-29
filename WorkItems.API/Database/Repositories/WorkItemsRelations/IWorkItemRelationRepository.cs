using Emsa.Mared.Common.Database.Repositories;
using System.Threading.Tasks;

namespace Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemRelations
{
    /// <summary>
	/// Provides CRUD methods over message entities as well as other utility methods.
	/// </summary>
	/// 
	/// <seealso cref="IRepository{WorkItemRelation, Long, WorkItemRelationParameters}" />
    public interface IWorkItemRelationRepository : IRepository<WorkItemRelation, long, WorkItemRelationParameters>
    {
        /// <summary>
        /// Check if relation belongs to work item.
        /// </summary>
        /// 
        /// <param name="workItemId">The work item id.</param>
        /// <param name="relationId">The relation id.</param>
        Task<bool> BelongsToWorkItem(long workItemId, long relationId);
    }
}