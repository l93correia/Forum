using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Common.Extensions;
using Emsa.Mared.Discussions.API.Database;
using Emsa.Mared.Discussions.API.Database.Repositories.Discussions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Emsa.Mared.Common.Claims;

namespace Emsa.Mared.Discussions.API.Tests
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

        /// <summary>
        /// The number of discussions.
        /// </summary>
        private readonly long _nDiscussions = 5;

        /// <summary>
        /// The user id.
        /// </summary>
        private readonly long _userId = 1;
        #endregion

        #region [Attributes]
        /// <summary>
        /// The repository.
        /// </summary>
        private IDiscussionRepository _repo;

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
            _repo = new DiscussionRepository(_dbContext);

            var discussions = new List<Discussion>();
            for (int i = 1; i <= _nDiscussions; i++)
            {
                discussions.Add(CreateDiscussion(i));
            }

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
            var membership = CreateMembership();
            // Act
            var discussion = _repo.GetAsync(discussionId, membership).Result;

            // Assert
            Assert.AreEqual(discussionId, discussion.Id);
            Assert.AreEqual(string.Format(_subject, discussionId), discussion.Subject);
            Assert.AreEqual(string.Format(_comment, discussionId), discussion.Comment);
        }

        /// <summary>
        /// The get by id test, user unauthorized.
        /// </summary>
        [Test]
        public void TestGetByIdUnauthorized()
        {
            Discussion discussionException = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                discussionException = _repo.GetAsync(_nDiscussions, membership).Result;
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
        /// The get by id test, id invalid.
        /// </summary>
        [Test]
        public void TestGetByIdInvalid()
        {
            Discussion discussionException = null;
            try
            {
                var membership = CreateMembership();
                discussionException = _repo.GetAsync(_nDiscussions + 1, membership).Result;
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
            var membership = CreateMembership();
            // Act
            var discussions = _repo.GetAllAsync(parameters: null, membership: membership).Result;

            // Assert
            Assert.AreEqual(_nDiscussions, discussions.Count);

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
            var membership = CreateMembership();
            var discussionId = _nDiscussions + 1;
            var discussion = CreateDiscussion(discussionId);

            // Act
            var discussionReturned = _repo.CreateAsync(discussion, membership).Result;

            // Assert
            Assert.AreEqual(discussionId, discussionReturned.Id);
            Assert.AreEqual(string.Format(_subject, discussionId), discussionReturned.Subject);
            Assert.AreEqual(string.Format(_comment, discussionId), discussionReturned.Comment);
        }

        /// <summary>
        /// The create test, subject invalid.
        /// </summary>
        [Test]
        public void TestCreateSubjectInvalid()
        {
            Discussion subjectInvalid = null;
            try
            {
                var membership = CreateMembership();
                subjectInvalid = CreateDiscussion(_nDiscussions);
                subjectInvalid.Subject = "";
                var discussionException = _repo.CreateAsync(subjectInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException1)
                {
                    Assert.AreEqual(subjectInvalid.InvalidFieldMessage(p => p.Subject), modelException1.Message);

                    return;
                }
            }
            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
        }

        /// <summary>
        /// The create test, comment invalid.
        /// </summary>
        [Test]
        public void TestCreateCommentInvalid()
        {
            Discussion commentInvalid = null;
            try
            {
                var membership = CreateMembership();
                commentInvalid = CreateDiscussion(_nDiscussions);
                commentInvalid.Comment = "";
                var discussionException = _repo.CreateAsync(commentInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException1)
                {
                    Assert.AreEqual(commentInvalid.InvalidFieldMessage(p => p.Comment), modelException1.Message);

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
            var discussionId = 1;
            var discussion = _repo.GetAsync(discussionId, membership).Result;

            // Act
            var discussionReturned = _repo.UpdateAsync(discussion, membership).Result;

            // Assert
            Assert.AreEqual(discussionId, discussionReturned.Id);
            Assert.AreEqual(string.Format(_subject, discussionId), discussionReturned.Subject);
            Assert.AreEqual(string.Format(_comment, discussionId), discussionReturned.Comment);
            Assert.AreEqual("Updated", discussionReturned.Status);
        }

        /// <summary>
        /// The update test, id not found.
        /// </summary>
        [Test]
        public void TestUpdateIdInvalid()
        {
            // Test exception id not found
            try
            {
                var membership = CreateMembership();
                var disc = CreateDiscussion(_nDiscussions + 1);
                var discussionException = _repo.UpdateAsync(disc, membership).Result;
            }
            catch (AggregateException execption)
            {
                if (execption.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(Discussion.DoesNotExist, modelException.Message);

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
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                var disc = CreateDiscussion(_nDiscussions);
                var discussionException = _repo.UpdateAsync(disc, membership).Result;
            }
            catch (AggregateException execption)
            {
                if (execption.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(true, modelException.UnauthorizedResource);

                    return;
                }
            }

            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
        }

        /// <summary>
        /// The update test, subject invalid.
        /// </summary>
        [Test]
        public void TestUpdateSubjectInvalid()
        {
            // Test exception subject empty
            Discussion subjectInvalid = null;
            try
            {
                var membership = CreateMembership();
                subjectInvalid = CreateDiscussion(_nDiscussions);
                subjectInvalid.Subject = "";
                var discussionException = _repo.UpdateAsync(subjectInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException1)
                {
                    Assert.AreEqual(subjectInvalid.InvalidFieldMessage(p => p.Subject), modelException1.Message);

                    return;
                }
            }
            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
        }

        /// <summary>
        /// The update test, comment invalid.
        /// </summary>
        [Test]
        public void TestUpdateCommentInvalid()
        {
            // Test exception comment empty
            Discussion commentInvalid = null;
            try
            {
                var membership = CreateMembership();
                commentInvalid = CreateDiscussion(_nDiscussions);
                commentInvalid.Comment = "";
                var discussionException = _repo.UpdateAsync(commentInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(commentInvalid.InvalidFieldMessage(p => p.Comment), modelException.Message);

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
            var discussionId = 1;

            // Act
            _repo.DeleteAsync(discussionId, membership);

            Discussion discussionException = null;
            try
            {
                discussionException = _repo.GetAsync(discussionId, membership).Result;
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
        /// The delete test, discussion not found.
        /// </summary>
        [Test]
        public void TestDeleteNotFound()
        {
            // Test exception
            try
            {
                var membership = CreateMembership();
                _repo.DeleteAsync(_nDiscussions + 1, membership).Wait();
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
        /// The delete test, user unauthorized.
        /// </summary>
        [Test]
        public void TestDeleteUnauthorized()
        {
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                _repo.DeleteAsync(_nDiscussions, membership).Wait();
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