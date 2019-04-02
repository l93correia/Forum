using Forum.API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.API.TestsNUnit
{
    public class InMemoryDbContextFactory
    {
        public DataContext GetDbContext(Guid id)
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                            .UseInMemoryDatabase(databaseName: id.ToString())
                            .Options;
            var dbContext = new DataContext(options);

            return dbContext;
        }
    }
}
