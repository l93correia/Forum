using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Database.Repositories.Attachments
{
    /// <summary>
	/// Provides CRUD methods over message entities as well as other utility methods.
	/// </summary>
	/// 
	/// <seealso cref="IRepository{Attachment, Long, AttachmentParameters}" />
    public interface IAttachmentRepository : IRepository<Attachment, long, AttachmentParameters>
    {
        /// <summary>
        /// Get a response by discussion id.
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="membership">The membership.</param>
        Task<List<Attachment>> GetByDiscussion(long discussionId, AttachmentParameters parameters, UserMembership membership = null);

    }
}
