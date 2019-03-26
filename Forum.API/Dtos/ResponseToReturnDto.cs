﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Dtos
{
    /// <summary>
	/// The request data transfer object to return a response.
	/// </summary>
    public class ResponseToReturnDto
    {
        /// <summary>
		/// Gets or sets the Id.
		/// </summary>
        public long Id { get; set; }

        /// <summary>
		/// Gets or sets the DiscussionId.
		/// </summary>
        public long DiscussionId { get; set; }

        /// <summary>
		/// Gets or sets the Username.
		/// </summary>
        public string Username { get; set; }

        /// <summary>
		/// Gets or sets the Response.
		/// </summary>
        public string Response { get; set; }

        /// <summary>
		/// Gets or sets the CreatedDate.
		/// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
		/// Gets or sets the UpdatedDate.
		/// </summary>
        public DateTime? UpdatedDate { get; set; }
    }
}
