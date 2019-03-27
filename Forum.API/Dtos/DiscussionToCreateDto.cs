using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Dtos
{
    /// <summary>
	/// The request data transfer object to create a discussion.
	/// </summary>
    public class DiscussionToCreateDto
    {
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

        /// <summary>
		/// Gets or sets the UserId.
		/// </summary>
        [Required]
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}
