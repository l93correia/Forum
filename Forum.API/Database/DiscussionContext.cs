using Emsa.Mared.Discussions.API.Database.Repositories;
using Emsa.Mared.Discussions.API.Database.Repositories.Attachments;
using Emsa.Mared.Discussions.API.Database.Repositories.Responses;
using Emsa.Mared.Discussions.API.Database.Repositories.Users;
using Microsoft.EntityFrameworkCore;

namespace Emsa.Mared.Discussions.API.Database
{
    /// <summary>
    /// Implements the Discussions database context for entity framework.
    /// </summary>
    /// 
    /// <seealso cref="DbContext" />
    public class DiscussionContext : DbContext
    {
        #region [Properties]
        /// <summary>
        /// The discussions.
        /// </summary>
        public DbSet<Discussion> Discussions { get; set; }

        /// <summary>
        /// The discussion participants.
        /// </summary>
        public DbSet<Participant> Participants { get; set; }

        /// <summary>
        /// The discussion responses.
        /// </summary>
        public DbSet<Response> Responses { get; set; }

        /// <summary>
        /// The user.
        /// </summary>
        public DbSet<User> User { get; set; }

        /// <summary>
        /// The document.
        /// </summary>
        public DbSet<Attachment> Attachments { get; set; }
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscussionContext" /> class.
        /// </summary>
        public DiscussionContext(DbContextOptions<DiscussionContext> options) : base(options) { }
        #endregion

        #region [Methods]
        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new DiscussionConfiguration());
            builder.ApplyConfiguration(new ResponseConfiguration());
        }
        #endregion
    }
}
