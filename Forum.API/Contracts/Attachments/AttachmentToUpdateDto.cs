using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Discussions.API.Contracts.Attachments
{
    /// <summary>
	/// The request data transfer object to update attachment.
	/// </summary>
    public class AttachmentToUpdateDto
    {
        #region [Properties]
        /// <summary>
		/// Gets or sets the discussion id.
		/// </summary>
        [Required]
        public long DiscussionId { get; set; }

        /// <summary>
		/// Gets or sets the external id.
		/// </summary>
        [Required]
        public long ExternalId { get; set; }

        /// <summary>
		/// Gets or sets the url.
		/// </summary>
        [Required]
        public string Url { get; set; }
        #endregion
    }
}
