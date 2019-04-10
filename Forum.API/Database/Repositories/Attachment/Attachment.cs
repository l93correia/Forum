using Emsa.Mared.Discussions.API.Database.Repositories.Discussions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Database.Repositories.Attachments
{
    /// <summary>
	/// Defines the Attachment entity.
	/// </summary>
    public class Attachment
    {
        #region [Constants]
        /// <summary>
        /// The attachment does not exist message.
        /// </summary>
        public const string DoesNotExist = "The Attachment does not exist.";
        #endregion

        #region [Properties]
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the discussion identifier.
        /// </summary>
        public long DiscussionId { get; set; }

        /// <summary>
        /// Gets or sets the Discussion.
        /// </summary>
        public Discussion Discussion { get; set; }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        public long ExternalId { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public string Url { get; set; }
        #endregion
    }
}
