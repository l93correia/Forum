﻿using Emsa.Mared.Common.Database;
using Emsa.Mared.Common.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemAttachments
{
    /// <summary>
	/// Provides pagination parameters as well as filtering over the attachment queries.
	/// </summary>
	/// 
	/// <seealso cref="PaginationParameters" />
    public class WorkItemAttachmentParameters : PaginationParameters
    {
        /// <summary>
        /// Gets or sets the work item id.
        /// </summary>
        public long WorkItemId { get; set; }
    }
}
