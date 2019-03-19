using Forum.API.Models;
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

            builder.Entity<OrganizationType>()
                .HasOne(s => s.DiscussionParticipants)
                .WithMany(g => g.OrganizationType)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DiscussionResponses>()
                .HasOne(s => s.Discussion)
                .WithMany(g => g.DiscussionResponses)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
