using Emsa.Mared.Common.Database;
using Emsa.Mared.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems
{
    /// <summary>
	/// Provides pagination parameters as well as filtering over the discussion queries.
	/// </summary>
	/// 
	/// <seealso cref="PaginationParameters" />
    public sealed class WorkItemParameters : PaginationParameters
    {
        /// <summary>
        /// Gets or sets the work item type.
        /// </summary>
        public WorkItemType? WorkItemType { get; set; }
    }
}
