using Emsa.Mared.Common.Pagination;

namespace Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemComments
{
    /// <summary>
	/// Provides pagination parameters as well as filtering over the discussion queries.
	/// </summary>
	/// 
	/// <seealso cref="PaginationParameters" />
    public class WorkItemCommentParameters : PaginationParameters
    {
        /// <summary>
        /// Gets or sets the work item id.
        /// </summary>
        public long WorkItemId { get; set; }
    }
}
