using Emsa.Mared.WorkItems.API.Database;
using Microsoft.EntityFrameworkCore;
using System;

namespace Emsa.Mared.WorkItems.API.Tests
{
    public class InMemoryDbContextFactory
    {
        public WorkItemsContext GetDbContext(Guid id)
        {
            var options = new DbContextOptionsBuilder<WorkItemsContext>()
                            .UseInMemoryDatabase(databaseName: id.ToString())
                            .Options;
            var dbContext = new WorkItemsContext(options);

            return dbContext;
        }
    }
}
