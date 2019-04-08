using Emsa.Mared.Common;
using Emsa.Mared.Discussions.API.Database;
using Emsa.Mared.Discussions.API.Database.Repositories.Responses;
using Emsa.Mared.Discussions.API.Database.Repositories.Users;
using Emsa.Mared.Discussions.API.Database.Repository;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Emsa.Mared.Discussions.API.Tests
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

        /// <summary>
        /// The number of responses.
        /// </summary>
        private readonly long _nResponses = 5;

        /// <summary>
        /// The discussion id.
        /// </summary>
        private readonly long _discussionId = 1;

        /// <summary>
        /// The user id.
        /// </summary>
        private readonly long _userId = 1;
        #endregion

        #region [Attributes]
        /// <summary>
        /// The repository.
        /// </summary>
        private IResponseRepository _repo;

        /// <summary>
        /// The data context.
        /// </summary>
        private DiscussionContext _dbContext;
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

            var responses = new List<Response>();
            for (int i = 1; i <= _nResponses; i++)
            {
                responses.Add(CreateResponse(i));
            }

            var user = new User
            {
                Id = _userId,
                Name = "Peter"
            };

            var discussion = new Discussion
            {
                Id = _discussionId,
                Subject = "Test Discussion",
                Comment = "Test discussion comment",
                CreatedDate = DateTime.Now,
                Status = _status,
                UserId = 1
            };

            _dbContext.User.Add(user);
            _dbContext.Discussions.Add(discussion);
            _dbContext.Responses.AddRange(responses);
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
            Assert.AreEqual(string.Format(_response, responseId), response.Comment);
            Assert.AreEqual(_status, response.Status);
            Assert.AreEqual(1, response.UserId);
            Assert.AreEqual(1, response.DiscussionId);
        }

        /// <summary>
        /// The get by id test, id invalid.
        /// </summary>
        [Test]
        public void TestGetByIdInvalid()
        {
            Response responseException = null;
            try
            {
                responseException = _repo.Get(_nResponses + 1).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(Response.DoesNotExist, modelException.Message);

                    return;
                }
            }

            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
        }

        /// <summary>
        /// The get by discussion id test.
        /// </summary>
        [Test]
        public void TestGetByDiscussionId()
        {
            var discussionId = _discussionId;
            // Act
            var responses = _repo.GetByDiscussion(discussionId).Result;

            // Assert
            Assert.AreEqual(_nResponses, responses.Count);

            var i = 1;
            foreach (Response response in responses)
            {
                Assert.AreEqual(string.Format(_response, i), response.Comment);
                Assert.AreEqual(_status, response.Status);
                Assert.AreEqual(1, response.DiscussionId);
                Assert.AreEqual(1, response.UserId);
                i++;
            }
        }

        /// <summary>
        /// The get by discussion id test, id invalid.
        /// </summary>
        [Test]
        public void TestGetByDiscussionIdInvalid()
        {
            List<Response> responseException = null;
            try
            {
                responseException = _repo.GetByDiscussion(_discussionId + 1).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(Discussion.DoesNotExist, modelException.Message);

                    return;
                }
            }

            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
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
            Assert.AreEqual(_nResponses, responses.Count);

            var i = 1;
            foreach (Response response in responses)
            {
                Assert.AreEqual(string.Format(_response, i), response.Comment);
                Assert.AreEqual(_status, response.Status);
                Assert.AreEqual(1, response.DiscussionId);
                Assert.AreEqual(1, response.UserId);
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
            Assert.AreEqual(string.Format(_response, responseId), discussionReturned.Comment);
            Assert.AreEqual(_status, discussionReturned.Status);
        }

        /// <summary>
        /// The create test, response invalid.
        /// </summary>
        [Test]
        public void TestCreateResponseInvalid()
        {
            Response responseInvalid = null;
            try
            {
                responseInvalid = CreateResponse(_nResponses);
                responseInvalid.Comment = "";
                var responseException = _repo.Create(responseInvalid).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException1)
                {
                    Assert.AreEqual(responseInvalid.InvalidFieldMessage(p => p.Comment), modelException1.Message);

                    return;
                }
            }
            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
        }

        /// <summary>
        /// The create test, user id invalid.
        /// </summary>
        [Test]
        public void TestCreateUserIdInvalid()
        {
            Response userIdInvalid = null;
            try
            {
                userIdInvalid = CreateResponse(_nResponses);
                userIdInvalid.UserId = _userId + 1;
                var responseException = _repo.Create(userIdInvalid).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException1)
                {
                    Assert.AreEqual(User.DoesNotExist, modelException1.Message);

                    return;
                }
            }
            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
        }

        /// <summary>
        /// The create test, discussion id invalid.
        /// </summary>
        [Test]
        public void TestCreateDiscussionIdInvalid()
        {
            Response userIdInvalid = null;
            try
            {
                userIdInvalid = CreateResponse(_nResponses);
                userIdInvalid.DiscussionId = _discussionId + 1;
                var responseException = _repo.Create(userIdInvalid).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException1)
                {
                    Assert.AreEqual(Discussion.DoesNotExist, modelException1.Message);

                    return;
                }
            }
            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
        }

        /// <summary>
        /// The update test.
        /// </summary>
        [Test]
        public void TestUpdate()
        {
            var response = _repo.Get(_nResponses).Result;

            // Act
            var discussionReturned = _repo.Update(response).Result;

            // Assert
            Assert.AreEqual(_nResponses, discussionReturned.Id);
            Assert.AreEqual(string.Format(_response, _nResponses), discussionReturned.Comment);
            Assert.AreEqual("Updated", discussionReturned.Status);
        }

        /// <summary>
        /// The update test, response invalid.
        /// </summary>
        [Test]
        public void TestUpdateResponseInvalid()
        {
            // Test exception comment empty
            Response responseInvalid = null;
            try
            {
                responseInvalid = CreateResponse(_nResponses);
                responseInvalid.Comment = "";
                var responseException = _repo.Update(responseInvalid).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(responseInvalid.InvalidFieldMessage(p => p.Comment), modelException.Message);

                    return;
                }
            }
            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
        }

        /// <summary>
        /// The delete test.
        /// </summary>
        [Test]
        public void TestDelete()
        {
            // Act
            _repo.Delete(_nResponses);

            Response responseException = null;
            try
            {
                responseException = _repo.Get(_nResponses).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(Response.DoesNotExist, modelException.Message);

                    return;
                }
            }

            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
        }

        /// <summary>
        /// The delete test.
        /// </summary>
        [Test]
        public void TestDeleteNotFound()
        {
            // Test exception
            try
            {
                _repo.Delete(_nResponses + 1).Wait();
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(Response.DoesNotExist, modelException.Message);

                    return;
                }
            }

            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
        }
        #endregion

        #region [Methods] Utility
        /// <summary>
        /// Create response.
        /// </summary>
        /// 
        /// <param name="index">The response index.</param>
        public Response CreateResponse(long? index = 0)
        {
            return new Response
            {
                Id = index.Value,
                Comment = string.Format(_response, index),
                CreatedDate = DateTime.Now,
                Status = _status,
                DiscussionId = _discussionId,
                UserId = _userId
            };

        }
        #endregion
    }
}
