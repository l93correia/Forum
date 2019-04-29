using Emsa.Mared.Common.Database.Repositories;
using Emsa.Mared.Common.Security;
using System.Threading.Tasks;

namespace Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemComments
{
    /// <summary>
	/// Provides CRUD methods over message entities as well as other utility methods.
	/// </summary>
	/// 
	/// <seealso cref="IRepository{WorkItemComment, Long, WorkItemCommentParameters}" />
    public interface IWorkItemCommentRepository : IRepository<WorkItemComment, long, WorkItemCommentParameters>
    {
        /// <summary>
        /// Check if comment belongs to work item.
        /// </summary>
        /// 
        /// <param name="workItemId">The work item id.</param>
        /// <param name="commentId">The comment id.</param>
        Task<bool> BelongsToWorkItem(long workItemId, long commentId);

        /// <summary>
        /// Check if membership is the owner of the work item comment.
        /// </summary>
        /// 
        /// <param name="id">The work item comment id.</param>
        /// <param name="membership">The membership.</param>
        Task<bool> IsCreator(long id, UserMembership membership);
    }
}