using Emsa.Mared.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemRelations
{
    /// <summary>
	/// Provides pagination parameters as well as filtering over the relations queries.
	/// </summary>
	/// 
	/// <seealso cref="PaginationParameters" />
    public class WorkItemRelationParameters : PaginationParameters
    {
        /// <summary>
        /// Gets or sets the work item id.
        /// </summary>
        public long workItemId { get; set; }
    }
}
