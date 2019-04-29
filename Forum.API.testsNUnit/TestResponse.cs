using Emsa.Mared.Common;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Common.Extensions;
using Emsa.Mared.Discussions.API.Database;
using Emsa.Mared.Discussions.API.Database.Repositories;
using Emsa.Mared.Discussions.API.Database.Repositories.Discussions;
using Emsa.Mared.Discussions.API.Database.Repositories.Responses;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Emsa.Mared.Common.Claims;

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
            var _repoDiscussion = new DiscussionRepository(_dbContext);
            _repo = new ResponseRepository(_dbContext, _repoDiscussion);

            var responses = new List<Response>();
            for (int i = 1; i <= _nResponses; i++)
            {
                responses.Add(CreateResponse(i));
            }

            var discussion = new Discussion
            {
                Id = _discussionId,
                Subject = "Test Discussion",
                Comment = "Test discussion comment",
                CreatedDate = DateTime.Now,
                Status = _status,
                UserId = 1
            };

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
            var membership = CreateMembership();
            var responseId = 1;
            // Act
            var response = _repo.GetAsync(responseId, membership).Result;

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
                var membership = CreateMembership();
                responseException = _repo.GetAsync(_nResponses + 1, membership).Result;
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
        /// The get by id test, user unauthorized.
        /// </summary>
        [Test]
        public void TestGetByUnauthorized()
        {
            Response responseException = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                responseException = _repo.GetAsync(_nResponses, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(true, modelException.UnauthorizedResource);

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
            var membership = CreateMembership();
            // Act
            var responses = _repo.GetByDiscussion(_discussionId, null, membership).Result;

            // Assert
            Assert.AreEqual(_nResponses, responses.Count);

            var i = 1;
            foreach (Response response in responses)
            {
                Assert.AreEqual(string.Format(_response, i), response.Comment);
                Assert.AreEqual(_status, response.Status);
                Assert.AreEqual(_discussionId, response.DiscussionId);
                Assert.AreEqual(_userId, response.UserId);
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
                var membership = CreateMembership();
                responseException = _repo.GetByDiscussion(_discussionId + 1, null, membership).Result;
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
        /// The get by discussion id test, user unauthorized.
        /// </summary>
        [Test]
        public void TestGetByDiscussionUnauthorized()
        {
            List<Response> responseException = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                responseException = _repo.GetByDiscussion(_discussionId, null, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(true, modelException.UnauthorizedResource);

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
            var membership = CreateMembership();
            // Act
            var responses = _repo.GetAllAsync(null, membership).Result;

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
            var membership = CreateMembership();
            var responseId = _nResponses + 1;
            var response = CreateResponse(responseId);

            // Act
            var discussionReturned = _repo.CreateAsync(response, membership).Result;

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
                var membership = CreateMembership();
                responseInvalid = CreateResponse(_nResponses);
                responseInvalid.Comment = "";
                var responseException = _repo.CreateAsync(responseInvalid, membership).Result;
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
        /// The create test, discussion id invalid.
        /// </summary>
        [Test]
        public void TestCreateDiscussionIdInvalid()
        {
            Response userIdInvalid = null;
            try
            {
                var membership = CreateMembership();
                userIdInvalid = CreateResponse(_nResponses);
                userIdInvalid.DiscussionId = _discussionId + 1;
                var responseException = _repo.CreateAsync(userIdInvalid, membership).Result;
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
        /// The create test, user unauthorized.
        /// </summary>
        [Test]
        public void TestCreateUnauthorized()
        {
            Response userIdInvalid = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                userIdInvalid = CreateResponse(_nResponses);
                var responseException = _repo.CreateAsync(userIdInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(true, modelException.UnauthorizedResource);

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
            var membership = CreateMembership();
            var response = _repo.GetAsync(_nResponses, membership).Result;

            // Act
            var discussionReturned = _repo.UpdateAsync(response, membership).Result;

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
                var membership = CreateMembership();
                responseInvalid = CreateResponse(_nResponses);
                responseInvalid.Comment = "";
                var responseException = _repo.UpdateAsync(responseInvalid, membership).Result;
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
        /// The update test, user unauthorized.
        /// </summary>
        [Test]
        public void TestUpdateUnauthorized()
        {
            Response response = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                response = CreateResponse(_nResponses);
                var responseException = _repo.UpdateAsync(response, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(true, modelException.UnauthorizedResource);

                    return;
                }
            }
            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
        }

        /// <summary>
        /// The update test, not found.
        /// </summary>
        [Test]
        public void TestUpdateNotFound()
        {
            Response responseInvalid = null;
            try
            {
                var membership = CreateMembership();
                responseInvalid = CreateResponse(_nResponses + 1);
                var responseException = _repo.UpdateAsync(responseInvalid, membership).Result;
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
        public void TestDelete()
        {
            var membership = CreateMembership();
            // Act
            _repo.DeleteAsync(_nResponses, membership);

            Response responseException = null;
            try
            {
                responseException = _repo.GetAsync(_nResponses, membership).Result;
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
        /// The delete test, not found.
        /// </summary>
        [Test]
        public void TestDeleteNotFound()
        {
            var membership = CreateMembership();
            // Test exception
            try
            {
                _repo.DeleteAsync(_nResponses + 1, membership).Wait();
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
        /// The delete test, user unauthorized.
        /// </summary>
        [Test]
        public void TestDeleteUnauthorized()
        {
            var membership = CreateMembership();
            membership.UserId = 2;
            // Test exception
            try
            {
                _repo.DeleteAsync(_nResponses, membership).Wait();
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(true, modelException.UnauthorizedResource);

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

        /// <summary>
		/// Create a membership.
		/// </summary>
		public UserMembership CreateMembership()
        {
            long userId = 1;
            long[] groupsIds = new long[0];
            long[] organizationIds = new long[0];

            return new UserMembership
			{
				UserId = userId,
				Groups = new GroupClaimType[0],
				Organizations = new OrganizationClaimType[0]
			};
        }
        #endregion
    }
}
