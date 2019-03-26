using Emsa.Mared.Common.Models;
using Forum.API.Models;
using Forum.API.Models.Repository.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Data
{
    /// <summary>
	/// Provides CRUD methods over message entities as well as other utility methods.
	/// </summary>
	/// 
	/// <seealso cref="IRepository{DiscussionResponses, ResponseParameters}" />
    public interface IResponseRepository : IRepository<DiscussionResponses, long, ResponseParameters>
    {
        /// <summary>
		/// Get a response by discussion id.
		/// </summary>
		/// 
		/// <param name="id">The discussion id.</param>
		/// <param name="parameters">The parameters.</param>
        Task<PagedList<DiscussionResponses>> GetByDiscussion(long id, ResponseParameters parameters);
    }
}
