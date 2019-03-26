using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models
{
    /// <summary>
	/// Defines the discussion responses type entity.
	/// </summary>
    public class DiscussionResponses
    {
        #region [Constants]
        /// <summary>
        /// The response does not exist message.
        /// </summary>
        public const string DoesNotExist = "The Response does not exist.";
        #endregion

        #region [Properties]
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the discussion id.
        /// </summary>
        public long DiscussionId { get; set; }

        /// <summary>
        /// Gets or sets the discussion.
        /// </summary>
        public Discussions Discussion { get; set; }

        /// <summary>
        /// Gets or sets the created by id.
        /// </summary>
        public long CreatedById { get; set; }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        public User CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the updated date.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the DocumentId.
        /// </summary>
        public long? DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the Document.
        /// </summary>
        public Document Document { get; set; }
        #endregion
    }
}
