using Forum.API.Models;
using Forum.API.Models.Repository.Discussions;
using Forum.API.Models.Repository.Organization;
using Forum.API.Models.Repository.Response;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Data
{
    /// <summary>
    /// Implements the Forum database context for entity framework.
    /// </summary>
    /// 
    /// <seealso cref="DbContext" />
    public class DataContext : DbContext
    {
        #region [Properties]
        /// <summary>
        /// The discussions.
        /// </summary>
        public DbSet<Discussion> Discussion { get; set; }

        /// <summary>
        /// The discussion participants.
        /// </summary>
        public DbSet<DiscussionParticipants> DiscussionsParticipants { get; set; }

        /// <summary>
        /// The discussion responses.
        /// </summary>
        public DbSet<DiscussionResponse> DiscussionResponses { get; set; }

        /// <summary>
        /// The user.
        /// </summary>
        public DbSet<User> User { get; set; }

        /// <summary>
        /// The document.
        /// </summary>
        public DbSet<Document> Document { get; set; }

        /// <summary>
        /// The organization type.
        /// </summary>
        public DbSet<OrganizationType> OrganizationType { get; set; }
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext" /> class.
        /// </summary>
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        #endregion

        #region [Methods]
        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new DiscussionConfiguration());
            builder.ApplyConfiguration(new ResponseConfiguration());
            builder.ApplyConfiguration(new OrganizationTypeConfiguration());
        }
        #endregion
    }
}
