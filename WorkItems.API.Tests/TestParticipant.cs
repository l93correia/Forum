using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Security;
using Emsa.Mared.WorkItems.API.Database;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.WorkItems.API.Database.Repositories.WorkItems;
using Emsa.Mared.WorkItems.API.Tests;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Emsa.Mared.WorkItems.API.Tests
{
    [TestFixture]
    public class TestParticipant
    {
        #region [Constants]
        /// <summary>
        /// The number of participants.
        /// </summary>
        private readonly long ParticipantsCount = 5;

        /// <summary>
        /// The worn item id.
        /// </summary>
        private readonly long WorkItemId = 1;

        /// <summary>
        /// The entity id.
        /// </summary>
        private readonly long EntityId = 1;

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

            var participants = new List<WorkItemParticipant>();
            for (int i = 1; i <= this.ParticipantsCount; i++)
            {
                participants.Add(CreateParticipant(i));
            }

			var workItem = new WorkItem
			{
				Id = this.WorkItemId,
				Title = "Test Discussion",
				Summary = "Test discussion summary",
				Body = "Test discussion body",
				Type = WorkItemType.Discussion,
				Status = Status.Created,
				UserId = this.EntityId,
				WorkItemParticipants = new List<WorkItemParticipant>
				{
					new WorkItemParticipant
					{
						EntityId = this.EntityId,
						EntityType = this.EntityType
					}
				}
			};

			this.Context.WorkItems.Add(workItem);
			this.Context.WorkItemParticipants.AddRange(participants);
			this.Context.SaveChanges();
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
            var participantId = 1;
            // Act
            var participant = ParticipantRepository.GetAsync(participantId, membership).Result;

            // Assert
            Assert.AreEqual(participantId, participant.Id);
            Assert.AreEqual(EntityId, participant.EntityId);
            Assert.AreEqual(EntityType, participant.EntityType);
            Assert.AreEqual(WorkItemId, participant.WorkItemId);
        }

        /// <summary>
        /// The get by id test, id invalid.
        /// </summary>
        [Test]
        public void TestGetByIdInvalid()
        {
            WorkItemParticipant participant = null;
            try
            {
                var membership = CreateMembership();
                participant = ParticipantRepository.GetAsync(ParticipantsCount + 1, membership).Result;
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
			WorkItemParticipant participant = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                participant = ParticipantRepository.GetAsync(ParticipantsCount, membership).Result;
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
            var participants = ParticipantRepository.GetAllAsync(null, membership).Result;

            // Assert
            Assert.AreEqual(ParticipantsCount, participants.Count);

            var i = 1;
            foreach (WorkItemParticipant participant in participants)
            {
                Assert.AreEqual(i, participant.Id);
                Assert.AreEqual(EntityId, participant.EntityId);
                Assert.AreEqual(EntityType, participant.EntityType);
                Assert.AreEqual(1, participant.WorkItemId);
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
            var participantId = ParticipantsCount + 1;
            var participant = CreateParticipant(participantId);

            // Act
            var discussionReturned = ParticipantRepository.CreateAsync(participant, membership).Result;

            // Assert
            Assert.AreEqual(participantId, discussionReturned.Id);
            Assert.AreEqual(EntityId, discussionReturned.EntityId);
            Assert.AreEqual(EntityType, discussionReturned.EntityType);
            Assert.AreEqual(WorkItemId, discussionReturned.WorkItemId);
        }

        /// <summary>
        /// The create test, discussion id invalid.
        /// </summary>
        [Test]
        public void TestCreateDiscussionIdInvalid()
        {
			WorkItemParticipant userIdInvalid = null;
            try
            {
                var membership = CreateMembership();
                userIdInvalid = CreateParticipant(ParticipantsCount);
                userIdInvalid.WorkItemId = WorkItemId + 1;
                var responseException = ParticipantRepository.CreateAsync(userIdInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException1)
                {
                    Assert.AreEqual(WorkItem.DoesNotExist, modelException1.Message);

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
			WorkItemParticipant userIdInvalid = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                userIdInvalid = CreateParticipant(ParticipantsCount);
                var responseException = ParticipantRepository.CreateAsync(userIdInvalid, membership).Result;
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
            var response = ParticipantRepository.GetAsync(ParticipantsCount, membership).Result;

            // Act
            var discussionReturned = ParticipantRepository.UpdateAsync(response, membership).Result;

            // Assert
            Assert.AreEqual(ParticipantsCount, discussionReturned.Id);
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
			WorkItemParticipant participant = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                participant = CreateParticipant(ParticipantsCount);
                var participantUpdated = ParticipantRepository.UpdateAsync(participant, membership).Result;
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
			WorkItemParticipant participantInvalid = null;
            try
            {
                var membership = CreateMembership();
                participantInvalid = CreateParticipant(ParticipantsCount + 1);
                var participantException = ParticipantRepository.UpdateAsync(participantInvalid, membership).Result;
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
            var membership = CreateMembership();
            // Act
            ParticipantRepository.DeleteAsync(ParticipantsCount, membership);

			WorkItemParticipant participantException = null;
            try
            {
                participantException = ParticipantRepository.GetAsync(ParticipantsCount, membership).Result;
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
            var membership = CreateMembership();
            // Test exception
            try
            {
                ParticipantRepository.DeleteAsync(ParticipantsCount + 1, membership).Wait();
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
            var membership = CreateMembership();
            membership.UserId = 2;
            // Test exception
            try
            {
                ParticipantRepository.DeleteAsync(ParticipantsCount, membership).Wait();
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
        /// <param name="index">The participant index.</param>
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
                GroupIds = new long[0],
                OrganizationsIds = new long[0]
            };
        }
        #endregion
    }
}
