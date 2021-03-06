﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using Emsa.Mared.Common.Database.Utility;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Pagination;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Common.Extensions;
using Emsa.Mared.Discussions.API.Database.Repositories.Participants;
using Microsoft.EntityFrameworkCore;

namespace Emsa.Mared.Discussions.API.Database.Repositories.Discussions
{
    /// <inheritdoc />
    public class DiscussionRepository : IDiscussionRepository
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        private readonly DiscussionContext _context;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscussionRepository"/> class.
        /// </summary>
        /// 
        /// <param name="context">The context.</param>
        public DiscussionRepository(DiscussionContext context)
        {
            _context = context;
        }
        #endregion

        #region [Methods] IRepository
        /// <inheritdoc />
        public async Task<Discussion> CreateAsync(Discussion discussionToCreate, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			if (string.IsNullOrWhiteSpace(discussionToCreate.Comment))
                throw new ModelException(discussionToCreate.InvalidFieldMessage(p => p.Comment));
            if (string.IsNullOrWhiteSpace(discussionToCreate.Subject))
                throw new ModelException(discussionToCreate.InvalidFieldMessage(p => p.Subject));

            discussionToCreate.UserId = membership.UserId;

            var participants = new List<Participant>();
            var participant = new Participant
            {
                EntityId = discussionToCreate.UserId,
                EntityType = EntityType.User
            };
            //var participant2 = new Participant
            //{
            //    EntityId = 1,
            //    EntityType = EntityType.Group
            //};
            participants.Add(participant);
            //participants.Add(participant2);

            discussionToCreate.CreatedDate = DateTime.Now;
            discussionToCreate.Status = "Created";
            discussionToCreate.Participants = participants;

            await _context.Discussions.AddAsync(discussionToCreate);
            await _context.SaveChangesAsync();

            return discussionToCreate;
        }

        /// <inheritdoc />
        public async Task<Discussion> GetAsync(long id, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var discussion = await GetBasicQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (discussion == null)
                throw new ModelException(Discussion.DoesNotExist, missingResource: true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			return discussion;
        }

        /// <inheritdoc />
        public async Task<Discussion> UpdateAsync(Discussion updateDiscussion, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var databaseDiscussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == updateDiscussion.Id);

			if (databaseDiscussion == null)
				throw new ModelException(Discussion.DoesNotExist, missingResource: true);
			if (databaseDiscussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			if (string.IsNullOrWhiteSpace(updateDiscussion.Subject))
                throw new ModelException(updateDiscussion.InvalidFieldMessage(p => p.Subject));
            if (string.IsNullOrWhiteSpace(updateDiscussion.Comment))
                throw new ModelException(updateDiscussion.InvalidFieldMessage(p => p.Comment));

            databaseDiscussion.Subject = updateDiscussion.Subject;
            databaseDiscussion.Comment = updateDiscussion.Comment;
            databaseDiscussion.UpdatedDate = DateTime.Now;
            databaseDiscussion.Status = "Updated";

            await _context.SaveChangesAsync();

            return databaseDiscussion;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(long id, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var discussion = await GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);

			if (discussion == null)
				throw new ModelException(Discussion.DoesNotExist, missingResource: true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			discussion.Status = "Removed";

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<PagedList<Discussion>> GetAllAsync(DiscussionParameters parameters, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			if (parameters == null)
            {
                parameters = new DiscussionParameters();
            }

			var discussions = GetBasicQueryable();
			var count = await discussions.CountAsync();

			return await PagedList<Discussion>.CreateAsync(discussions, parameters.PageNumber, parameters.PageSize, count);
        }

        /// <inheritdoc />
        public Task<bool> ExistsAsync(long entityId, UserMembership membership = null)
        {
            throw new NotImplementedException();
        }
		#endregion

		#region [Methods] Utility
		/// <summary>
		/// Gets the queryable.
		/// </summary>
		private IQueryable<Discussion> GetQueryable()
		{
			return _context.Discussions
				.Where(s => s.Status != "Removed");
		}

		/// <summary>
		/// Gets the basic queryable.
		/// </summary>
		private IQueryable<Discussion> GetBasicQueryable(UserMembership membership = null)
		{
			var queryable = GetQueryable();

			if (membership != null)
			{
				queryable = queryable
					.Where(d => d.Participants.Any(p => (membership.UserId == p.EntityId && p.EntityType == EntityType.User)
					|| (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group)
					|| (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)));
			}

			return queryable;
		}
		#endregion
	}
}