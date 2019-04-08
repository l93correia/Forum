using Emsa.Mared.Common.Database;

namespace Emsa.Mared.Discussions.API.Database.Repositories
{
    /// <summary>
	/// Provides CRUD methods over message entities as well as other utility methods.
	/// </summary>
	/// 
	/// <seealso cref="IRepository{Participant, Long, ParticipantParameters}" />
    public interface IParticipantRepository : IRepository<Participant, long, ParticipantParameters>
    {
    }
}
