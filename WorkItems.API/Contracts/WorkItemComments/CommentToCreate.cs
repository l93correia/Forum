using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemComments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.ContentManagement.WorkItems.Contracts.WorkItemComments
{
    public class CommentToCreate
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        public string Comment { get; set; }
        #endregion
    }
}
