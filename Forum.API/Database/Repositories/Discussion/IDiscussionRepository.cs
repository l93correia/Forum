using Emsa.Mared.Common.Database;
using Emsa.Mared.Discussions.API.Database.Repository;
using Emsa.Mared.Discussions.API.Database.Repositories.Discussions;

namespace Emsa.Mared.Discussions.API.Database.Repositories
{
    /// <summary>
	/// Provides CRUD methods over message entities as well as other utility methods.
	/// </summary>
	/// 
	/// <seealso cref="IRepository{Discussions, Long, DiscussionParameters}" />
    public interface IDiscussionRepository : IRepository<Discussion, long, DiscussionParameters>
    {
    }
}
