using Emsa.Mared.Common;
using Emsa.Mared.Discussions.API.Database;
using Emsa.Mared.Discussions.API.Database.Repositories;
using Emsa.Mared.Discussions.API.Database.Repositories.Discussions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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
            // Act
            var discussion = _repo.Get(discussionId).Result;

            // Assert
            Assert.AreEqual(discussionId, discussion.Id);
            Assert.AreEqual(string.Format(_subject, discussionId), discussion.Subject);
            Assert.AreEqual(string.Format(_comment, discussionId), discussion.Comment);
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
                discussionException = _repo.Get(_nDiscussions + 1).Result;
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
            var discussions = _repo.GetAll().Result;

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
        /// The create test, subject invalid.
        /// </summary>
        [Test]
        public void TestCreateSubjectInvalid()
        {
            Discussion subjectInvalid = null;
            try
            {
                subjectInvalid = CreateDiscussion(_nDiscussions);
                subjectInvalid.Subject = "";
                var discussionException = _repo.Create(subjectInvalid).Result;
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
                commentInvalid = CreateDiscussion(_nDiscussions);
                commentInvalid.Comment = "";
                var discussionException = _repo.Create(commentInvalid).Result;
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
        /// The create test, UserId invalid.
        /// </summary>
        [Test]
        public void TestCreateUserIdInvalid()
        {
            Discussion userIdInvalid = null;
            try
            {
                userIdInvalid = CreateDiscussion(_nDiscussions);
                userIdInvalid.UserId = _userId + 1;
                var discussionException = _repo.Create(userIdInvalid).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException1)
                {
                    //Assert.AreEqual(User.DoesNotExist, modelException1.Message);

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
        /// The update test, id not found.
        /// </summary>
        [Test]
        public void TestUpdateIdInvalid()
        {
            // Test exception id not found
            try
            {
                var disc = CreateDiscussion(_nDiscussions + 1);
                var discussionException = _repo.Update(disc).Result;
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
        /// The update test, subject invalid.
        /// </summary>
        [Test]
        public void TestUpdateSubjectInvalid()
        {
            // Test exception subject empty
            Discussion subjectInvalid = null;
            try
            {
                subjectInvalid = CreateDiscussion(_nDiscussions);
                subjectInvalid.Subject = "";
                var discussionException = _repo.Update(subjectInvalid).Result;
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
                commentInvalid = CreateDiscussion(_nDiscussions);
                commentInvalid.Comment = "";
                var discussionException = _repo.Update(commentInvalid).Result;
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
            var discussionId = 1;

            // Act
            _repo.Delete(discussionId);

            Discussion discussionException = null;
            try
            {
                discussionException = _repo.Get(discussionId).Result;
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
                _repo.Delete(_nDiscussions + 1).Wait();
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
        #endregion
    }
}