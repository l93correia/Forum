using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models
{
    /// <summary>
	/// Defines the discussions entity.
	/// </summary>
    public class Discussion
    {
        #region [Constants]
        /// <summary>
        /// The discussion does not exist message.
        /// </summary>
        public const string DoesNotExist = "The Discussion does not exist.";

        /// <summary>
        /// Empty discussions message.
        /// </summary>
        public const string Empty = "No Discussions founded.";
        #endregion

        #region [Properties]
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the Subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the UserId.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the User.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the CreatedDate.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the EndDate.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the DocumentId.
        /// </summary>
        public long? DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the Document.
        /// </summary>
        public Document Document { get; set; }

        /// <summary>
        /// Gets or sets the Comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the Status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the UpdatedDate.
        /// </summary>
        public DateTime? UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the DiscussionResponses.
        /// </summary>
        public ICollection<DiscussionResponse> DiscussionResponses { get; set; }
        #endregion
    }
}
