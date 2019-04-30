using Emsa.Mared.Common.Claims;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Extensions;
using Emsa.Mared.Common.Security;
using Emsa.Mared.WorkItems.API.WorkItems.Database;
using Emsa.Mared.WorkItems.API.WorkItems.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.WorkItems.API.WorkItems.Database.Repositories.WorkItems;
using Emsa.Mared.WorkItems.API.WorkItems.Tests;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Emsa.Mared.WorkItems.API.WorkItems.Tests
{
	[TestFixture]
    public class TestWorkItem
    {
        #region [Constants]
        /// <summary>
        /// The title to test.
        /// </summary>
        private readonly string Title = "Test {0}";

        /// <summary>
        /// The summary to test.
        /// </summary>
        private readonly string Summary = "Test summary {0}";

		/// <summary>
		/// The body to test.
		/// </summary>
		private readonly string Body = "Test body {0}";

        /// <summary>
        /// The number of work items.
        /// </summary>
        private readonly long WorkItemsCount = 5;

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
		#endregion

		#region [Attributes]
		/// <summary>
		/// The repository.
		/// </summary>
		private IWorkItemRepository WorkItemRepository;

        /// <summary>
        /// The data context.
        /// </summary>
        private WorkItemsContext Context;

		/// <summary>
		/// The first work item id.
		/// </summary>
		private long FirstWorkItemId;
		#endregion

		#region [SetUp]
		/// <summary>
		/// The tests setup.
		/// </summary>
		[SetUp]
        public void Setup()
        {
            this.Context = new InMemoryDbContextFactory().GetDbContext(Guid.NewGuid());
			this.WorkItemRepository = new WorkItemRepository(Context);

			// Add the work items to generate the ids
            for (int i = 0; i < this.WorkItemsCount; i++)
            {
				this.CreateSetupWorkItem(userId: 1,type: WorkItemType.Discussion);
            }

			this.Context.SaveChanges();

			this.FirstWorkItemId = this.Context.WorkItems.Min(workItem => workItem.Id);
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
		/// The participant get work item by id test, from user claim.
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
			var workItemId = this.FirstWorkItemId;
			// Act
			var workItem = this.WorkItemRepository.GetAsync(workItemId, membership).Result;

			// Assert
			Assert.AreEqual(string.Format(Title, workItemId), workItem.Title);
			Assert.AreEqual(string.Format(Summary, workItemId), workItem.Summary);
			Assert.AreEqual(string.Format(Body, workItemId), workItem.Body);
			Assert.AreEqual(WorkItemType.Discussion, workItem.Type);
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
			var workItemId = this.FirstWorkItemId;
			// Act
			var workItem = this.WorkItemRepository.GetAsync(workItemId, membership).Result;

			// Assert
			Assert.AreEqual(string.Format(Title, workItemId), workItem.Title);
			Assert.AreEqual(string.Format(Summary, workItemId), workItem.Summary);
			Assert.AreEqual(string.Format(Body, workItemId), workItem.Body);
			Assert.AreEqual(WorkItemType.Discussion, workItem.Type);
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
			var workItemId = this.FirstWorkItemId;
			// Act
			var workItem = this.WorkItemRepository.GetAsync(workItemId, membership).Result;

			// Assert
			Assert.AreEqual(string.Format(Title, workItemId), workItem.Title);
			Assert.AreEqual(string.Format(Summary, workItemId), workItem.Summary);
			Assert.AreEqual(string.Format(Body, workItemId), workItem.Body);
			Assert.AreEqual(WorkItemType.Discussion, workItem.Type);
		}

		/// <summary>
		/// The get by id test.
		/// </summary>
		[Test]
        public void TestGetById()
        {
            var workItemId = this.FirstWorkItemId;
            var membership = this.CreateMembership();
            // Act
            var workItem = this.WorkItemRepository.GetAsync(workItemId, membership).Result;

            // Assert
            Assert.AreEqual(string.Format(Title, workItemId), workItem.Title);
            Assert.AreEqual(string.Format(Summary, workItemId), workItem.Summary);
            Assert.AreEqual(string.Format(Body, workItemId), workItem.Body);
            Assert.AreEqual(WorkItemType.Discussion, workItem.Type);
            Assert.AreEqual(membership.UserId, workItem.UserId);
        }

        /// <summary>
        /// The get by id test, user unauthorized.
        /// </summary>
        [Test]
        public void TestGetByIdUnauthorized()
        {
            try
            {
                var membership = this.CreateMembership();
                membership.UserId = 2;
                var discussionException = this.WorkItemRepository.GetAsync(this.FirstWorkItemId, membership).Result;
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
            try
            {
                var membership = this.CreateMembership();
                var workItemException = this.WorkItemRepository.GetAsync(WorkItemsCount + 1, membership).Result;
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
        /// The get all test.
        /// </summary>
        [Test]
        public void TestGetAll()
        {
            var membership = this.CreateMembership();
            // Act
            var workItems = this.WorkItemRepository.GetAllAsync(parameters: null, membership: membership).Result;

            // Assert
            Assert.AreEqual(this.WorkItemsCount, workItems.Count);

            var i = this.FirstWorkItemId;
            foreach (WorkItem workItem in workItems)
            {
				Assert.AreEqual(string.Format(Title, i), workItem.Title);
				Assert.AreEqual(string.Format(Summary, i), workItem.Summary);
				Assert.AreEqual(string.Format(Body, i), workItem.Body);
				Assert.AreEqual(WorkItemType.Discussion, workItem.Type);
				Assert.AreEqual(membership.UserId, workItem.UserId);
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
            var workItem = this.CreateTestWorkItem(type: WorkItemType.Discussion);

            // Act
            var workItemReturned = this.WorkItemRepository.CreateAsync(workItem, membership).Result;

			// Assert
			Assert.AreEqual(workItem.Title, workItemReturned.Title);
			Assert.AreEqual(workItem.Summary, workItemReturned.Summary);
			Assert.AreEqual(workItem.Body, workItemReturned.Body);
			Assert.AreEqual(workItem.Type, workItemReturned.Type);
			Assert.AreEqual(workItem.UserId, workItemReturned.UserId);
		}

        /// <summary>
        /// The create test, title invalid.
        /// </summary>
        [Test]
        public void TestCreateTitleInvalid()
        {
            WorkItem titleInvalid = null;
            try
            {
                var membership = this.CreateMembership();
                titleInvalid = this.CreateTestWorkItem();
                titleInvalid.Title = string.Empty;
                var workItemException = this.WorkItemRepository.CreateAsync(titleInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(titleInvalid.InvalidFieldMessage(p => p.Title), modelException.Message);

                    return;
                }
            }
            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
        }

		/// <summary>
		/// The create test, summary invalid.
		/// </summary>
		[Test]
		public void TestCreateSummaryInvalid()
		{
			WorkItem summaryInvalid = null;
			try
			{
				var membership = this.CreateMembership();
				summaryInvalid = this.CreateTestWorkItem();
				summaryInvalid.Summary = string.Empty;
				var workItemException = this.WorkItemRepository.CreateAsync(summaryInvalid, membership).Result;
			}
			catch (AggregateException exc)
			{
				if (exc.InnerException is ModelException modelException)
				{
					Assert.AreEqual(summaryInvalid.InvalidFieldMessage(p => p.Summary), modelException.Message);

					return;
				}
			}
			Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
		}

		/// <summary>
		/// The create test, body invalid.
		/// </summary>
		[Test]
        public void TestCreateBodyInvalid()
        {
            WorkItem bodyInvalid = null;
            try
            {
                var membership = this.CreateMembership();
                bodyInvalid = this.CreateTestWorkItem();
                bodyInvalid.Body = string.Empty;
                var workItemException = this.WorkItemRepository.CreateAsync(bodyInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException1)
                {
                    Assert.AreEqual(bodyInvalid.InvalidFieldMessage(p => p.Body), modelException1.Message);

                    return;
                }
            }
            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
        }

		/// <summary>
		/// The create test, type invalid.
		/// </summary>
		[Test]
		public void TestCreateTypeInvalid()
		{
			WorkItem typeInvalid = null;
			try
			{
				var membership = this.CreateMembership();
				typeInvalid = this.CreateTestWorkItem(type: WorkItemType.Default);
				var workItemException = this.WorkItemRepository.CreateAsync(typeInvalid, membership).Result;
			}
			catch (AggregateException exc)
			{
				if (exc.InnerException is ModelException modelException1)
				{
					Assert.AreEqual(typeInvalid.InvalidFieldMessage(p => p.Type), modelException1.Message);

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
            var workItemId = this.FirstWorkItemId;
            var workItem = this.WorkItemRepository.GetAsync(workItemId, membership).Result;

            // Act
            var workItemReturned = this.WorkItemRepository.UpdateAsync(workItem, membership).Result;

            // Assert
            Assert.AreEqual(workItemId, workItemReturned.Id);
            Assert.AreEqual(string.Format(Title, workItemId), workItemReturned.Title);
            Assert.AreEqual(string.Format(Summary, workItemId), workItemReturned.Summary);
            Assert.AreEqual(string.Format(Body, workItemId), workItemReturned.Body);
            Assert.AreEqual(WorkItemStatus.Updated, workItemReturned.Status);
            Assert.AreEqual(membership.UserId, workItemReturned.UserId);
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
                var membership = this.CreateMembership();
				var workItem = this.WorkItemRepository.GetAsync(this.Context.WorkItems.Max(w => w.Id), membership).Result;
				this.Context.Entry(workItem).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
				workItem.Id += 1;
				var workItemException = this.WorkItemRepository.UpdateAsync(workItem, membership).Result;
            }
            catch (AggregateException execption)
            {
                if (execption.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItem.DoesNotExist, modelException.Message);

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
                var membership = this.CreateMembership();

				var workItem = this.WorkItemRepository.GetAsync(this.FirstWorkItemId, membership).Result;

				membership.UserId = 2;
				var workItemException = this.WorkItemRepository.UpdateAsync(workItem, membership).Result;
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
        /// The update test, title invalid.
        /// </summary>
        [Test]
        public void TestUpdateTitleInvalid()
        {
            WorkItem titleInvalid = null;
            try
            {
                var membership = this.CreateMembership();
                titleInvalid = this.WorkItemRepository.GetAsync(this.Context.WorkItems.Max(w => w.Id), membership).Result;
				this.Context.Entry(titleInvalid).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
				titleInvalid.Title = string.Empty;
                var workItemException = this.WorkItemRepository.UpdateAsync(titleInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException1)
                {
                    Assert.AreEqual(titleInvalid.InvalidFieldMessage(p => p.Title), modelException1.Message);

                    return;
                }
            }
            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));
        }

        /// <summary>
        /// The update test, summary invalid.
        /// </summary>
        [Test]
        public void TestUpdateSummaryInvalid()
        {
            WorkItem summaryInvalid = null;
            try
            {
                var membership = this.CreateMembership();
                summaryInvalid = this.WorkItemRepository.GetAsync(this.Context.WorkItems.Max(w => w.Id), membership).Result;
				this.Context.Entry(summaryInvalid).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
				summaryInvalid.Summary = string.Empty;
                var summaryException = this.WorkItemRepository.UpdateAsync(summaryInvalid, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(summaryInvalid.InvalidFieldMessage(p => p.Summary), modelException.Message);

                    return;
                }
            }
            Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));

        }

		/// <summary>
		/// The update test, body invalid.
		/// </summary>
		[Test]
		public void TestUpdateBodyInvalid()
		{
			WorkItem bodyInvalid = null;
			try
			{
				var membership = this.CreateMembership();
				var id = this.Context.WorkItems.Max(w => w.Id);
				bodyInvalid = this.WorkItemRepository.GetAsync(id, membership).Result;
				this.Context.Entry(bodyInvalid).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
				bodyInvalid.Body = string.Empty;
				var bodyException = this.WorkItemRepository.UpdateAsync(bodyInvalid, membership).Result;
			}
			catch (AggregateException exc)
			{
				if (exc.InnerException is ModelException modelException)
				{
					Assert.AreEqual(bodyInvalid.InvalidFieldMessage(p => p.Body), modelException.Message);

					return;
				}
			}
			Assert.Fail("Exception of type {0} should be thrown.", typeof(ModelException));

		}

		/// <summary>
		/// The update test, cannot change type.
		/// </summary>
		[Test]
		public void TestUpdateTypeInvalid()
		{
			try
			{
				var membership = this.CreateMembership();

				var typeInvalid = this.WorkItemRepository.GetAsync(this.Context.WorkItems.Max(w => w.Id), membership).Result;
				this.Context.Entry(typeInvalid).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
				typeInvalid.Type = WorkItemType.Event;

				var typeException = this.WorkItemRepository.UpdateAsync(typeInvalid, membership).Result;
			}
			catch (AggregateException exc)
			{
				if (exc.InnerException is ModelException modelException)
				{
					Assert.AreEqual(WorkItem.CannotChangeType, modelException.Message);

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
            var workItemId = 1;

            // Act
            WorkItemRepository.DeleteAsync(workItemId, membership);

            WorkItem workItemException = null;
            try
            {
                workItemException = this.WorkItemRepository.GetAsync(workItemId, membership).Result;
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
        /// The delete test, discussion not found.
        /// </summary>
        [Test]
        public void TestDeleteNotFound()
        {
            try
            {
                var membership = this.CreateMembership();
				this.WorkItemRepository.DeleteAsync(this.WorkItemsCount + 1, membership).Wait();
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
        /// The delete test, user unauthorized.
        /// </summary>
        [Test]
        public void TestDeleteUnauthorized()
        {
            try
            {
                var membership = this.CreateMembership();
                membership.UserId = 2;
				this.WorkItemRepository.DeleteAsync(this.FirstWorkItemId, membership).Wait();
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
		/// Creates a work item intended to be used in the setup process.
		/// </summary>
		/// 
		/// <param name="userId">The user id.</param>
		/// <param name="type">The work item type.</param>
		public WorkItem CreateSetupWorkItem(long userId = 0, WorkItemType type = WorkItemType.Default)
        {
			var workItem = new WorkItem
			{
				Title = string.Format(this.Title, 0),
				Summary = string.Format(this.Summary, 0),
				Body = string.Format(this.Body, 0),
				Type = type,
				Status = WorkItemStatus.Created,
				UserId = userId,
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

			workItem.Title = string.Format(this.Title, workItem.Id);
			workItem.Summary = string.Format(this.Summary, workItem.Id);
			workItem.Body = string.Format(this.Body, workItem.Id);

			this.Context.SaveChanges();

			return workItem;
		}

		/// <summary>
		/// Creates a work item intended to be used in the test process.
		/// </summary>
		/// 
		/// <param name="userId">The user id.</param>
		/// <param name="type">The work item type.</param>
		public WorkItem CreateTestWorkItem(long userId = 0, WorkItemType type = WorkItemType.Default)
		{
			var random = new Random().Next();

			var workItem = new WorkItem
			{
				Title = string.Format(this.Title, random),
				Summary = string.Format(this.Summary, random),
				Body = string.Format(this.Body, random),
				Type = type,
				Status = WorkItemStatus.Created,
				UserId = userId,
				WorkItemParticipants = new List<WorkItemParticipant>
				{
					new WorkItemParticipant
					{
						EntityId = userId,
						EntityType = EntityType.User
					}
				}
			};

			return workItem;
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