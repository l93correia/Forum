using Emsa.Mared.Common.Database.Repositories;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemAttachments;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemComments;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItems;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemRelations;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System;

namespace Emsa.Mared.ContentManagement.WorkItems.Database
{
    /// <summary>
    /// Implements the work items database context for entity framework.
    /// </summary>
    /// 
    /// <seealso cref="DbContext" />
    public class WorkItemsContext : DbContext
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
        /// Initializes a new instance of the <see cref="WorkItemsContext" /> class.
        /// </summary>
        public WorkItemsContext(DbContextOptions<WorkItemsContext> options) : base(options) { }
        #endregion

        #region [Methods]
        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new WorkItemConfiguration());
			builder.ApplyConfiguration(new WorkItemAttachmentConfiguration());
			builder.ApplyConfiguration(new WorkItemCommentConfiguration());
            builder.ApplyConfiguration(new WorkItemParticipantConfiguration());
			builder.ApplyConfiguration(new WorkItemRelationConfiguration());
		}
        #endregion

        #region [Methods] Save Changes
        /// <inheritdoc />
        public override int SaveChanges()
        {
            this.UpdateEntityTimestamps();

            return base.SaveChanges();
        }

        /// <inheritdoc />
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.UpdateEntityTimestamps();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            this.UpdateEntityTimestamps();

            return base.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.UpdateEntityTimestamps();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Updates the entities in the change tracker that were either added or updated.
        /// If an entity was added, then then created at is automatically filled.
        /// If an entity was modified, then then updated at is automatically filled.
        /// </summary>
        private void UpdateEntityTimestamps()
        {
            foreach (var entity in ChangeTracker.Entries().Where(p => p.State == EntityState.Added))
            {
                if (entity.Entity is IEntity created)
                {
                    created.CreatedAt = DateTime.UtcNow;
                }
            }

            foreach (var entity in ChangeTracker.Entries().Where(p => p.State == EntityState.Modified))
            {
                if (entity.Entity is IEntity updated)
                {
                    updated.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
        #endregion
    }
}
