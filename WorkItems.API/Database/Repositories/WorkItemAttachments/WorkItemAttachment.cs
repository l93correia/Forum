using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemAttachments
{
    /// <summary>
	/// Defines the Attachment entity.
	/// </summary>
    public class WorkItemAttachment
    {
        #region [Constants]
        /// <summary>
        /// The attachment does not exist message.
        /// </summary>
        public const string DoesNotExist = "The Attachment does not exist.";
        #endregion

        #region [Properties]
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the discussion identifier.
        /// </summary>
        public long WorkItemId { get; set; }

        /// <summary>
        /// Gets or sets the Discussion.
        /// </summary>
        public WorkItem WorkItem { get; set; }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        public long ExternalId { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string Url { get; set; }
        #endregion
    }
}
