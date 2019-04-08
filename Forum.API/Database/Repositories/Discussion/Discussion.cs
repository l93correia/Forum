using Emsa.Mared.Discussions.API.Database.Repositories.Attachments;
using Emsa.Mared.Discussions.API.Database.Repositories.Responses;
using Emsa.Mared.Discussions.API.Database.Repositories.Users;
using System;
using System.Collections.Generic;

namespace Emsa.Mared.Discussions.API.Database.Repositories
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
        public ICollection<Attachment> Attachments { get; set; }

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
        /// Gets or sets the Responses.
        /// </summary>
        public ICollection<Response> Responses { get; set; }

        /// <summary>
        /// Gets or sets the Participants.
        /// </summary>
        public ICollection<Participant> Participants { get; set; }
        #endregion
    }
}
