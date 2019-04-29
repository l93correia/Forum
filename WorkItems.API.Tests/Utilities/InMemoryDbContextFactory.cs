using Emsa.Mared.ContentManagement.WorkItems.Database;
using Microsoft.EntityFrameworkCore;
using System;

namespace Emsa.Mared.ContentManagement.WorkItems.Tests
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
