using Forum.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Dtos
{
    /// <summary>
	/// The request data transfer object to return a discussion.
	/// </summary>
    public class DiscussionToReturnDto
    {
        /// <summary>
		/// Gets or sets the Id.
		/// </summary>
        public long Id { get; set; }

        /// <summary>
		/// Gets or sets the Subject.
		/// </summary>
        public string Subject { get; set; }

        /// <summary>
		/// Gets or sets the Username.
		/// </summary>
        public string Username { get; set; }

        /// <summary>
		/// Gets or sets the CreatedDate.
		/// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
		/// Gets or sets the Comment.
		/// </summary>
        public string Comment { get; set; }

        /// <summary>
		/// Gets or sets the Status.
		/// </summary>
        public string Status { get; set; }

        /// <summary>
		/// Gets or sets the EndDate.
		/// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
		/// Gets or sets the UpdatedDate.
		/// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
		/// Gets or sets the DiscussionResponses.
		/// </summary>
        public List<ResponseToReturnDto> DiscussionResponses { get; set; }
    }
}
