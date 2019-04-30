using Emsa.Mared.Common.Claims;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Security;
using Emsa.Mared.WorkItems.API.WorkItems.Database;
using Emsa.Mared.WorkItems.API.WorkItems.Database.Repositories.WorkItemComments;
using Emsa.Mared.WorkItems.API.WorkItems.Database.Repositories.WorkItemParticipants;
using Emsa.Mared.WorkItems.API.WorkItems.Database.Repositories.WorkItems;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emsa.Mared.WorkItems.API.WorkItems.Tests
{
	[TestFixture]
    public class TestComment
    {
        #region [Constants]
        /// <summary>
        /// The number of comments.
        /// </summary>
        private readonly long CommentsCount = 5;

        /// <summary>
        /// The comment text.
        /// </summary>
        private readonly string Comment = "Comment";

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
		private IWorkItemCommentRepository CommentRepository;

        /// <summary>
        /// The data context.
        /// </summary>
        private WorkItemsContext Context;

		/// <summary>
		/// The first comment id.
		/// </summary>
		private long FirstCommentId;

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
            this.CommentRepository = new WorkItemCommentRepository(Context, workItemRepository);

            

			var workItem = new WorkItem
			{
				Title = "Test Discussion",
				Summary = "Test discussion summary",
				Body = "Test discussion body",
				Type = WorkItemType.Discussion,
				Status = WorkItemStatus.Created,
				UserId = this.UserId,
				WorkItemComments = new List<WorkItemComment>
				{
					new WorkItemComment
					{
						Comment = this.Comment,
						UserId = this.UserId,
						Status = WorkItemCommentStatus.Created
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
			this.FirstCommentId = this.Context.WorkItemComments.Min(comment => comment.Id);

			var comments = new List<WorkItemComment>();
			for (int i = 1; i < this.CommentsCount; i++)
			{
				comments.Add(CreateComment());
			}
			this.Context.WorkItemComments.AddRange(comments);
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
				var parameters = new WorkItemCommentParameters
				{
					WorkItemId = this.Context.WorkItems.Count() + 1
				};
				var membership = this.CreateMembership();
				// Act
				var comments = this.CommentRepository.GetAllAsync(parameters, membership).Result;
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
			var parameters = new WorkItemCommentParameters
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
			var comments = this.CommentRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.CommentsCount, comments.Count);

			foreach (WorkItemComment comment in comments)
			{
				Assert.AreEqual(this.Comment, comment.Comment);
				Assert.AreEqual(this.WorkItemId, comment.WorkItemId);
			}
		}

		/// <summary>
		/// The participant get all test, from group claim.
		/// </summary>
		[Test]
		public void TestParticipantGroupGetAll()
		{
			var parameters = new WorkItemCommentParameters
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
			var comments = this.CommentRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.CommentsCount, comments.Count);

			foreach (WorkItemComment comment in comments)
			{
				Assert.AreEqual(this.Comment, comment.Comment);
				Assert.AreEqual(this.WorkItemId, comment.WorkItemId);
			}
		}

		/// <summary>
		/// The participant get all test, from organization claim.
		/// </summary>
		[Test]
		public void TestParticipantOrganizationGetAll()
		{
			var parameters = new WorkItemCommentParameters
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
			var comments = this.CommentRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.CommentsCount, comments.Count);

			foreach (WorkItemComment comment in comments)
			{
				Assert.AreEqual(this.Comment, comment.Comment);
				Assert.AreEqual(this.WorkItemId, comment.WorkItemId);
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
				var parameters = new WorkItemCommentParameters
				{
					WorkItemId = this.WorkItemId
				};
				var membership = this.CreateMembership();
				membership.UserId = 2;
				// Act
				var comments = this.CommentRepository.GetAllAsync(parameters, membership).Result;
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
				var parameters = new WorkItemCommentParameters
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
				var comments = this.CommentRepository.GetAllAsync(parameters, membership).Result;
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
		/// The participant get comment by id test.
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
			var comment = this.CommentRepository.GetAsync(this.FirstCommentId, membership).Result;

			// Assert
			Assert.AreEqual(this.Comment, comment.Comment);
			Assert.AreEqual(this.WorkItemId, comment.WorkItemId);
		}

		/// <summary>
		/// The participant get comment by id test, from group claim.
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
			var comment = this.CommentRepository.GetAsync(this.FirstCommentId, membership).Result;

			// Assert
			Assert.AreEqual(this.Comment, comment.Comment);
			Assert.AreEqual(this.WorkItemId, comment.WorkItemId);
		}

		/// <summary>
		/// The participant get comment by id test, from organization claim.
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
			var comment = this.CommentRepository.GetAsync(this.FirstCommentId, membership).Result;

			// Assert
			Assert.AreEqual(this.Comment, comment.Comment);
			Assert.AreEqual(this.WorkItemId, comment.WorkItemId);
		}

		/// <summary>
		/// The get by id test.
		/// </summary>
		[Test]
        public void TestGetById()
        {
            var membership = this.CreateMembership();
            // Act
            var comment = this.CommentRepository.GetAsync(this.FirstCommentId, membership).Result;

            // Assert
            Assert.AreEqual(this.Comment, comment.Comment);
            Assert.AreEqual(this.WorkItemId, comment.WorkItemId);
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
				var invalidId = this.Context.WorkItemComments.Max(c => c.Id) + 1;
				var comment = this.CommentRepository.GetAsync(invalidId, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemComment.DoesNotExist, modelException.Message);

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
                var comment = this.CommentRepository.GetAsync(this.FirstCommentId, membership).Result;
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
			var parameters = new WorkItemCommentParameters
			{
				WorkItemId = this.WorkItemId,
				PageSize = 20
			};
			var membership = this.CreateMembership();
			// Act
			var comments = this.CommentRepository.GetAllAsync(parameters, membership).Result;

			// Assert
			Assert.AreEqual(this.CommentsCount, comments.Count);

			foreach (WorkItemComment comment in comments)
			{
				Assert.AreEqual(this.Comment, comment.Comment);
			}
		}

		/// <summary>
		/// The create test.
		/// </summary>
		[Test]
        public void TestCreate()
        {
            var membership = this.CreateMembership();
            var comment = this.CreateComment();

            // Act
            var commentReturned = this.CommentRepository.CreateAsync(comment, membership).Result;

            // Assert
            Assert.AreEqual(this.Comment, commentReturned.Comment);
            Assert.AreEqual(this.WorkItemId, commentReturned.WorkItemId);
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
                var workItemInvalid = this.CreateComment();
                workItemInvalid.WorkItemId = WorkItemId + 1;
                var responseException = this.CommentRepository.CreateAsync(workItemInvalid, membership).Result;
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
			WorkItemComment userIdInvalid = null;
            try
            {
                var membership = this.CreateMembership();
                membership.UserId = 2;
                userIdInvalid = this.CreateComment();
                var commentException = this.CommentRepository.CreateAsync(userIdInvalid, membership).Result;
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
            var comment = this.CommentRepository.GetAsync(this.FirstCommentId, membership).Result;
			var commentField = "updated";
			comment.Comment = commentField;
            // Act
            var attchmentReturned = this.CommentRepository.UpdateAsync(comment, membership).Result;

            // Assert
            Assert.AreEqual(commentField, attchmentReturned.Comment);
            Assert.AreEqual(WorkItemCommentStatus.Updated, attchmentReturned.Status);
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
				var comment = this.CommentRepository.GetAsync(this.FirstCommentId, membership).Result;
				comment.Comment = "updated";
				

				var commentUpdated = this.CommentRepository.UpdateAsync(comment, membership).Result;
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
				var comment = this.CommentRepository.GetAsync(this.FirstCommentId, membership).Result;
				comment.Comment = "updated";
				comment.Id = this.Context.WorkItemComments.Max(a => a.Id) + 1;

				var commentException = this.CommentRepository.UpdateAsync(comment, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemComment.DoesNotExist, modelException.Message);

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
			this.CommentRepository.DeleteAsync(this.FirstCommentId, membership);

            try
            {
                var commentException = this.CommentRepository.GetAsync(this.CommentsCount, membership).Result;
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemComment.DoesNotExist, modelException.Message);

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
				this.CommentRepository.DeleteAsync(this.CommentsCount, membership).Wait();
            }
            catch (AggregateException exc)
            {
                if (exc.InnerException is ModelException modelException)
                {
                    Assert.AreEqual(WorkItemComment.DoesNotExist, modelException.Message);

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
				this.CommentRepository.DeleteAsync(this.FirstCommentId, membership).Wait();
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
        public WorkItemComment CreateComment()
        {
            return new WorkItemComment
			{
                Comment = this.Comment,
				UserId = this.UserId,
				WorkItemId = this.WorkItemId
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
