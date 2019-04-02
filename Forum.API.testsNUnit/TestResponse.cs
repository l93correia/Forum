using Forum.API.Data;
using Forum.API.Models;
using Forum.API.TestsNUnit;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Forum.API.testsNUnit
{
    [TestFixture]
    public class TestResponse
    {
        #region [Constants]
        /// <summary>
        /// The subject to test.
        /// </summary>
        private readonly string _response = "Response {0}";

        /// <summary>
        /// The status to test.
        /// </summary>
        private readonly string _status = "Created";
        #endregion

        #region [Attributes]
        /// <summary>
        /// The repository.
        /// </summary>
        private IResponseRepository _repo;

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
            _repo = new ResponseRepository(_dbContext);

            var responses = new List<DiscussionResponse>();
            for (int i = 1; i <= 5; i++)
            {
                responses.Add(CreateResponse(i));
            }

            var user = new User
            {
                Id = 1,
                Name = "Peter"
            };

            var discussion = new Discussion
            {
                Id = 1,
                Subject = "Test Discussion",
                Comment = "Test discussion comment",
                CreatedDate = DateTime.Now,
                Status = _status,
                UserId = 1
            };

            _dbContext.User.Add(user);
            _dbContext.Discussions.Add(discussion);
            _dbContext.DiscussionResponses.AddRange(responses);
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
            var responseId = 1;
            // Act
            var response = _repo.Get(responseId).Result;

            // Assert
            Assert.AreEqual(responseId, response.Id);
            Assert.AreEqual(string.Format(_response, responseId), response.Response);
            Assert.AreEqual(_status, response.Status);
            Assert.AreEqual(1, response.CreatedById);
            Assert.AreEqual(1, response.DiscussionId);
        }

        /// <summary>
        /// The get all test.
        /// </summary>
        [Test]
        public void TestGetAll()
        {
            // Act
            var responses = _repo.GetAll().Result;

            // Assert
            Assert.AreEqual(5, responses.Count);

            var i = 1;
            foreach (DiscussionResponse response in responses)
            {
                Assert.AreEqual(string.Format(_response, i), response.Response);
                Assert.AreEqual(_status, response.Status);
                Assert.AreEqual(1, response.DiscussionId);
                Assert.AreEqual(1, response.CreatedById);
                i++;
            }
        }

        /// <summary>
        /// The create test.
        /// </summary>
        [Test]
        public void TestCreate()
        {
            var responseId = 6;
            var response = CreateResponse(responseId);

            // Act
            var discussionReturned = _repo.Create(response).Result;

            // Assert
            Assert.AreEqual(responseId, discussionReturned.Id);
            Assert.AreEqual(string.Format(_response, responseId), discussionReturned.Response);
            Assert.AreEqual(_status, discussionReturned.Status);
        }

        /// <summary>
        /// The update test.
        /// </summary>
        [Test]
        public void TestUpdate()
        {
            var ResponseId = 1;
            var response = _repo.Get(ResponseId).Result;

            // Act
            var discussionReturned = _repo.Update(response).Result;

            // Assert
            Assert.AreEqual(ResponseId, discussionReturned.Id);
            Assert.AreEqual(string.Format(_response, ResponseId), discussionReturned.Response);
            Assert.AreEqual("Updated", discussionReturned.Status);
        }

        /// <summary>
        /// The delete test.
        /// </summary>
        [Test]
        public void TestDelete()
        {
            var responseId = 1;

            // Act
            _repo.Delete(responseId);

            var response = _repo.Get(responseId).Result;

            // Assert
            Assert.AreEqual(responseId, response.Id);
            Assert.AreEqual(string.Format(_response, responseId), response.Response);
            Assert.AreEqual("Removed", response.Status);
        }
        #endregion

        #region [Methods] Utility
        /// <summary>
        /// Create response.
        /// </summary>
        /// 
        /// <param name="index">The response index.</param>
        public DiscussionResponse CreateResponse(long? index = 0)
        {
            return new DiscussionResponse
            {
                Id = index.Value,
                Response = string.Format(_response, index),
                CreatedDate = DateTime.Now,
                Status = _status,
                DiscussionId = 1,
                CreatedById = 1
            };

        }
        #endregion
    }
}
