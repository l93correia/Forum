﻿using Emsa.Mared.Common.Database;
using Emsa.Mared.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemParticipants
{
    /// <summary>
	/// Provides pagination parameters as well as filtering over the participants queries.
	/// </summary>
	/// 
	/// <seealso cref="PaginationParameters" />
    public class WorkItemParticipantParameters : PaginationParameters
    {
        /// <summary>
        /// Gets or sets the work item id.
        /// </summary>
        public long WorkItemId { get; set; }
    }
}
