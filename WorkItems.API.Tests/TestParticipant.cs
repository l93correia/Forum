using Emsa.Mared.Common.Claims;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Security;
using Emsa.Mared.ContentManagement.WorkItems.Database;
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
    public class TestParticipant
    {
        #region [Constants]
        /// <summary>
        /// The number of participants.
        /// </summary>
        private readonly long ParticipantsCount = 10;

        /// <summary>
        /// The entity id.
        /// </summary>
        private readonly long EntityId = 1;

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
		private readonly EntityType EntityType = EntityType.User;
        #endregion

        #region [Attributes]
        /// <summary>
        /// The repository.
        /// </summary>
        private IWorkItemParticipantRepository ParticipantRepository;

        /// <summary>
        /// The data context.
        /// </summary>
        private WorkItemsContext Context;

		/// <summary>
		/// The first participant id.
		/// </summary>
		private long FirstParticipantId;

		/// <summary>
		/// The number of participants created on work item creation.
		/// </summary>
		private long InitialParticipantCount;

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
            var workItemRepository = new WorkItemRepository(this.Context);
			this.ParticipantRepository = new WorkItemParticipantRepository(this.Context, workItemRepository);

			var workItem = new WorkItem
			{
				Title = "Test Discussion",
				Summary = "Test discussion summary",
				Body = "Test discussion body",
				Type = WorkItemType.Discussion,
				Status = WorkItemStatus.Created,
				UserId = this.EntityId,
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

			this.InitialParticipantCount = workItem.WorkItemParticipants.Count();

			this.Context.WorkItems.Add(workItem);
			this.Context.SaveChanges();

			this.WorkItemId = this.Context.WorkItems.Min(wi => wi.Id);
			this.FirstParticipantId = this.Context.WorkItemParticipants.Min(participant => participant.Id);

			var participants = new List<WorkItemParticipant>();
			for (int i = 1; i <= this.ParticipantsCount; i++)
			{
				participants.Add(this.CreateParticipant());
			}

			this.Context.WorkItemParticipants.AddRange(participants);
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
		/// The get all test, work item invalid.
		/// </summary>
		[Test]
		public void TestGetAllInvalid()
		{
			try
			{
				var parameters = new WorkItemParticipantParameters
				{
					WorkItemId = this.Context.WorkItems.Count() + 1
				};
				var membership = this.CreateMembership();
				// Act
				var participants = this.ParticipantRepository.GetAllAsync(parameters, membership).Result;
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
			var parameters = new WorkItemParticipantParameters
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
			var participants = this.ParticipantRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.Context.WorkItemParticipants.Count(), participants.Count);
		}

		/// <summary>
		/// The participant get all test, from group claim.
		/// </summary>
		[Test]
		public void TestParticipantGroupGetAll()
		{
			var parameters = new WorkItemParticipantParameters
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
			var participants = this.ParticipantRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.Context.WorkItemParticipants.Count(), participants.Count);
		}

		/// <summary>
		/// The participant get all test, from organization claim.
		/// </summary>
		[Test]
		public void TestParticipantOrganizationGetAll()
		{
			var parameters = new WorkItemParticipantParameters
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
			var participants = this.ParticipantRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.Context.WorkItemParticipants.Count(), participants.Count);
		}

		/// <summary>
		/// The participant get all test unauthorized, from user claim.
		/// </summary>
		[Test]
		public void TestParticipantUserGetAllUnauthorized()
		{
			try
			{
				var parameters = new WorkItemParticipantParameters
				{
					WorkItemId = this.WorkItemId
				};
				var membership = this.CreateMembership();
				membership.UserId = 2;
				// Act
				var participants = this.ParticipantRepository.GetAllAsync(parameters, membership).Result;
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
				var parameters = new WorkItemParticipantParameters
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
				var participants = this.ParticipantRepository.GetAllAsync(parameters, membership).Result;
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
		/// The participant get participant by id test.
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
			var participant = this.ParticipantRepository.GetAsync(this.FirstParticipantId, membership).Result;

			// Assert
			Assert.AreEqual(this.EntityId, participant.EntityId);
			Assert.AreEqual(this.EntityType, participant.EntityType);
			Assert.AreEqual(this.WorkItemId, participant.WorkItemId);
		}

		/// <summary>
		/// The participant get participant by id test, from group claim.
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
			var participant = this.ParticipantRepository.GetAsync(this.FirstParticipantId, membership).Result;

			// Assert
			Assert.AreEqual(this.EntityId, participant.EntityId);
			Assert.AreEqual(this.EntityType, participant.EntityType);
			Assert.AreEqual(this.WorkItemId, participant.WorkItemId);
		}

		/// <summary>
		/// The participant get participant by id test, from organization claim.
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
			var participant = this.ParticipantRepository.GetAsync(this.FirstParticipantId, membership).Result;

			// Assert
			Assert.AreEqual(this.EntityId, participant.EntityId);
			Assert.AreEqual(this.EntityType, participant.EntityType);
			Assert.AreEqual(this.WorkItemId, participant.WorkItemId);
		}

		/// <summary>
		/// The get by id test.
		/// </summary>
		[Test]
        public void TestGetById()
        {
            var membership = this.CreateMembership();
            var participantId = this.FirstParticipantId;
            // Act
            var participant = this.ParticipantRepository.GetAsync(participantId, membership).Result;

            // Assert
            Assert.AreEqual(this.EntityId, participant.EntityId);
            Assert.AreEqual(this.EntityType, participant.EntityType);
            Assert.AreEqual(this.WorkItemId, participant.WorkItemId);
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
                var participant = this.ParticipantRepository.GetAsync
					(this.Context.WorkItemParticipants.Max(p => p.Id) + 1, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemParticipant.DoesNotExist, modelException.Message);

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
                var participant = this.ParticipantRepository.GetAsync(this.FirstParticipantId, membership).Result;
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
			var parameters = new WorkItemParticipantParameters
			{
				WorkItemId = this.WorkItemId,
				PageSize = 20
			};
            var membership = this.CreateMembership();
            // Act
            var participants = this.ParticipantRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			var participantsCount = this.ParticipantsCount + this.InitialParticipantCount;

			Assert.AreEqual(participantsCount, participants.Count);
        }

        /// <summary>
        /// The create test.
        /// </summary>
        [Test]
        public void TestCreate()
        {
            var membership = this.CreateMembership();
            var participantId = this.Context.WorkItemParticipants.Max(p =>p.Id) + 1;
            var participant = this.CreateParticipant();

            // Act
            var discussionReturned = this.ParticipantRepository.CreateAsync(participant, membership).Result;

            // Assert
            Assert.AreEqual(this.EntityId, discussionReturned.EntityId);
            Assert.AreEqual(this.EntityType, discussionReturned.EntityType);
            Assert.AreEqual(this.WorkItemId, discussionReturned.WorkItemId);
        }

        /// <summary>
        /// The create test, discussion id invalid.
        /// </summary>
        [Test]
        public void TestCreateDiscussionIdInvalid()
        {
            try
            {
                var membership = this.CreateMembership();
                var userIdInvalid = this.CreateParticipant();
                userIdInvalid.WorkItemId = WorkItemId + 1;
                var responseException = this.ParticipantRepository.CreateAsync(userIdInvalid, membership).Result;
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
                var userIdInvalid = this.CreateParticipant();
                var responseException = this.ParticipantRepository.CreateAsync(userIdInvalid, membership).Result;
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
            var membership = this.CreateMembership();
            var participant = this.ParticipantRepository.GetAsync(this.FirstParticipantId, membership).Result;

            // Act
            var discussionReturned = this.ParticipantRepository.UpdateAsync(participant, membership).Result;

            // Assert
            Assert.AreEqual(this.FirstParticipantId, discussionReturned.Id);
            Assert.AreEqual(EntityId, discussionReturned.EntityId);
            Assert.AreEqual(EntityType, discussionReturned.EntityType);
            Assert.AreEqual(WorkItemId, discussionReturned.WorkItemId);
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
				var participant = this.ParticipantRepository.GetAsync(this.FirstParticipantId, membership).Result;

				membership.UserId = 2;

                var participantUpdated = this.ParticipantRepository.UpdateAsync(participant, membership).Result;
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
                var participantInvalid = this.CreateParticipant();
                var participantException = this.ParticipantRepository.UpdateAsync(participantInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemParticipant.DoesNotExist, modelException.Message);

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
            var membership = this.CreateMembership();
			// Act
			this.ParticipantRepository.DeleteAsync(ParticipantsCount, membership);

			WorkItemParticipant participantException = null;
            try
            {
                participantException = this.ParticipantRepository.GetAsync(ParticipantsCount, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemParticipant.DoesNotExist, modelException.Message);

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
            // Test exception
            try
            {
				this.ParticipantRepository.DeleteAsync(ParticipantsCount + 1, membership).Wait();
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemParticipant.DoesNotExist, modelException.Message);

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
            // Test exception
            try
            {
				this.ParticipantRepository.DeleteAsync(this.FirstParticipantId, membership).Wait();
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
        public WorkItemParticipant CreateParticipant()
        {
            return new WorkItemParticipant
			{
                WorkItemId = this.WorkItemId,
                EntityId = this.EntityId,
                EntityType = this.EntityType,
				UserId = this.EntityId
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
