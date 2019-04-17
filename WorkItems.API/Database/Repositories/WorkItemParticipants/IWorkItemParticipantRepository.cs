using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using Emsa.Mared.Common.Database.Repositories;
using Emsa.Mared.Common.Security;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants
{
    /// <summary>
	/// Provides CRUD methods over message entities as well as other utility methods.
	/// </summary>
	/// 
	/// <seealso cref="IRepository{WorkItemParticipant, Long, WorkItemParticipantParameters}" />
    public interface IWorkItemParticipantRepository : IRepository<WorkItemParticipant, long, WorkItemParticipantParameters>
    {
        /// <summary>
        /// Check if participant belongs to work item.
        /// </summary>
        /// 
        /// <param name="workItemId">The work item id.</param>
        /// <param name="participantId">The participant id.</param>
        Task<bool> BelongsToWorkItem(long workItemId, long participantId);

        /// <summary>
        /// Check if membership is the owner of the work item participant.
        /// </summary>
        /// 
        /// <param name="id">The work item comment id.</param>
        /// <param name="membership">The membership.</param>
        Task<bool> IsCreator(long id, UserMembership membership);
    }
}
