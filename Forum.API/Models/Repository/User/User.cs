using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Models
{
    /// <summary>
	/// Defines the user type entity.
	/// </summary>
    public class User
    {
        #region [Constants]
        /// <summary>
        /// The user does not exist message.
        /// </summary>
        public const string DoesNotExist = "The User does not exist.";
        #endregion

        #region [Properties]
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
        #endregion
    }
}
