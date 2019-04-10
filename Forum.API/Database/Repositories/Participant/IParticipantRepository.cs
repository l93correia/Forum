using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Database.Repositories
{
    /// <summary>
	/// Provides CRUD methods over message entities as well as other utility methods.
	/// </summary>
	/// 
	/// <seealso cref="IRepository{Participant, Long, ParticipantParameters}" />
    public interface IParticipantRepository : IRepository<Participant, long, ParticipantParameters>
    {
        /// <summary>
		/// Get a response by discussion id.
		/// </summary>
		/// 
		/// <param name="discussionId">The discussion id.</param>
		/// <param name="parameters">The parameters.</param>
		/// <param name="membership">The membership.</param>
        Task<List<Participant>> GetByDiscussion(long discussionId, ParticipantParameters parameters, UserMembership membership = null);
    }
}
