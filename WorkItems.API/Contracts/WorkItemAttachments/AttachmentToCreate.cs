using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Contracts.WorkItemAttachments
{
    public class AttachmentToCreate
    {
        #region [Properties]
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
