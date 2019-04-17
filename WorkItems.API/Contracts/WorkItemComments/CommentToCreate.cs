using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemComments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Emsa.Mared.WorkItems.API.Contracts.WorkItemComments
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
