using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using Emsa.Mared.Common.Database.Utility;
using Emsa.Mared.Common.Exceptions;
using Emsa.Mared.Common.Pagination;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Discussions.API.Database.Repositories.Discussions;
using Microsoft.EntityFrameworkCore;

namespace Emsa.Mared.Discussions.API.Database.Repositories.Participants
{
    /// <inheritdoc />
    public class ParticipantRepository : IParticipantRepository
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        private readonly DiscussionContext _context;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="ParticipantRepository"/> class.
        /// </summary>
        /// 
        /// <param name="context">The context.</param>
        public ParticipantRepository(DiscussionContext context)
        {
            _context = context;
        }
        #endregion

        #region [Methods] IRepository
        /// <inheritdoc />
        public async Task<Participant> CreateAsync(Participant participantToCreate, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == participantToCreate.DiscussionId);

            if (discussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);
            if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			await _context.Participants.AddAsync(participantToCreate);
            await _context.SaveChangesAsync();

            return participantToCreate;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(long participantId, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var participant = await GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == participantId);

			if (participant == null)
				throw new ModelException(Participant.DoesNotExist, missingResource: true);

			var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == participant.DiscussionId);

			if (discussion == null)
				throw new ModelException(Discussion.DoesNotExist, missingResource: true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			_context.Participants.Remove(participant);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task<bool> ExistsAsync(long entityId, UserMembership membership = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<Participant> GetAsync(long participantId, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var participant = await GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == participantId);

            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == participant.DiscussionId);

			if (discussion == null)
				throw new ModelException(Discussion.DoesNotExist, missingResource: true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			return participant;
        }

        /// <inheritdoc />
        public async Task<PagedList<Participant>> GetAllAsync(ParticipantParameters parameters, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			if (parameters == null)
            {
                parameters = new ParticipantParameters();
            }
			var participants = GetCompleteQueryable();
			var count = await GetBasicQueryable().CountAsync();

			return await PagedList<Participant>.CreateAsync(participants, parameters.PageNumber, parameters.PageSize, count);
        }

        /// <inheritdoc />
        public async Task<Participant> UpdateAsync(Participant participant, UserMembership membership = null)
        {
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var participantToUpdate = await GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == participant.Id);

            if (participantToUpdate == null)
                throw new ModelException(Participant.DoesNotExist, true);

            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == participant.DiscussionId);
			if (discussion == null)
				throw new ModelException(Discussion.DoesNotExist, missingResource: true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			participantToUpdate.EntityId = participant.EntityId;
            participantToUpdate.EntityType = participant.EntityType;

            await _context.SaveChangesAsync();

            return participantToUpdate;
        }
		#endregion

		#region [Methods] IParticipantRepository
		/// <inheritdoc />
		public async Task<List<Participant>> GetByDiscussion(long discussionId, ParticipantParameters parameters, UserMembership membership = null)
		{
			if (membership == null)
				throw new ModelException(String.Format(Constants.IsInvalidMessageFormat, nameof(membership)));
			var discussion = await _context.Discussions
				.FirstOrDefaultAsync(x => x.Id == discussionId);

			if (discussion == null)
				throw new ModelException(Discussion.DoesNotExist, missingResource: true);
			if (discussion.UserId != membership.UserId)
				throw new ModelException(string.Empty, unauthorizedResource: true);

			var participants = GetQueryableByDiscussion(discussionId);
			var count = await participants.CountAsync();

			return await PagedList<Participant>.CreateAsync(participants, parameters.PageNumber, parameters.PageSize, count);
		}

		#endregion

		#region [Methods] Utility
		/// <summary>
		/// Gets the queryable.
		/// </summary>
		private IQueryable<Participant> GetQueryable()
        {
            return _context.Participants;
        }

		/// <summary>
		/// Gets the basic queryable.
		/// </summary>
		private IQueryable<Participant> GetBasicQueryable(UserMembership membership = null)
		{
			var queryable = GetQueryable();

			if (membership != null)
			{
				queryable = queryable
					.Where(i => i.DiscussionId == i.Discussion.Id
					&& i.Discussion.UserId == membership.UserId);
			}

			return queryable;
		}

		/// <summary>
		/// Gets the complete queryable.
		/// </summary>
		private IQueryable<Participant> GetCompleteQueryable(UserMembership membership = null)
		{
			return GetBasicQueryable(membership)
				.Include(d => d.Discussion);
		}

		/// <summary>
		/// Gets the queryable by discussion.
		/// </summary>
		/// 
		/// <param name="discussionId">The discussion id.</param>
		private IQueryable<Participant> GetQueryableByDiscussion(long discussionId)
        {
            return GetQueryable()
                .Where(p => p.DiscussionId == discussionId);
        }
        #endregion
    }
}