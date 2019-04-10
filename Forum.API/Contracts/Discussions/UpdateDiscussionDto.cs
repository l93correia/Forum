using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Contracts
{
    /// <summary>
	/// The request data transfer object to update a discussion.
	/// </summary>
    public class UpdateDiscussionDto
    {
        #region [Properties]
        /// <summary>
		/// Gets or sets the Subject.
		/// </summary>
        [Required]
        public string Subject { get; set; }

        /// <summary>
		/// Gets or sets the Comment.
		/// </summary>
        [Required]
        public string Comment { get; set; }
        #endregion
    }
}
