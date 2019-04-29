using Emsa.Mared.Common.Claims;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Discussions.API.Database;
using Emsa.Mared.Discussions.API.Database.Repositories.Discussions;
using Emsa.Mared.Discussions.API.Database.Repositories.Participants;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Emsa.Mared.Discussions.API.Tests
{
    [TestFixture]
    public class TestParticipant
    {
        #region [Constants]
        /// <summary>
        /// The number of responses.
        /// </summary>
        private readonly long _nParticipants = 5;

        /// <summary>
        /// The discussion id.
        /// </summary>
        private readonly long _discussionId = 1;

        /// <summary>
        /// The entity id.
        /// </summary>
        private readonly long _entityId = 1;

        /// <summary>
        /// The entity type.
        /// </summary>
        private readonly EntityType _entityType = EntityType.User;
        #endregion

        #region [Attributes]
        /// <summary>
        /// The repository.
        /// </summary>
        private IParticipantRepository _repo;

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
            _repo = new ParticipantRepository(_dbContext);

            var participants = new List<Participant>();
            for (int i = 1; i <= _nParticipants; i++)
            {
                participants.Add(CreateParticipant(i));
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
            _dbContext.Participants.AddRange(participants);
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
            var participantId = 1;
            // Act
            var participant = _repo.GetAsync(participantId, membership).Result;

            // Assert
            Assert.AreEqual(participantId, participant.Id);
            Assert.AreEqual(_entityId, participant.EntityId);
            Assert.AreEqual(_entityType, participant.EntityType);
            Assert.AreEqual(_discussionId, participant.DiscussionId);
        }

        /// <summary>
        /// The get by id test, id invalid.
        /// </summary>
        [Test]
        public void TestGetByIdInvalid()
        {
            Participant participant = null;
            try
            {
                var membership = CreateMembership();
                participant = _repo.GetAsync(_nParticipants + 1, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(Participant.DoesNotExist, modelException.Message);

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
            Participant participant = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                participant = _repo.GetAsync(_nParticipants, membership).Result;
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
            var participants = _repo.GetByDiscussion(_discussionId, null, membership).Result;

            // Assert
            Assert.AreEqual(_nParticipants, participants.Count);

            var i = 1;
            foreach (Participant participant in participants)
            {
                Assert.AreEqual(i, participant.Id);
                Assert.AreEqual(_entityId, participant.EntityId);
                Assert.AreEqual(_entityType, participant.EntityType);
                Assert.AreEqual(_discussionId, participant.DiscussionId);
                i++;
            }
        }

        /// <summary>
        /// The get by discussion id test, id invalid.
        /// </summary>
        [Test]
        public void TestGetByDiscussionIdInvalid()
        {
            List<Participant> participants = null;
            try
            {
                var membership = CreateMembership();
                participants = _repo.GetByDiscussion(_discussionId + 1, null, membership).Result;
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
            List<Participant> participants = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                participants = _repo.GetByDiscussion(_discussionId, null, membership).Result;
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
            var participants = _repo.GetAllAsync(null, membership).Result;

            // Assert
            Assert.AreEqual(_nParticipants, participants.Count);

            var i = 1;
            foreach (Participant participant in participants)
            {
                Assert.AreEqual(i, participant.Id);
                Assert.AreEqual(_entityId, participant.EntityId);
                Assert.AreEqual(_entityType, participant.EntityType);
                Assert.AreEqual(1, participant.DiscussionId);
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
            var participantId = _nParticipants + 1;
            var participant = CreateParticipant(participantId);

            // Act
            var discussionReturned = _repo.CreateAsync(participant, membership).Result;

            // Assert
            Assert.AreEqual(participantId, discussionReturned.Id);
            Assert.AreEqual(_entityId, discussionReturned.EntityId);
            Assert.AreEqual(_entityType, discussionReturned.EntityType);
            Assert.AreEqual(_discussionId, discussionReturned.DiscussionId);
        }

        /// <summary>
        /// The create test, discussion id invalid.
        /// </summary>
        [Test]
        public void TestCreateDiscussionIdInvalid()
        {
            Participant userIdInvalid = null;
            try
            {
                var membership = CreateMembership();
                userIdInvalid = CreateParticipant(_nParticipants);
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
            Participant userIdInvalid = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                userIdInvalid = CreateParticipant(_nParticipants);
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
            var response = _repo.GetAsync(_nParticipants, membership).Result;

            // Act
            var discussionReturned = _repo.UpdateAsync(response, membership).Result;

            // Assert
            Assert.AreEqual(_nParticipants, discussionReturned.Id);
            Assert.AreEqual(_entityId, discussionReturned.EntityId);
            Assert.AreEqual(_entityType, discussionReturned.EntityType);
            Assert.AreEqual(_discussionId, discussionReturned.DiscussionId);
        }

        /// <summary>
        /// The update test, user unauthorized.
        /// </summary>
        [Test]
        public void TestUpdateUnauthorized()
        {
            Participant participant = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                participant = CreateParticipant(_nParticipants);
                var participantUpdated = _repo.UpdateAsync(participant, membership).Result;
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
            Participant participantInvalid = null;
            try
            {
                var membership = CreateMembership();
                participantInvalid = CreateParticipant(_nParticipants + 1);
                var participantException = _repo.UpdateAsync(participantInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(Participant.DoesNotExist, modelException.Message);

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
            _repo.DeleteAsync(_nParticipants, membership);

            Participant participantException = null;
            try
            {
                participantException = _repo.GetAsync(_nParticipants, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(Participant.DoesNotExist, modelException.Message);

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
                _repo.DeleteAsync(_nParticipants + 1, membership).Wait();
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(Participant.DoesNotExist, modelException.Message);

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
                _repo.DeleteAsync(_nParticipants, membership).Wait();
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
        public Participant CreateParticipant(long? index = 0)
        {
            return new Participant
            {
                Id = index.Value,
                DiscussionId = _discussionId,
                EntityId = _entityId,
                EntityType = _entityType
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
