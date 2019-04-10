using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using Microsoft.EntityFrameworkCore;

namespace Emsa.Mared.Discussions.API.Database.Repositories
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
        public async Task<Participant> Create(Participant participantToCreate, UserMembership membership = null)
        {
            var discussion = _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == participantToCreate.DiscussionId);
            if (discussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);

            await _context.Participants.AddAsync(participantToCreate);
            await _context.SaveChangesAsync();

            return participantToCreate;
        }

        /// <inheritdoc />
        public async Task Delete(long participantId, UserMembership membership = null)
        {
            var participant = await GetQueryable()
                .Include(d => d.Discussion)
                .Where(i => i.DiscussionId == i.Discussion.Id
                    && i.Discussion.UserId == membership.UserId)
                .FirstOrDefaultAsync(x => x.Id == participantId);

            if (participant == null)
                throw new ModelException(Participant.DoesNotExist, true);

            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task<bool> Exists(long entityId, UserMembership membership = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<Participant> Get(long participantId, UserMembership membership = null)
        {
            var participant = await GetQueryable()
                .Include(d => d.Discussion)
                .Where(i => i.DiscussionId == i.Discussion.Id
                    && i.Discussion.UserId == membership.UserId)
                .FirstOrDefaultAsync(x => x.Id == participantId);

            if (participant == null)
                throw new ModelException(Participant.DoesNotExist, true);

            return participant;
        }

        /// <inheritdoc />
        public async Task<List<Participant>> GetByDiscussion(long discussionId, ParticipantParameters parameters, UserMembership membership = null)
        {
            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == discussionId);

            if (discussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);
            if (discussion.UserId != membership.UserId)
                throw new ModelException(Discussion.DoesNotExist, true);

            var participants = GetQueryableByDiscussion(discussionId);

            return await PagedList<Participant>.CreateAsync(participants, parameters.PageNumber, parameters.PageSize);
        }

        /// <inheritdoc />
        public Task<List<Participant>> GetAll(string name = null, UserMembership membership = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<PagedList<Participant>> GetAll(ParticipantParameters parameters, UserMembership membership = null)
        {
            if (parameters == null)
            {
                parameters = new ParticipantParameters();
            }
            var participants = GetQueryable()
                .Include(d => d.Discussion)
                .Where(i => i.DiscussionId == i.Discussion.Id
                    && i.Discussion.UserId == membership.UserId);

            return await PagedList<Participant>.CreateAsync(participants, parameters.PageNumber, parameters.PageSize);
        }

        /// <inheritdoc />
        public async Task<Participant> Update(Participant participant, UserMembership membership = null)
        {
            var participantToUpdate = await GetQueryable()
                .Include(d => d.Discussion)
                .Where(i => i.DiscussionId == i.Discussion.Id
                    && i.Discussion.UserId == membership.UserId)
                .FirstOrDefaultAsync(x => x.Id == participant.Id);

            if (participantToUpdate == null)
                throw new ModelException(Participant.DoesNotExist, true);

            participantToUpdate = participant;

            await _context.SaveChangesAsync();

            return participantToUpdate;
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