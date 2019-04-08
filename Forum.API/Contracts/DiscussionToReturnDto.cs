using System;
using System.Collections.Generic;

namespace Emsa.Mared.Discussions.API.Contracts
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
		/// Gets or sets the Responses.
		/// </summary>
        public List<ResponseToReturnDto> Responses { get; set; }
    }
}
