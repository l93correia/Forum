using Emsa.Mared.Common.Database.Repositories;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItems;
using System;

namespace Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemComments
{
	/// <summary>
	/// Defines the discussion responses type entity.
	/// </summary>
	/// 
	/// <seealso cref="IEntity" />
	public class WorkItemComment : IEntity
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
        /// Gets or sets the WorkItemId.
        /// </summary>
        public long WorkItemId { get; set; }

        /// <summary>
        /// Gets or sets the discussion.
        /// </summary>
        public WorkItem WorkItem { get; set; }

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
        public WorkItemCommentStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the updated date.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        #endregion
    }

    #region [Enum]
    /// <summary>
	/// Defines the status.
	/// </summary>
    public enum WorkItemCommentStatus
    {
        Default = 0,
        Created = 1,
        Updated = 2,
        Removed = 4
    }
    #endregion
}
