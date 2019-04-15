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
	/// <seealso cref="IRepository{Participant, Long, ParticipantParameters}" />
    public interface IWorkItemParticipantRepository : IRepository<WorkItemParticipant, long, WorkItemParticipantParameters>
    {
        /// <summary>
		/// Get a response by discussion id.
		/// </summary>
		/// 
		/// <param name="discussionId">The discussion id.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="membership">The membership.</param>
        Task<List<WorkItemParticipant>> GetByWorkItem(long discussionId, WorkItemParticipantParameters parameters, UserMembership membership = null);
    }
}
