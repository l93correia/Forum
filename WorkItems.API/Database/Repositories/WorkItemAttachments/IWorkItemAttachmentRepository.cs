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
	/// <seealso cref="IRepository{WorkItemAttachment, Long, WorkItemAttachmentParameters}" />
    public interface IWorkItemAttachmentRepository : IRepository<WorkItemAttachment, long, WorkItemAttachmentParameters>
    {
        /// <summary>
        /// Check if membership is the owner of the work item attachment.
        /// </summary>
        /// 
        /// <param name="id">The work item comment id.</param>
        /// <param name="membership">The membership.</param>
        Task<bool> IsCreator(long id, UserMembership membership);
    }
}
