using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Discussions.API.Database;
using Emsa.Mared.Discussions.API.Database.Repositories.Attachments;
using Emsa.Mared.Discussions.API.Database.Repositories.Discussions;
using Emsa.Mared.Discussions.API.Database.Repositories.Participants;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Emsa.Mared.Discussions.API.Tests
{
    [TestFixture]
    public class TestAttachment
    {
        #region [Constants]
        /// <summary>
        /// The number of responses.
        /// </summary>
        private readonly long _nAttachments = 5;

        /// <summary>
        /// The discussion id.
        /// </summary>
        private readonly long _discussionId = 1;

        /// <summary>
        /// The entity id.
        /// </summary>
        private readonly long _externalId = 1;

        /// <summary>
        /// The entity type.
        /// </summary>
        private readonly string _url = "https://www";
        #endregion

        #region [Attributes]
        /// <summary>
        /// The repository.
        /// </summary>
        private IAttachmentRepository _repo;

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
            _repo = new AttachmentRepository(_dbContext);

            var attachments = new List<Attachment>();
            for (int i = 1; i <= _nAttachments; i++)
            {
                attachments.Add(CreateAttachment(i));
            }

            var discussion = new Discussion
            {
                Id = _discussionId,
                Subject = "Test Discussion",
                Comment = "Test discussion comment",
                CreatedDate = DateTime.Now,
                Status = "Created",
                UserId = 1
            };

            _dbContext.Discussions.Add(discussion);
            _dbContext.Attachments.AddRange(attachments);
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
            var attachmentId = 1;
            // Act
            var attachment = _repo.GetAsync(attachmentId, membership).Result;

            // Assert
            Assert.AreEqual(attachmentId, attachment.Id);
            Assert.AreEqual(_externalId, attachment.ExternalId);
            Assert.AreEqual(_url, attachment.Url);
            Assert.AreEqual(_discussionId, attachment.DiscussionId);
        }

        /// <summary>
        /// The get by id test, id invalid.
        /// </summary>
        [Test]
        public void TestGetByIdInvalid()
        {
            Attachment attachment = null;
            try
            {
                var membership = CreateMembership();
                attachment = _repo.GetAsync(_nAttachments + 1, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(Attachment.DoesNotExist, modelException.Message);

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
            Attachment attachment = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                attachment = _repo.GetAsync(_nAttachments, membership).Result;
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
            var attachments = _repo.GetByDiscussion(_discussionId, null, membership).Result;

            // Assert
            Assert.AreEqual(_nAttachments, attachments.Count);

            var i = 1;
            foreach (Attachment attachment in attachments)
            {
                Assert.AreEqual(i, attachment.Id);
                Assert.AreEqual(_externalId, attachment.ExternalId);
                Assert.AreEqual(_url, attachment.Url);
                Assert.AreEqual(_discussionId, attachment.DiscussionId);
                i++;
            }
        }

        /// <summary>
        /// The get by discussion id test, id invalid.
        /// </summary>
        [Test]
        public void TestGetByDiscussionIdInvalid()
        {
            List<Attachment> attachments = null;
            try
            {
                var membership = CreateMembership();
                attachments = _repo.GetByDiscussion(_discussionId + 1, null, membership).Result;
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
            List<Attachment> attachments = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                attachments = _repo.GetByDiscussion(_discussionId, null, membership).Result;
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
            var attchaments = _repo.GetAllAsync(null, membership).Result;

            // Assert
            Assert.AreEqual(_nAttachments, attchaments.Count);

            var i = 1;
            foreach (Attachment attachement in attchaments)
            {
                Assert.AreEqual(i, attachement.Id);
                Assert.AreEqual(_externalId, attachement.ExternalId);
                Assert.AreEqual(_url, attachement.Url);
                Assert.AreEqual(1, attachement.DiscussionId);
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
            var attachmentId = _nAttachments + 1;
            var attachment = CreateAttachment(attachmentId);

            // Act
            var discussionReturned = _repo.CreateAsync(attachment, membership).Result;

            // Assert
            Assert.AreEqual(attachmentId, discussionReturned.Id);
            Assert.AreEqual(_externalId, discussionReturned.ExternalId);
            Assert.AreEqual(_url, discussionReturned.Url);
            Assert.AreEqual(_discussionId, discussionReturned.DiscussionId);
        }

        /// <summary>
        /// The create test, discussion id invalid.
        /// </summary>
        [Test]
        public void TestCreateDiscussionIdInvalid()
        {
            Attachment discussionInvalid = null;
            try
            {
                var membership = CreateMembership();
                discussionInvalid = CreateAttachment(_nAttachments);
                discussionInvalid.DiscussionId = _discussionId + 1;
                var responseException = _repo.CreateAsync(discussionInvalid, membership).Result;
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
            Attachment userIdInvalid = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                userIdInvalid = CreateAttachment(_nAttachments);
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
            var response = _repo.GetAsync(_nAttachments, membership).Result;

            // Act
            var discussionReturned = _repo.UpdateAsync(response, membership).Result;

            // Assert
            Assert.AreEqual(_nAttachments, discussionReturned.Id);
            Assert.AreEqual(_externalId, discussionReturned.ExternalId);
            Assert.AreEqual(_url, discussionReturned.Url);
            Assert.AreEqual(_discussionId, discussionReturned.DiscussionId);
        }

        /// <summary>
        /// The update test, user unauthorized.
        /// </summary>
        [Test]
        public void TestUpdateUnauthorized()
        {
            Attachment attachment = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                attachment = CreateAttachment(_nAttachments);
                var attachmentUpdated = _repo.UpdateAsync(attachment, membership).Result;
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
            Attachment attachmentInvalid = null;
            try
            {
                var membership = CreateMembership();
                attachmentInvalid = CreateAttachment(_nAttachments + 1);
                var participantException = _repo.UpdateAsync(attachmentInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(Attachment.DoesNotExist, modelException.Message);

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
            _repo.DeleteAsync(_nAttachments, membership);

            Attachment attachmentException = null;
            try
            {
                attachmentException = _repo.GetAsync(_nAttachments, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(Attachment.DoesNotExist, modelException.Message);

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
            try
            {
                _repo.DeleteAsync(_nAttachments + 1, membership).Wait();
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(Attachment.DoesNotExist, modelException.Message);

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
            try
            {
                _repo.DeleteAsync(_nAttachments, membership).Wait();
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
        /// Create Participant.
        /// </summary>
        /// 
        /// <param name="index">The response index.</param>
        public Attachment CreateAttachment(long? index = 0)
        {
            return new Attachment
            {
                Id = index.Value,
                DiscussionId = _discussionId,
                ExternalId = _externalId,
                Url = _url
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
                GroupIds = new long[0],
                OrganizationsIds = new long[0]
            };
        }
        #endregion
    }
}
