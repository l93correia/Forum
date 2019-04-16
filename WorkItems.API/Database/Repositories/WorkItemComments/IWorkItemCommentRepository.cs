using Emsa.Mared.Common.Database.Repositories;
using Emsa.Mared.Common.Security;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemComments
{
    /// <summary>
	/// Provides CRUD methods over message entities as well as other utility methods.
	/// </summary>
	/// 
	/// <seealso cref="IRepository{WorkItemComment, Long, WorkItemCommentParameters}" />
    public interface IWorkItemCommentRepository : IRepository<WorkItemComment, long, WorkItemCommentParameters>
    {
        /// <summary>
        /// Check if membership is the owner of the work item comment.
        /// </summary>
        /// 
        /// <param name="id">The work item comment id.</param>
        /// <param name="membership">The membership.</param>
        Task<bool> IsCreator(long id, UserMembership membership);
    }
}