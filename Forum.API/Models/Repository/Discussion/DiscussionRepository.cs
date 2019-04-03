using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using Forum.API.Dtos;
using Forum.API.Models;
using Forum.API.Models.Repository.Discussions;
using Microsoft.EntityFrameworkCore;

namespace Forum.API.Data
{
    /// <inheritdoc />
    public class DiscussionRepository : IDiscussionRepository
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        private readonly DataContext _context;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscussionRepository"/> class.
        /// </summary>
        /// 
        /// <param name="context">The context.</param>
        public DiscussionRepository(DataContext context)
        {
            _context = context;
        }
        #endregion

        #region [Methods] IRepository
        /// <inheritdoc />
        public async Task<Discussion> Create(Discussion discussionToCreate)
        {
            if (string.IsNullOrWhiteSpace(discussionToCreate.Comment))
                throw new ModelException(discussionToCreate.InvalidFieldMessage(p => p.Comment));
            if (string.IsNullOrWhiteSpace(discussionToCreate.Subject))
                throw new ModelException(discussionToCreate.InvalidFieldMessage(p => p.Subject));

            var user = await _context.User
                .FirstOrDefaultAsync(x => x.Id == discussionToCreate.UserId);

            if (user == null)
                throw new ModelException(User.DoesNotExist, true);


            discussionToCreate.CreatedDate = DateTime.Now;
            discussionToCreate.Status = "Created";

            await _context.Discussions.AddAsync(discussionToCreate);
            await _context.SaveChangesAsync();

            return discussionToCreate;
        }

        /// <inheritdoc />
        public async Task<List<Discussion>> GetAll()
        {
            var discusssions = GetQueryable();

            return await discusssions.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Discussion> Get(long id)
        {
            var discusssion = await _context.Discussions
                .Include(d => d.User)
                .Include(d => d.DiscussionResponses)
                .ThenInclude(r => r.CreatedBy)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (discusssion == null)
                throw new ModelException(Discussion.DoesNotExist, true);

            return discusssion;
        }

        /// <inheritdoc />
        public async Task<Discussion> Update(Discussion updateDiscussion)
        {
            var databaseDiscussion = await _context.Discussions.FindAsync(updateDiscussion.Id);
            if (databaseDiscussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);

            if (string.IsNullOrWhiteSpace(updateDiscussion.Subject))
                throw new ModelException(updateDiscussion.InvalidFieldMessage(p => p.Subject));
            if (string.IsNullOrWhiteSpace(updateDiscussion.Comment))
                throw new ModelException(updateDiscussion.InvalidFieldMessage(p => p.Comment));

            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == updateDiscussion.Id);

            discussion.Comment = updateDiscussion.Comment;
            discussion.UpdatedDate = DateTime.Now;
            discussion.Status = "Updated";

            await _context.SaveChangesAsync();

            return discussion;
        }

        /// <inheritdoc />
        public async Task Delete(long id)
        {
            var databaseDiscussion = await _context.Discussions.FindAsync(id);
            if (databaseDiscussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);

            var discussion = await _context.Discussions.FindAsync(id);

            discussion.Status = "Removed";

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<PagedList<Discussion>> GetAll(DiscussionParameters parameters)
        {
            var discussions = GetQueryable();

            return await PagedList<Discussion>.CreateAsync(discussions, parameters.PageNumber, parameters.PageSize);
        }

        /// <inheritdoc />
        public Task<bool> Exists(long entityId)
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
                .Where(s => s.Status != "Removed")
                .Include(d => d.DiscussionResponses)
                .Include(d => d.User);
        }
        #endregion
    }
}