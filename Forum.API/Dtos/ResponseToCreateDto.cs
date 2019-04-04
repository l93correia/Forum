using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Dtos
{
    /// <summary>
	/// The request data transfer object to create a response.
	/// </summary>
    public class ResponseToCreateDto
    {
        /// <summary>
		/// Gets or sets the Response.
		/// </summary>
        [Required]
        public string Response { get; set; }

        /// <summary>
		/// Gets or sets the CreatedById.
		/// </summary>
        [Required]
        public long UserId { get; set; }
    }
}
