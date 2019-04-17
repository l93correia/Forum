using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemAttachments;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemComments;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemRelations;
using Microsoft.EntityFrameworkCore;

namespace Emsa.Mared.WorkItems.API.Database
{
    /// <summary>
    /// Implements the Discussions database context for entity framework.
    /// </summary>
    /// 
    /// <seealso cref="DbContext" />
    public class WorkItemContext : DbContext
    {
        #region [Properties]
        /// <summary>
        /// The discussions.
        /// </summary>
        public DbSet<WorkItem> WorkItems { get; set; }

        /// <summary>
        /// The discussion participants.
        /// </summary>
        public DbSet<WorkItemParticipant> WorkItemParticipants { get; set; }

        /// <summary>
        /// The discussion responses.
        /// </summary>
        public DbSet<WorkItemComment> WorkItemComments { get; set; }

        /// <summary>
        /// The attachments.
        /// </summary>
        public DbSet<WorkItemAttachment> WorkItemAttachments { get; set; }

        /// <summary>
        /// The relations.
        /// </summary>
        public DbSet<WorkItemRelation> WorkItemRelations { get; set; }
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemContext" /> class.
        /// </summary>
        public WorkItemContext(DbContextOptions<WorkItemContext> options) : base(options) { }
        #endregion

        #region [Methods]
        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new WorkItemConfiguration());
            builder.ApplyConfiguration(new WorkItemCommentConfiguration());
            builder.ApplyConfiguration(new WorkItemParticipantConfiguration());
            builder.ApplyConfiguration(new WorkItemAttachmentConfiguration());
        }
        #endregion
    }
}
