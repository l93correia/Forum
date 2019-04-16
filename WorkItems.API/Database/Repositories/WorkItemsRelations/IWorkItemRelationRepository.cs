using Emsa.Mared.Common.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemsRelations
{
    /// <summary>
	/// Provides CRUD methods over message entities as well as other utility methods.
	/// </summary>
	/// 
	/// <seealso cref="IRepository{WorkItemRelation, Long, WorkItemRelationParameters}" />
    interface IWorkItemRelationRepository : IRepository<WorkItemRelation, long, WorkItemRelationParameters>
    {
    }
}
