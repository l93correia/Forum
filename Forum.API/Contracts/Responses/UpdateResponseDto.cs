using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Contracts
{
    /// <summary>
	/// The request data transfer object to update a response.
	/// </summary>
    public class UpdateResponseDto
    {
        /// <summary>
		/// Gets or sets the Response.
		/// </summary>
        [Required]
        public string Comment { get; set; }
    }
}
