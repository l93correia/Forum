using Forum.API.Data;
using Forum.API.Models;
using Forum.API.testsNUnit;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestFixture]
    public class Tests
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

        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IDiscussionRepository _repo;

        /// <summary>
        /// The data context.
        /// </summary>
        private readonly DataContext _dbContext;

        public Tests()
        {
            _dbContext = new InMemoryDbContextFactory().GetDbContext();
            _repo = new DiscussionRepository(_dbContext);
        }

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Test1()
        {
            var discussionId = 0;
            var discussion = CreateDiscussion(discussionId);
            // Arrange
            List<Discussion> discussions = new List<Discussion>();
            for (int i = 0; i < 3; i++)
            {
                discussions.Add(CreateDiscussion(i));
            }

            _dbContext.Discussion.AddRangeAsync(discussion);
            _dbContext.Discussion.AddAsync(discussion);
            _dbContext.SaveChangesAsync();

            // Act
            var actual = _repo.Get(discussionId);

            // Assert
            //actual.Id.Should().Be(discussionId);
            actual.Id.Equals(discussionId);
        }

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
    }
}