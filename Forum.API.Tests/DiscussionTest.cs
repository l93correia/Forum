using AutoMapper;
using Forum.API.Controllers;
using Forum.API.Data;
using Forum.API.Dtos;
using Forum.API.Helpers;
using Forum.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Forum.API.Tests
{
    [TestClass]
    public class DiscussionTest
    {
        #region [Constants]
        /// <summary>
        /// The subject to test.
        /// </summary>
        private readonly string _subject = "Test {0}";

        /// <summary>
        /// The comment to test.
        /// </summary>
        private readonly string _comment = "Test comment {0}";

        /// <summary>
        /// The status to test.
        /// </summary>
        private readonly string _status = "Created";
        #endregion

        #region [Attributes]
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IDiscussionRepository _repo;

        /// <summary>
        /// The data context.
        /// </summary>
        private readonly DataContext _dbContext;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscussionTest"/> class.
        /// </summary>
        /// 
        public DiscussionTest()
        {
            //_dbContext = new InMemoryDbContextFactory().GetDbContext();
            //_repo = new DiscussionRepository(_dbContext);
        }
        #endregion

        //[AssemblyInitialize]
        //public static void Initialize(TestContext context)
        //{
        //    Mapper.Initialize(m => m.AddProfile<AutoMapperProfile>());
        //}

        public Discussion CreateDiscussion(long? index = 0)
        {
            return new Discussion
            {
                Id = index.Value,
                Subject = string.Format(_subject, index),
                Comment = string.Format(_comment, index),
                CreatedDate = DateTime.Now,
                Status = _status,
                UserId = 1
            };

        }

        [TestMethod]
        public void TestGetById()
        {
            // Arrange - We're mocking our dbSet & dbContext
            // in-memory data
            IQueryable<Discussion> discussions = new List<Discussion>
            {
                new Discussion
                {
                    Id = 1,
                    Subject = "Subject 1",
                    Comment = "Comment 1",
                    Status = "Created",
                    UserId = 1
                },
                new Discussion
                {
                    Id = 2,
                    Subject = "Subject 2",
                    Comment = "Comment 2",
                    Status = "Created",
                    UserId = 1
                }

            }.AsQueryable();

            // To query our database we need to implement IQueryable 
            var mockSet = new Mock<DbSet<Discussion>>();
            mockSet.As<IQueryable<Discussion>>().Setup(m => m.Provider).Returns(discussions.Provider);
            mockSet.As<IQueryable<Discussion>>().Setup(m => m.Expression).Returns(discussions.Expression);
            mockSet.As<IQueryable<Discussion>>().Setup(m => m.ElementType).Returns(discussions.ElementType);
            mockSet.As<IQueryable<Discussion>>().Setup(m => m.GetEnumerator()).Returns(discussions.GetEnumerator());

            var mockContext = new Mock<DataContext>();
            mockContext.Setup(c => c.Discussion).Returns(mockSet.Object);

            // Act - fetch books
            var repository = new DiscussionRepository(mockContext.Object);

            Task.Run(async () =>
            {
                var actual = await repository.GetAll();
                Assert.AreEqual(2, actual.Count());

            }).GetAwaiter().GetResult();

            // Asset
            // Ensure that 2 books are returned and
            // the first one's title is "Hamlet"
            
            //Assert.AreEqual("Hamlet", actual.First().Title);
            
        }

        //[TestMethod]
        //public void TestGetAll()
        //{
            
        //}

        //[TestMethod]
        //public void TestCreateDiscussion()
        //{

        //}

        //[TestMethod]
        //public void TestUpdateDiscussion()
        //{
            
        //}

        //[TestMethod]
        //public void TestDeleteDiscussion()
        //{

        //}
    }
}
