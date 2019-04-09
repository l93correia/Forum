using Emsa.Mared.Discussions.API.Database;
using Microsoft.EntityFrameworkCore;
using System;

namespace Emsa.Mared.Discussions.API.Tests
{
    public class InMemoryDbContextFactory
    {
        public DiscussionContext GetDbContext(Guid id)
        {
            var options = new DbContextOptionsBuilder<DiscussionContext>()
                            .UseInMemoryDatabase(databaseName: id.ToString())
                            .Options;
            var dbContext = new DiscussionContext(options);

            return dbContext;
        }
    }
}
