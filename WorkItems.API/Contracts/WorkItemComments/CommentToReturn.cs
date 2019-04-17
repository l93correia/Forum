using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemComments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Contracts.WorkItemComments
{
    public class CommentToReturn
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the WorkItemId.
        /// </summary>
        public long WorkItemId { get; set; }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the created by id.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public Status Status { get; set; }

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
