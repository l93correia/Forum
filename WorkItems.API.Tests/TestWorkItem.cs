using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Common.Utility;
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

            var workItems = new List<WorkItem>();
            for (int i = 1; i <= this.WorkItemsCount; i++)
            {
                workItems.Add(CreateSetupWorkItem(index: i, userId: 1,type: WorkItemType.Discussion));
            }

			this.Context.WorkItems.AddRange(workItems);
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
            var workItemId = 1;
            var membership = CreateMembership();
            // Act
            var workItem = WorkItemRepository.GetAsync(workItemId, membership).Result;

            // Assert
            Assert.AreEqual(workItemId, workItem.Id);
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
            WorkItem discussionException = null;
            try
            {
                var membership = CreateMembership();
                membership.UserId = 2;
                discussionException = WorkItemRepository.GetAsync(WorkItemsCount, membership).Result;
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
            WorkItem workItemException = null;
            try
            {
                var membership = CreateMembership();
                workItemException = WorkItemRepository.GetAsync(WorkItemsCount + 1, membership).Result;
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
            var membership = CreateMembership();
            // Act
            var workItems = WorkItemRepository.GetAllAsync(parameters: null, membership: membership).Result;

            // Assert
            Assert.AreEqual(WorkItemsCount, workItems.Count);

            var i = 1;
            foreach (WorkItem workItem in workItems)
            {
				Assert.AreEqual(i, workItem.Id);
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
            var membership = CreateMembership();
            var workItemId = WorkItemsCount + 1;
            var workItem = CreateSetupWorkItem(index: workItemId, type: WorkItemType.Discussion);

            // Act
            var workItemReturned = WorkItemRepository.CreateAsync(workItem, membership).Result;

			// Assert
			Assert.AreEqual(workItemId, workItem.Id);
			Assert.AreEqual(string.Format(Title, workItemId), workItem.Title);
			Assert.AreEqual(string.Format(Summary, workItemId), workItem.Summary);
			Assert.AreEqual(string.Format(Body, workItemId), workItem.Body);
			Assert.AreEqual(WorkItemType.Discussion, workItem.Type);
			Assert.AreEqual(membership.UserId, workItem.UserId);
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
                var membership = CreateMembership();
                titleInvalid = CreateSetupWorkItem(index: WorkItemsCount);
                titleInvalid.Title = "";
                var workItemException = WorkItemRepository.CreateAsync(titleInvalid, membership).Result;
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
				var membership = CreateMembership();
				summaryInvalid = CreateSetupWorkItem(index: WorkItemsCount);
				summaryInvalid.Summary = "";
				var workItemException = WorkItemRepository.CreateAsync(summaryInvalid, membership).Result;
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
                var membership = CreateMembership();
                bodyInvalid = CreateSetupWorkItem(index: WorkItemsCount);
                bodyInvalid.Body = "";
                var workItemException = WorkItemRepository.CreateAsync(bodyInvalid, membership).Result;
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
				var membership = CreateMembership();
				typeInvalid = CreateSetupWorkItem(index: WorkItemsCount, type: WorkItemType.Default);
				var workItemException = WorkItemRepository.CreateAsync(typeInvalid, membership).Result;
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
            var membership = CreateMembership();
            var workItemId = 1;
            var workItem = WorkItemRepository.GetAsync(workItemId, membership).Result;

            // Act
            var workItemReturned = WorkItemRepository.UpdateAsync(workItem, membership).Result;

            // Assert
            Assert.AreEqual(workItemId, workItemReturned.Id);
            Assert.AreEqual(string.Format(Title, workItemId), workItemReturned.Title);
            Assert.AreEqual(string.Format(Summary, workItemId), workItemReturned.Summary);
            Assert.AreEqual(string.Format(Body, workItemId), workItemReturned.Body);
            Assert.AreEqual(Status.Updated, workItemReturned.Status);
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
                var membership = CreateMembership();
                var workItem = CreateSetupWorkItem(index: WorkItemsCount + 1);
                var workItemException = WorkItemRepository.UpdateAsync(workItem, membership).Result;
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
                var membership = CreateMembership();
                membership.UserId = 2;
                var workItem = CreateSetupWorkItem(index: WorkItemsCount);
                var workItemException = WorkItemRepository.UpdateAsync(workItem, membership).Result;
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
                var membership = CreateMembership();
                titleInvalid = CreateSetupWorkItem(index: WorkItemsCount);
                titleInvalid.Title = "";
                var workItemException = WorkItemRepository.UpdateAsync(titleInvalid, membership).Result;
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
                var membership = CreateMembership();
                summaryInvalid = CreateSetupWorkItem(index: WorkItemsCount);
                summaryInvalid.Summary = "";
                var summaryException = WorkItemRepository.UpdateAsync(summaryInvalid, membership).Result;
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
				var membership = CreateMembership();
				bodyInvalid = CreateSetupWorkItem(index: WorkItemsCount);
				bodyInvalid.Body = "";
				var bodyException = WorkItemRepository.UpdateAsync(bodyInvalid, membership).Result;
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
			WorkItem typeInvalid = null;
			try
			{
				var membership = CreateMembership();
				typeInvalid = CreateSetupWorkItem(index: WorkItemsCount);
				typeInvalid.Type = WorkItemType.Event;
				var typeException = WorkItemRepository.UpdateAsync(typeInvalid, membership).Result;
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
            var membership = CreateMembership();
            var workItemId = 1;

            // Act
            WorkItemRepository.DeleteAsync(workItemId, membership);

            WorkItem workItemException = null;
            try
            {
                workItemException = WorkItemRepository.GetAsync(workItemId, membership).Result;
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
                var membership = CreateMembership();
                WorkItemRepository.DeleteAsync(WorkItemsCount + 1, membership).Wait();
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
                var membership = CreateMembership();
                membership.UserId = 2;
                WorkItemRepository.DeleteAsync(WorkItemsCount, membership).Wait();
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
        /// <param name="index">The work item index.</param>
        public WorkItem CreateSetupWorkItem(long? index = 0, long userId = 0, WorkItemType type = WorkItemType.Default)
        {
            return new WorkItem
            {
                Id = index.Value,
                Title = string.Format(this.Title, index),
                Summary = string.Format(this.Summary, index),
                Body = string.Format(this.Body, index),
				Type = type,
				Status = Status.Created,
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