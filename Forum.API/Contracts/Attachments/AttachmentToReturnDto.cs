using Emsa.Mared.Discussions.API.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discussions.API.Contracts.Attachments
{
    /// <summary>
	/// The request data transfer object to return attachment.
	/// </summary>
    public class AttachmentToReturnDto
    {
        #region [Properties]
        /// <summary>
		/// Gets or sets the Id.
		/// </summary>
        public long Id { get; set; }

        /// <summary>
		/// Gets or sets the discussion id.
		/// </summary>
        public long DiscussionId { get; set; }

        /// <summary>
		/// Gets or sets the external id.
		/// </summary>
        public long ExternalId { get; set; }

        /// <summary>
		/// Gets or sets the url.
		/// </summary>
        public string Url { get; set; }
        #endregion
    }
}
