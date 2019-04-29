using Emsa.Mared.Common.Claims;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Security;
using Emsa.Mared.ContentManagement.WorkItems.Database;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemAttachments;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.ContentManagement.WorkItems.Database.Repositories.WorkItems;
using Emsa.Mared.ContentManagement.WorkItems.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emsa.Mared.ContentManagement.WorkItems.Tests
{
	[TestFixture]
    public class TestAttachment
    {
        #region [Constants]
        /// <summary>
        /// The number of attachments.
        /// </summary>
        private readonly long AttachmentsCount = 10;

        /// <summary>
        /// The external id.
        /// </summary>
        private readonly long ExternalId = 1;

		/// <summary>
		/// The user id.
		/// </summary>
		private readonly long UserId = 1;

		/// <summary>
		/// The participant user id.
		/// </summary>
		private readonly long ParticipantUserId = 10;

		/// <summary>
		/// The group user id.
		/// </summary>
		private readonly long ParticipantGroupId = 1;

		/// <summary>
		/// The organizationuser id.
		/// </summary>
		private readonly long ParticipantOrganizationId = 1;

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

		/// <summary>
		/// The first attachment id.
		/// </summary>
		private long FirstAttachmentId;

		/// <summary>
		/// The work item id.
		/// </summary>
		private long WorkItemId;
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

			var workItem = new WorkItem
			{
				Title = "Test Discussion",
				Summary = "Test discussion summary",
				Body = "Test discussion body",
				Type = WorkItemType.Discussion,
				Status = WorkItemStatus.Created,
				UserId = this.UserId,
				WorkItemAttachments = new List<WorkItemAttachment>
				{
					new WorkItemAttachment
					{
						ExternalId = this.ExternalId,
						Url = this.Url,
						UserId = this.UserId
					}
				},
				WorkItemParticipants = new List<WorkItemParticipant>
				{
					new WorkItemParticipant
					{
						EntityId = this.UserId,
						EntityType = EntityType.User
					},
					new WorkItemParticipant
					{
						EntityId = this.ParticipantUserId,
						EntityType = EntityType.User
					},
					new WorkItemParticipant
					{
						EntityId = this.ParticipantGroupId,
						EntityType = EntityType.Group
					},
					new WorkItemParticipant
					{
						EntityId = this.ParticipantOrganizationId,
						EntityType = EntityType.Organization
					}
				}
			};

			this.Context.WorkItems.Add(workItem);
			this.Context.SaveChanges();

			this.WorkItemId = this.Context.WorkItems.Min(wi => wi.Id);
			this.FirstAttachmentId = this.Context.WorkItemAttachments.Min(attachment => attachment.Id);

			var attachments = new List<WorkItemAttachment>();
			for (int i = 1; i < this.AttachmentsCount; i++)
			{
				attachments.Add(CreateAttachment());
			}

			this.Context.WorkItemAttachments.AddRange(attachments);
			this.Context.SaveChanges();
		}

		/// <summary>
		/// the test clean up
		/// </summary>
		[TearDown]
		public void Cleanup()
		{
			this.Context.Database.EnsureDeleted();
			this.Context.Dispose();
		}
		#endregion

		#region [Methods] Tests
		/// <summary>
		/// The participant get attachment by id test, from user claim.
		/// </summary>
		[Test]
		public void TestParticipantUserGetById()
		{
			var membership = new UserMembership
			{
				UserId = this.ParticipantUserId,
				Groups = new GroupClaimType[0],
				Organizations = new OrganizationClaimType[0]
			};
			// Act
			var attachment = this.AttachmentRepository.GetAsync(this.FirstAttachmentId, membership).Result;

			// Assert
			Assert.AreEqual(this.ExternalId, attachment.ExternalId);
			Assert.AreEqual(this.Url, attachment.Url);
			Assert.AreEqual(this.WorkItemId, attachment.WorkItemId);
		}

		/// <summary>
		/// The participant get attachment by id test, from group claim.
		/// </summary>
		[Test]
		public void TestParticipantGroupGetById()
		{
			var membership = new UserMembership
			{
				UserId = 2,
				Groups = new[]
				{
					new GroupClaimType
					{
						Id = this.ParticipantGroupId,
						Name = "test",
						Claims = null
					}
				},
				Organizations = new OrganizationClaimType[0]
			};
			// Act
			var attachment = this.AttachmentRepository.GetAsync(this.FirstAttachmentId, membership).Result;

			// Assert
			Assert.AreEqual(this.ExternalId, attachment.ExternalId);
			Assert.AreEqual(this.Url, attachment.Url);
			Assert.AreEqual(this.WorkItemId, attachment.WorkItemId);
		}

		/// <summary>
		/// The participant get attachment by id test, from organization claim.
		/// </summary>
		[Test]
		public void TestParticipantOrganizationGetById()
		{
			var membership = new UserMembership
			{
				UserId = 2,
				Groups = new GroupClaimType[0],
				Organizations = new[]
				{
					new OrganizationClaimType
					{
						Id = this.ParticipantOrganizationId,
						Name = "test",
						Role = "test",
						Type = "test"						
					}
				}
			};
			// Act
			var attachment = this.AttachmentRepository.GetAsync(this.FirstAttachmentId, membership).Result;

			// Assert
			Assert.AreEqual(this.ExternalId, attachment.ExternalId);
			Assert.AreEqual(this.Url, attachment.Url);
			Assert.AreEqual(this.WorkItemId, attachment.WorkItemId);
		}

		/// <summary>
		/// The get by id test.
		/// </summary>
		[Test]
        public void TestGetById()
        {
            var membership = this.CreateMembership();
            // Act
            var attachment = this.AttachmentRepository.GetAsync(this.FirstAttachmentId, membership).Result;

            // Assert
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
            try
            {
                var membership = this.CreateMembership();
				var invalidId = this.Context.WorkItemAttachments.Max(a => a.Id) + 1;
				var attachment = this.AttachmentRepository.GetAsync(invalidId, membership).Result;
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
            try
            {
                var membership = this.CreateMembership();
                membership.UserId = 2;
                var attachment = this.AttachmentRepository.GetAsync(this.FirstAttachmentId, membership).Result;
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
				WorkItemId = this.WorkItemId,
				PageSize = 20
			};
			var membership = this.CreateMembership();
			// Act
			var attachments = this.AttachmentRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.AttachmentsCount, attachments.Count);

			foreach (WorkItemAttachment attachment in attachments)
			{
				Assert.AreEqual(this.ExternalId, attachment.ExternalId);
				Assert.AreEqual(this.Url, attachment.Url);
			}
		}

		/// <summary>
		/// The get all test, work item invalid.
		/// </summary>
		[Test]
		public void TestGetAllInvalid()
		{
			try
			{
				var parameters = new WorkItemAttachmentParameters
				{
					WorkItemId = this.Context.WorkItems.Count() + 1
				};
				var membership = this.CreateMembership();
				// Act
				var attachments = this.AttachmentRepository.GetAllAsync(parameters, membership).Result;
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
		/// The participant get all test, from user claim.
		/// </summary>
		[Test]
		public void TestParticipantUserGetAll()
		{
			var parameters = new WorkItemAttachmentParameters
			{
				WorkItemId = this.WorkItemId,
				PageSize = 20
			};
			var membership = new UserMembership
			{
				UserId = this.ParticipantUserId,
				Groups = new GroupClaimType[0],
				Organizations = new OrganizationClaimType[0]
			};
			// Act
			var attachments = this.AttachmentRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.AttachmentsCount, attachments.Count);

			foreach (WorkItemAttachment attachment in attachments)
			{
				Assert.AreEqual(this.ExternalId, attachment.ExternalId);
				Assert.AreEqual(this.Url, attachment.Url);
			}
		}

		/// <summary>
		/// The participant get all test, from group claim.
		/// </summary>
		[Test]
		public void TestParticipantGroupGetAll()
		{
			var parameters = new WorkItemAttachmentParameters
			{
				WorkItemId = this.WorkItemId,
				PageSize = 20
			};
			var membership = new UserMembership
			{
				UserId = 2,
				Groups = new[]
				{
					new GroupClaimType
					{
						Id = this.ParticipantGroupId,
						Name = "test",
						Claims = null
					}
				},
				Organizations = new OrganizationClaimType[0]
			};
			// Act
			var attachments = this.AttachmentRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.AttachmentsCount, attachments.Count);

			foreach (WorkItemAttachment attachment in attachments)
			{
				Assert.AreEqual(this.ExternalId, attachment.ExternalId);
				Assert.AreEqual(this.Url, attachment.Url);
			}
		}

		/// <summary>
		/// The participant get all test, from organization claim.
		/// </summary>
		[Test]
		public void TestParticipantOrganizationGetAll()
		{
			var parameters = new WorkItemAttachmentParameters
			{
				WorkItemId = this.WorkItemId,
				PageSize = 20
			};
			var membership = new UserMembership
			{
				UserId = 2,
				Groups = new GroupClaimType[0],
				Organizations = new[]
				{
					new OrganizationClaimType
					{
						Id = this.ParticipantOrganizationId,
						Name = "test",
						Role = "test",
						Type = "test"
					}
				}
			};
			// Act
			var attachments = this.AttachmentRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.AttachmentsCount, attachments.Count);

			foreach (WorkItemAttachment attachment in attachments)
			{
				Assert.AreEqual(this.ExternalId, attachment.ExternalId);
				Assert.AreEqual(this.Url, attachment.Url);
			}
		}

		/// <summary>
		/// The participant get all test unauthorized, from user claim.
		/// </summary>
		[Test]
		public void TestParticipantUserGetAllUnauthorized()
		{
			try
			{
				var parameters = new WorkItemAttachmentParameters
				{
					WorkItemId = this.WorkItemId
				};
				var membership = this.CreateMembership();
				membership.UserId = 2;
				// Act
				var attachments = this.AttachmentRepository.GetAllAsync(parameters, membership).Result;
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
		/// The participant get all test unauthorized, from group claim.
		/// </summary>
		[Test]
		public void TestParticipantGroupGetAllUnauthorized()
		{
			try
			{
				var parameters = new WorkItemAttachmentParameters
				{
					WorkItemId = this.WorkItemId
				};
				var membership = new UserMembership
				{
					UserId = 2,
					Groups = new[]
				{
					new GroupClaimType
					{
						Id = 2,
						Name = "test",
						Claims = null
					}
				},
					Organizations = new OrganizationClaimType[0]
				};
				// Act
				var attachments = this.AttachmentRepository.GetAllAsync(parameters, membership).Result;
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
		/// The participant get all test unauthorized, from organization claim.
		/// </summary>
		[Test]
		public void TestParticipantOrganizationGetAllUnauthorized()
		{
			try
			{
				var parameters = new WorkItemAttachmentParameters
				{
					WorkItemId = this.WorkItemId
				};
				var membership = new UserMembership
				{
					UserId = 2,
					Groups = new GroupClaimType[0],
					Organizations = new[]
				{
					new OrganizationClaimType
					{
						Id = 2,
						Name = "test",
						Role = "test",
						Type = "test"
					}
				}
				};
				// Act
				var attachments = this.AttachmentRepository.GetAllAsync(parameters, membership).Result;
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
		/// The create test.
		/// </summary>
		[Test]
        public void TestCreate()
        {
            var membership = this.CreateMembership();
            var attachment = this.CreateAttachment();

            // Act
            var attachmentReturned = this.AttachmentRepository.CreateAsync(attachment, membership).Result;

            // Assert
            Assert.AreEqual(this.ExternalId, attachmentReturned.ExternalId);
            Assert.AreEqual(this.Url, attachmentReturned.Url);
            Assert.AreEqual(this.WorkItemId, attachmentReturned.WorkItemId);
        }

        /// <summary>
        /// The create test, work item id invalid.
        /// </summary>
        [Test]
        public void TestCreateWithWorkItemIdInvalid()
        {
			WorkItemAttachment workItemInvalid = null;
            try
            {
                var membership = this.CreateMembership();
                workItemInvalid = this.CreateAttachment();
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
            try
            {
                var membership = this.CreateMembership();
                membership.UserId = 2;
                var userIdInvalid = this.CreateAttachment();
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
            var attachment = this.AttachmentRepository.GetAsync(this.FirstAttachmentId, membership).Result;
			attachment.Url = "updated";
            // Act
            var attchmentReturned = this.AttachmentRepository.UpdateAsync(attachment, membership).Result;

            // Assert
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
            try
            {
                var membership = this.CreateMembership();
                membership.UserId = 2;
				var attachment = this.AttachmentRepository.GetAsync(this.FirstAttachmentId, membership).Result;
				attachment.Url = "updated";
				

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
            try
            {
                var membership = this.CreateMembership();
				var attachment = this.AttachmentRepository.GetAsync(this.FirstAttachmentId, membership).Result;
				attachment.Url = "updated";
				attachment.Id = this.Context.WorkItemAttachments.Max(a => a.Id) + 1;

				var attachmentException = this.AttachmentRepository.UpdateAsync(attachment, membership).Result;
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
			this.AttachmentRepository.DeleteAsync(this.FirstAttachmentId, membership);

            try
            {
                var attachmentException = this.AttachmentRepository.GetAsync(this.AttachmentsCount, membership).Result;
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
				this.AttachmentRepository.DeleteAsync(this.AttachmentsCount, membership).Wait();
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
				this.AttachmentRepository.DeleteAsync(this.FirstAttachmentId, membership).Wait();
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
        public WorkItemAttachment CreateAttachment()
        {
            return new WorkItemAttachment
			{
                WorkItemId = this.WorkItemId,
                ExternalId = this.ExternalId,
                Url = this.Url,
				UserId = this.UserId
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
