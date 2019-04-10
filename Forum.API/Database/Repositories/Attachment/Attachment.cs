using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.Discussions.API.Database.Repositories.Attachments
{
    /// <summary>
	/// Defines the documents entity.
	/// </summary>
    public class Attachment
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        public long ExternalId { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public long Url { get; set; }
        #endregion
    }
}
