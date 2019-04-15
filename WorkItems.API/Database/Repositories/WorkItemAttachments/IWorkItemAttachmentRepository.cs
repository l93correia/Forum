using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using Emsa.Mared.Common.Database.Repositories;
using Emsa.Mared.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemAttachments
{
    /// <summary>
	/// Provides CRUD methods over message entities as well as other utility methods.
	/// </summary>
	/// 
	/// <seealso cref="IRepository{Attachment, Long, AttachmentParameters}" />
    public interface IWorkItemAttachmentRepository : IRepository<WorkItemAttachment, long, WorkItemAttachmentParameters>
    {
        /// <summary>
        /// Get a response by discussion id.
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="membership">The membership.</param>
        Task<List<WorkItemAttachment>> GetByDiscussion(long discussionId, WorkItemAttachmentParameters parameters, UserMembership membership = null);

    }
}
