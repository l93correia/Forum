using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Security;
using Emsa.Mared.WorkItems.API.Database;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemAttachments;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using Emsa.Mared.WorkItems.API.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Emsa.Mared.WorkItems.API.Tests
{
	[TestFixture]
    public class TestAttachment
    {
        #region [Constants]
        /// <summary>
        /// The number of responses.
        /// </summary>
        private readonly long AttachmentsCount = 5;

        /// <summary>
        /// The discussion id.
        /// </summary>
        private readonly long WorkItemId = 1;

        /// <summary>
        /// The entity id.
        /// </summary>
        private readonly long ExternalId = 1;

        /// <summary>
        /// The entity type.
        /// </summary>
        private readonly string Url = "https://www";
        #endregion

        #region [Attributes]
        /// <summary>
        /// The repository.
        /// </summary>
        private IWorkItemAttachmentRepository AttachmentRepository;

        /// <summary>
        /// The data context.
        /// </summary>
        private WorkItemsContext Context;
        #endregion

        #region [SetUp]
        /// <summary>
        /// The tests setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Context = new InMemoryDbContextFactory().GetDbContext(Guid.NewGuid());
            var workItemRepository = new WorkItemRepository(Context);
            AttachmentRepository = new WorkItemAttachmentRepository(Context, workItemRepository);

            var attachments = new List<WorkItemAttachment>();
            for (int i = 1; i <= AttachmentsCount; i++)
            {
                attachments.Add(CreateAttachment(i));
            }

            var workItem = new WorkItem
            {
                Id = WorkItemId,
                Title = "Test Discussion",
                Summary = "Test discussion summary",
                Body = "Test discussion body",
				Type = WorkItemType.Discussion,
				Status = Status.Created,
				UserId = 1,
				WorkItemParticipants = new List<WorkItemParticipant>
				{
					new WorkItemParticipant
					{
						EntityId = 1,
						EntityType = EntityType.User
					}
				}
			};

            Context.WorkItems.Add(workItem);
            Context.WorkItemAttachments.AddRange(attachments);
            Context.SaveChanges();
        }
        #endregion

        #region [Methods] Tests
        /// <summary>
        /// The get by id test.
        /// </summary>
        [Test]
        public void TestGetById()
        {
            var membership = this.CreateMembership();
            var attachmentId = 1;
            // Act
            var attachment = this.AttachmentRepository.GetAsync(attachmentId, membership).Result;

            // Assert
            Assert.AreEqual(attachmentId, attachment.Id);
            Assert.AreEqual(this.ExternalId, attachment.ExternalId);
            Assert.AreEqual(this.Url, attachment.Url);
            Assert.AreEqual(this.WorkItemId, attachment.WorkItemId);
        }

        /// <summary>
        /// The get by id test, id invalid.
        /// </summary>
        [Test]
        public void TestGetByIdInvalid()
        {
            WorkItemAttachment attachment = null;
            try
            {
                var membership = this.CreateMembership();
                attachment = this.AttachmentRepository.GetAsync(this.AttachmentsCount + 1, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemAttachment.DoesNotExist, modelException.Message);

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
			WorkItemAttachment attachment = null;
            try
            {
                var membership = this.CreateMembership();
                membership.UserId = 2;
                attachment = this.AttachmentRepository.GetAsync(this.AttachmentsCount, membership).Result;
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
			var parameters = new WorkItemAttachmentParameters
			{
				workItemId = this.WorkItemId
			};
            var membership = this.CreateMembership();
            // Act
            var attachaments = this.AttachmentRepository.GetAllAsync(parameters, membership).Result;

            // Assert
            Assert.AreEqual(this.AttachmentsCount, attachaments.Count);

            var i = 1;
            foreach (WorkItemAttachment attachement in attachaments)
            {
                Assert.AreEqual(i, attachement.Id);
                Assert.AreEqual(this.ExternalId, attachement.ExternalId);
                Assert.AreEqual(this.Url, attachement.Url);
                Assert.AreEqual(this.WorkItemId, attachement.WorkItemId);
                i++;
            }
        }

        /// <summary>
        /// The create test.
        /// </summary>
        [Test]
        public void TestCreate()
        {
            var membership = this.CreateMembership();
            var attachmentId = this.AttachmentsCount + 1;
            var attachment = this.CreateAttachment(attachmentId);

            // Act
            var attachmentReturned = this.AttachmentRepository.CreateAsync(attachment, membership).Result;

            // Assert
            Assert.AreEqual(attachmentId, attachmentReturned.Id);
            Assert.AreEqual(this.ExternalId, attachmentReturned.ExternalId);
            Assert.AreEqual(this.Url, attachmentReturned.Url);
            Assert.AreEqual(this.WorkItemId, attachmentReturned.WorkItemId);
        }

        /// <summary>
        /// The create test, work item id invalid.
        /// </summary>
        [Test]
        public void TestCreateWorkItemIdInvalid()
        {
			WorkItemAttachment workItemInvalid = null;
            try
            {
                var membership = this.CreateMembership();
                workItemInvalid = this.CreateAttachment(AttachmentsCount);
                workItemInvalid.WorkItemId = WorkItemId + 1;
                var responseException = this.AttachmentRepository.CreateAsync(workItemInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItem.DoesNotExist, modelException.Message);

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
			WorkItemAttachment userIdInvalid = null;
            try
            {
                var membership = this.CreateMembership();
                membership.UserId = 2;
                userIdInvalid = this.CreateAttachment(AttachmentsCount);
                var attachmentException = this.AttachmentRepository.CreateAsync(userIdInvalid, membership).Result;
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
            var attachment = AttachmentRepository.GetAsync(AttachmentsCount, membership).Result;
			attachment.Url = "updated";
            // Act
            var attchmentReturned = AttachmentRepository.UpdateAsync(attachment, membership).Result;

            // Assert
            Assert.AreEqual(this.AttachmentsCount, attchmentReturned.Id);
            Assert.AreEqual(this.ExternalId, attchmentReturned.ExternalId);
            Assert.AreEqual("updated", attchmentReturned.Url);
            Assert.AreEqual(this.WorkItemId, attchmentReturned.WorkItemId);
        }

        /// <summary>
        /// The update test, user unauthorized.
        /// </summary>
        [Test]
        public void TestUpdateUnauthorized()
        {
			WorkItemAttachment attachment = null;
            try
            {
                var membership = this.CreateMembership();
                membership.UserId = 2;
                attachment = this.CreateAttachment(this.AttachmentsCount);
                var attachmentUpdated = this.AttachmentRepository.UpdateAsync(attachment, membership).Result;
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
			WorkItemAttachment attachmentInvalid = null;
            try
            {
                var membership = this.CreateMembership();
                attachmentInvalid = this.CreateAttachment(this.AttachmentsCount + 1);
                var attachmentException = this.AttachmentRepository.UpdateAsync(attachmentInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemAttachment.DoesNotExist, modelException.Message);

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
			this.AttachmentRepository.DeleteAsync(this.AttachmentsCount, membership);

			WorkItemAttachment attachmentException = null;
            try
            {
                attachmentException = this.AttachmentRepository.GetAsync(this.AttachmentsCount, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemAttachment.DoesNotExist, modelException.Message);

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
            var membership = this.CreateMembership();
            try
            {
				this.AttachmentRepository.DeleteAsync(this.AttachmentsCount + 1, membership).Wait();
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemAttachment.DoesNotExist, modelException.Message);

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
            var membership = this.CreateMembership();
            membership.UserId = 2;
            try
            {
				this.AttachmentRepository.DeleteAsync(this.AttachmentsCount, membership).Wait();
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
        /// Create Attchment.
        /// </summary>
        /// 
        /// <param name="index">The attachment index.</param>
        public WorkItemAttachment CreateAttachment(long? index = 0)
        {
            return new WorkItemAttachment
			{
                Id = index.Value,
                WorkItemId = this.WorkItemId,
                ExternalId = this.ExternalId,
                Url = this.Url,
				UserId = 1
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
