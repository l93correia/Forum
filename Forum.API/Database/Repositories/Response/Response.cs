using Emsa.Mared.Discussions.API.Database.Repositories.Discussions;
using System;

namespace Emsa.Mared.Discussions.API.Database.Repositories.Responses
{
    /// <summary>
	/// Defines the discussion responses type entity.
	/// </summary>
    public class Response
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
        public Discussion Discussion { get; set; }

        /// <summary>
        /// Gets or sets the created by id.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        public string Comment { get; set; }

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
        #endregion
    }
}
