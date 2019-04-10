using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Contracts
{
    /// <summary>
	/// The request data transfer object to return a list of discussions.
	/// </summary>
    public class DiscussionForListDto
    {
        #region [Properties]
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
		/// Gets or sets the UpdateDate.
		/// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
		/// Gets or sets the ResponsesCount.
		/// </summary>
        public int ResponsesCount { get; set; }
        #endregion
    }
}
