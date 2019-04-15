using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using Emsa.Mared.Common.Database.Repositories;
using Emsa.Mared.Common.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemComments
{
    /// <summary>
	/// Provides CRUD methods over message entities as well as other utility methods.
	/// </summary>
	/// 
	/// <seealso cref="IRepository{Response, Long, ResponseParameters}" />
    public interface IWorkItemCommentRepository : IRepository<WorkItemComment, long, WorkItemCommentParameters>
    {
        /// <summary>
		/// Get a response by discussion id.
		/// </summary>
		/// 
		/// <param name="discussionId">The discussion id.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="membership">The membership.</param>
        Task<List<WorkItemComment>> GetByDiscussion(long discussionId, WorkItemCommentParameters parameters, UserMembership membership = null);
    }
}
