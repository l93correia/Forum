using Forum.API.Models;
using Forum.API.Models.Repository.Discussion;
using Forum.API.Models.Repository.Organization;
using Forum.API.Models.Repository.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Discussions> Discussions { get; set; }
        public DbSet<DiscussionParticipants> DiscussionsParticipants { get; set; }
        public DbSet<DiscussionResponses> DiscussionResponses { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<OrganizationType> OrganizationType { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new DiscussionConfiguration());
            builder.ApplyConfiguration(new ResponseConfiguration());
            builder.ApplyConfiguration(new OrganizationTypeConfiguration());
        }
    }
}
