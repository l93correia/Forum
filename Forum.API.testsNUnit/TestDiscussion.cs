using Forum.API.Data;
using Forum.API.Models;
using Forum.API.TestsNUnit;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Forum.API.testsNUnit
{
    [TestFixture]
    public class TestDiscussion
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
        private IDiscussionRepository _repo;

        /// <summary>
        /// The data context.
        /// </summary>
        private DataContext _dbContext;
        #endregion

        #region [SetUp]
        /// <summary>
        /// The tests setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _dbContext = new InMemoryDbContextFactory().GetDbContext(Guid.NewGuid());
            _repo = new DiscussionRepository(_dbContext);

            var discussions = new List<Discussion>();
            for (int i = 1; i <= 5; i++)
            {
                discussions.Add(CreateDiscussion(i));
            }

            var user = new User
            {
                Id = 1,
                Name = "Peter"
            };
            _dbContext.User.Add(user);
            _dbContext.Discussions.AddRange(discussions);
            _dbContext.SaveChanges();
        }
        #endregion

        #region [Methods] Tests
        /// <summary>
        /// The get by id test.
        /// </summary>
        [Test]
        public void TestGetById()
        {
            var discussionId = 1;
            // Act
            var discussion = _repo.Get(discussionId).Result;

            // Assert
            Assert.AreEqual(discussionId, discussion.Id);
            Assert.AreEqual(string.Format(_subject, discussionId), discussion.Subject);
            Assert.AreEqual(string.Format(_comment, discussionId), discussion.Comment);
        }

        /// <summary>
        /// The get all test.
        /// </summary>
        [Test]
        public void TestGetAll()
        {
            // Act
            var discussions = _repo.GetAll().Result;

            // Assert
            Assert.AreEqual(5, discussions.Count);

            var i = 1;
            foreach (Discussion discussion in discussions)
            {
                Assert.AreEqual(string.Format(_subject, i), discussion.Subject);
                Assert.AreEqual(string.Format(_comment, i), discussion.Comment);
                Assert.AreEqual(_status, discussion.Status);
                i++;
            }
        }

        /// <summary>
        /// The create test.
        /// </summary>
        [Test]
        public void TestCreate()
        {
            var discussionId = 6;
            var discussion = CreateDiscussion(discussionId);

            // Act
            var discussionReturned = _repo.Create(discussion).Result;

            // Assert
            Assert.AreEqual(discussionId, discussionReturned.Id);
            Assert.AreEqual(string.Format(_subject, discussionId), discussionReturned.Subject);
            Assert.AreEqual(string.Format(_comment, discussionId), discussionReturned.Comment);
        }

        /// <summary>
        /// The update test.
        /// </summary>
        [Test]
        public void TestUpdate()
        {
            var discussionId = 1;
            var discussion = _repo.Get(discussionId).Result;

            // Act
            var discussionReturned = _repo.Update(discussion).Result;

            // Assert
            Assert.AreEqual(discussionId, discussionReturned.Id);
            Assert.AreEqual(string.Format(_subject, discussionId), discussionReturned.Subject);
            Assert.AreEqual(string.Format(_comment, discussionId), discussionReturned.Comment);
            Assert.AreEqual("Updated", discussionReturned.Status);
        }

        /// <summary>
        /// The delete test.
        /// </summary>
        [Test]
        public void TestDelete()
        {
            var discussionId = 1;

            // Act
            _repo.Delete(discussionId);

            var discussion = _repo.Get(discussionId).Result;

            // Assert
            Assert.AreEqual(discussionId, discussion.Id);
            Assert.AreEqual(string.Format(_subject, discussionId), discussion.Subject);
            Assert.AreEqual(string.Format(_comment, discussionId), discussion.Comment);
            Assert.AreEqual("Removed", discussion.Status);
        }
        #endregion

        #region [Methods] Utility
        /// <summary>
        /// Create discussion.
        /// </summary>
        /// 
        /// <param name="index">The discussion index.</param>
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
        #endregion
    }
}