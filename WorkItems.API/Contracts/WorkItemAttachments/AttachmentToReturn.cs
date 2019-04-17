using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Contracts.WorkItemAttachments
{
    public class AttachmentToReturn
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the discussion identifier.
        /// </summary>
        public long WorkItemId { get; set; }

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
