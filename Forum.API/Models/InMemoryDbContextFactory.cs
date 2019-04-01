using Forum.API.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Forum.API.testsNUnit
{
    public class InMemoryDbContextFactory
    {
        public DataContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
                            .Options;
            var dbContext = new DataContext(options);

            return dbContext;
        }
    }
}
