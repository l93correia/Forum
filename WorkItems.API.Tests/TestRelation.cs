using Emsa.Mared.Common.Claims;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Security;
using Emsa.Mared.WorkItems.API.WorkItems.Database;
using Emsa.Mared.WorkItems.API.WorkItems.Database.Repositories.WorkItemRelations;
using Emsa.Mared.WorkItems.API.WorkItems.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.WorkItems.API.WorkItems.Database.Repositories.WorkItems;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emsa.Mared.WorkItems.API.WorkItems.Tests
{
	[TestFixture]
    public class TestRelation
	{
        #region [Constants]
        /// <summary>
        /// The number of relations.
        /// </summary>
        private readonly long RelationsCount = 5;

        /// <summary>
        /// The related from work item id.
        /// </summary>
        private readonly long RelatedFromWorkItemId = 1;

		/// <summary>
		/// The related to work item id.
		/// </summary>
		private readonly long RelatedToWorkItemId = 2;

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
		/// The relation type.
		/// </summary>
		private readonly RelationType RelationType = RelationType.Related;
        #endregion

        #region [Attributes]
        /// <summary>
        /// The repository.
        /// </summary>
        private IWorkItemRelationRepository RelationRepository;

        /// <summary>
        /// The data context.
        /// </summary>
        private WorkItemsContext Context;

		/// <summary>
		/// The first relation id.
		/// </summary>
		private long FirstRelationId;
		#endregion

		#region [SetUp]
		/// <summary>
		/// The tests setup.
		/// </summary>
		[SetUp]
        public void Setup()
        {
            this.Context = new InMemoryDbContextFactory().GetDbContext(Guid.NewGuid());
            var workItemRepository = new WorkItemRepository(Context);
            this.RelationRepository = new WorkItemRelationRepository(Context, workItemRepository);

            var relations = new List<WorkItemRelation>();
            for (int i = 1; i <= this.RelationsCount; i++)
            {
                relations.Add(CreateRelation());
            }

			var workItemFrom = new WorkItem
			{
				Id = this.RelatedFromWorkItemId,
				Title = "Test Discussion from",
				Summary = "Test discussion summary from",
				Body = "Test discussion body from",
				Type = WorkItemType.Discussion,
				Status = WorkItemStatus.Created,
				UserId = this.UserId,
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
			var workItemTo = new WorkItem
			{
				Id = this.RelatedToWorkItemId,
				Title = "Test Discussion to",
				Summary = "Test discussion summary to",
				Body = "Test discussion body to",
				Type = WorkItemType.Discussion,
				Status = WorkItemStatus.Created,
				UserId = this.UserId,
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

			Context.WorkItems.Add(workItemFrom);
			Context.WorkItems.Add(workItemTo);
            Context.WorkItemRelations.AddRange(relations);
            Context.SaveChanges();

			this.FirstRelationId = this.Context.WorkItemRelations.Min(relation => relation.Id);
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
				var parameters = new WorkItemRelationParameters
				{
					WorkItemId = this.Context.WorkItems.Count() + 1
				};
				var membership = this.CreateMembership();
				// Act
				var relations = this.RelationRepository.GetAllAsync(parameters, membership).Result;
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
			var parameters = new WorkItemRelationParameters
			{
				WorkItemId = this.RelatedFromWorkItemId,
				PageSize = 20
			};
			var membership = new UserMembership
			{
				UserId = this.ParticipantUserId,
				Groups = new GroupClaimType[0],
				Organizations = new OrganizationClaimType[0]
			};
			// Act
			var relations = this.RelationRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.RelationsCount, relations.Count);

			foreach (WorkItemRelation relation in relations)
			{
				Assert.AreEqual(this.RelatedFromWorkItemId, relation.RelatedFromWorkItemId);
				Assert.AreEqual(this.RelatedToWorkItemId, relation.RelatedToWorkItemId);
				Assert.AreEqual(this.RelationType, relation.RelationType);
			}
		}

		/// <summary>
		/// The participant get all test, from group claim.
		/// </summary>
		[Test]
		public void TestParticipantGroupGetAll()
		{
			var parameters = new WorkItemRelationParameters
			{
				WorkItemId = this.RelatedFromWorkItemId,
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
			var relations = this.RelationRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.RelationsCount, relations.Count);

			foreach (WorkItemRelation relation in relations)
			{
				Assert.AreEqual(this.RelatedFromWorkItemId, relation.RelatedFromWorkItemId);
				Assert.AreEqual(this.RelatedToWorkItemId, relation.RelatedToWorkItemId);
				Assert.AreEqual(this.RelationType, relation.RelationType);
			}
		}

		/// <summary>
		/// The participant get all test, from organization claim.
		/// </summary>
		[Test]
		public void TestParticipantOrganizationGetAll()
		{
			var parameters = new WorkItemRelationParameters
			{
				WorkItemId = this.RelatedFromWorkItemId,
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
			var relations = this.RelationRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.RelationsCount, relations.Count);

			foreach (WorkItemRelation relation in relations)
			{
				Assert.AreEqual(this.RelatedFromWorkItemId, relation.RelatedFromWorkItemId);
				Assert.AreEqual(this.RelatedToWorkItemId, relation.RelatedToWorkItemId);
				Assert.AreEqual(this.RelationType, relation.RelationType);
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
				var parameters = new WorkItemRelationParameters
				{
					WorkItemId = this.RelatedFromWorkItemId
				};
				var membership = this.CreateMembership();
				membership.UserId = 2;
				// Act
				var relations = this.RelationRepository.GetAllAsync(parameters, membership).Result;
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
				var parameters = new WorkItemRelationParameters
				{
					WorkItemId = this.RelatedFromWorkItemId
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
				var relations = this.RelationRepository.GetAllAsync(parameters, membership).Result;
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
		/// The participant get relation by id test.
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
			var relation = this.RelationRepository.GetAsync(this.FirstRelationId, membership).Result;

			// Assert
			Assert.AreEqual(this.RelatedFromWorkItemId, relation.RelatedFromWorkItemId);
			Assert.AreEqual(this.RelatedToWorkItemId, relation.RelatedToWorkItemId);
			Assert.AreEqual(this.RelationType, relation.RelationType);
		}

		/// <summary>
		/// The participant get relation by id test, from group claim.
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
			var relation = this.RelationRepository.GetAsync(this.FirstRelationId, membership).Result;

			// Assert
			Assert.AreEqual(this.RelatedFromWorkItemId, relation.RelatedFromWorkItemId);
			Assert.AreEqual(this.RelatedToWorkItemId, relation.RelatedToWorkItemId);
			Assert.AreEqual(this.RelationType, relation.RelationType);
		}

		/// <summary>
		/// The participant get relation by id test, from organization claim.
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
			var relation = this.RelationRepository.GetAsync(this.FirstRelationId, membership).Result;

			// Assert
			Assert.AreEqual(this.RelatedFromWorkItemId, relation.RelatedFromWorkItemId);
			Assert.AreEqual(this.RelatedToWorkItemId, relation.RelatedToWorkItemId);
			Assert.AreEqual(this.RelationType, relation.RelationType);
		}

		/// <summary>
		/// The get by id test.
		/// </summary>
		[Test]
        public void TestGetById()
        {
            var membership = this.CreateMembership();
            // Act
            var relation = this.RelationRepository.GetAsync(this.FirstRelationId, membership).Result;

            // Assert
            Assert.AreEqual(this.RelatedFromWorkItemId, relation.RelatedFromWorkItemId);
            Assert.AreEqual(this.RelatedToWorkItemId, relation.RelatedToWorkItemId);
            Assert.AreEqual(this.RelationType, relation.RelationType);
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
				var invalidId = this.Context.WorkItemRelations.Max(a => a.Id) + 1;
				var relation = this.RelationRepository.GetAsync(invalidId, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemRelation.DoesNotExist, modelException.Message);

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
                var relation = this.RelationRepository.GetAsync(this.FirstRelationId, membership).Result;
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
			var parameters = new WorkItemRelationParameters
			{
				WorkItemId = this.RelatedFromWorkItemId,
				PageSize = 20
			};
			var membership = this.CreateMembership();
			// Act
			var relations = this.RelationRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.RelationsCount, relations.Count);

			foreach (WorkItemRelation relation in relations)
			{
				Assert.AreEqual(this.RelatedFromWorkItemId, relation.RelatedFromWorkItemId);
				Assert.AreEqual(this.RelatedToWorkItemId, relation.RelatedToWorkItemId);
				Assert.AreEqual(this.RelationType, relation.RelationType);

			}
		}

        /// <summary>
        /// The create test.
        /// </summary>
        [Test]
        public void TestCreate()
        {
            var membership = this.CreateMembership();
            var relation = this.CreateRelation();

            // Act
            var relationReturned = this.RelationRepository.CreateAsync(relation, membership).Result;

            // Assert
            Assert.AreEqual(this.RelatedFromWorkItemId, relationReturned.RelatedFromWorkItemId);
            Assert.AreEqual(this.RelatedToWorkItemId, relationReturned.RelatedToWorkItemId);
            Assert.AreEqual(this.RelationType, relationReturned.RelationType);
        }

        /// <summary>
        /// The create test, work item id invalid.
        /// </summary>
        [Test]
        public void TestCreateWithWorkItemIdInvalid()
        {
            try
            {
                var membership = this.CreateMembership();
                var workItemInvalid = this.CreateRelation();
                workItemInvalid.RelatedToWorkItemId = this.RelatedToWorkItemId + 1;
                var responseException = this.RelationRepository.CreateAsync(workItemInvalid, membership).Result;
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
			WorkItemRelation userIdInvalid = null;
            try
            {
                var membership = this.CreateMembership();
                membership.UserId = 2;
                userIdInvalid = this.CreateRelation();
                var relationException = this.RelationRepository.CreateAsync(userIdInvalid, membership).Result;
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
            var relation = this.RelationRepository.GetAsync(this.FirstRelationId, membership).Result;
			relation.RelatedToWorkItemId = this.RelatedFromWorkItemId;
			relation.RelatedFromWorkItemId = this.RelatedToWorkItemId;
            // Act
            var relationReturned = this.RelationRepository.UpdateAsync(relation, membership).Result;

			// Assert
			Assert.AreEqual(this.RelatedToWorkItemId, relationReturned.RelatedFromWorkItemId);
			Assert.AreEqual(this.RelatedFromWorkItemId, relationReturned.RelatedToWorkItemId);
			Assert.AreEqual(this.RelationType, relationReturned.RelationType);
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
				var relation = this.RelationRepository.GetAsync(this.FirstRelationId, membership).Result;				

				var relationUpdated = this.RelationRepository.UpdateAsync(relation, membership).Result;
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
				var relation = this.RelationRepository.GetAsync(this.FirstRelationId, membership).Result;

				relation.Id = this.Context.WorkItemRelations.Max(a => a.Id) + 1;

				var relationException = this.RelationRepository.UpdateAsync(relation, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemRelation.DoesNotExist, modelException.Message);

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
			this.RelationRepository.DeleteAsync(this.FirstRelationId, membership);

            try
            {
                var relationException = this.RelationRepository.GetAsync(this.RelationsCount, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemRelation.DoesNotExist, modelException.Message);

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
				this.RelationRepository.DeleteAsync(this.RelationsCount, membership).Wait();
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemRelation.DoesNotExist, modelException.Message);

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
				this.RelationRepository.DeleteAsync(this.FirstRelationId, membership).Wait();
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
        public WorkItemRelation CreateRelation()
        {
            return new WorkItemRelation
			{
                RelationType = RelationType.Related,
				RelatedFromWorkItemId = this.RelatedFromWorkItemId,
				RelatedToWorkItemId = this.RelatedToWorkItemId
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
