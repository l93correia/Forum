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
        public async Task<Discussion> Create(Discussion discussionToCreate, UserMembership membership = null)
        {
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
            var participant2 = new Participant
            {
                EntityId = 1,
                EntityType = EntityType.Group
            };
            participants.Add(participant);
            participants.Add(participant2);

            discussionToCreate.CreatedDate = DateTime.Now;
            discussionToCreate.Status = "Created";
            discussionToCreate.Participants = participants;

            await _context.Discussions.AddAsync(discussionToCreate);
            await _context.SaveChangesAsync();

            return discussionToCreate;
        }

        /// <inheritdoc />
        public async Task<List<Discussion>> GetAll(string name = null, UserMembership membership = null)
        {
            var discussions = GetQueryable()
                .Where(d => d.Participants.Any(p => (membership.UserId == p.EntityId && p.EntityType == EntityType.User) 
                || (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group) 
                || (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)));

            return await discussions.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Discussion> Get(long id, UserMembership membership = null)
        {
            var discussion = GetQueryable()
                .Where(d => d.Participants.Any(p => (membership.UserId == p.EntityId && p.EntityType == EntityType.User)
                || (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group)
                || (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)))
                .FirstOrDefaultAsync(x => x.Id == id);

            if (discussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);

            return await discussion;
        }

        /// <inheritdoc />
        public async Task<Discussion> Update(Discussion updateDiscussion, UserMembership membership = null)
        {
            var databaseDiscussion = await _context.Discussions
                .Where(d => d.UserId == membership.UserId)
                .FirstOrDefaultAsync(x => x.Id == updateDiscussion.Id);

            if (databaseDiscussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);

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
        public async Task Delete(long id, UserMembership membership = null)
        {
            var discussion = await GetQueryable()
                .Where(d => d.UserId == membership.UserId)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (discussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);            

            discussion.Status = "Removed";

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<PagedList<Discussion>> GetAll(DiscussionParameters parameters, UserMembership membership = null)
        {
            if(parameters == null)
            {
                parameters = new DiscussionParameters();
            }
            var discussions = GetQueryable()
                .Where(d => d.Participants.Any(p => (membership.UserId == p.EntityId && p.EntityType == EntityType.User)
                || (membership.GroupIds.Contains(p.EntityId) && p.EntityType == EntityType.Group)
                || (membership.OrganizationsIds.Contains(p.EntityId) && p.EntityType == EntityType.Organization)));

            return await PagedList<Discussion>.CreateAsync(discussions, parameters.PageNumber, parameters.PageSize);
        }

        /// <inheritdoc />
        public Task<bool> Exists(long entityId, UserMembership membership = null)
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
                .Include(d => d.Responses)
                .Include(p => p.Participants)
                .Where(s => s.Status != "Removed");
        }
        #endregion
    }
}