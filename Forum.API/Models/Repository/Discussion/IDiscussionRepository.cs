using Emsa.Mared.Common.Database;
using Forum.API.Dtos;
using Forum.API.Models;
using Forum.API.Models.Repository.Discussions;
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
	/// <seealso cref="IRepository{Discussions, Long, DiscussionParameters}" />
    public interface IDiscussionRepository : IRepository<Discussion, long, DiscussionParameters>
    {
    }
}
