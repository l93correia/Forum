using Emsa.Mared.Common.Database;
using Emsa.Mared.Common.Database.Repositories;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems
{
    /// <summary>
	/// Provides CRUD methods over message entities as well as other utility methods.
	/// </summary>
	/// 
	/// <seealso cref="IRepository{Discussions, Long, DiscussionParameters}" />
    public interface IWorkItemRepository : IRepository<WorkItem, long, WorkItemParameters>
    {
    }
}
