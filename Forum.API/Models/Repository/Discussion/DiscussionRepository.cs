using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emsa.Mared.Common.Models;
using Forum.API.Dtos;
using Forum.API.Models;
using Forum.API.Models.Repository.Discussion;
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
        public async Task<Discussions> Create(Discussions discussionToCreate)
        {
            if (string.IsNullOrWhiteSpace(discussionToCreate.Comment))
                throw new ModelException(discussionToCreate.InvalidFieldMessage(p => p.Comment));
            if (string.IsNullOrWhiteSpace(discussionToCreate.UserId.ToString()))
                throw new ModelException(discussionToCreate.InvalidFieldMessage(p => p.UserId));
            if (string.IsNullOrWhiteSpace(discussionToCreate.Subject))
                throw new ModelException(discussionToCreate.InvalidFieldMessage(p => p.Subject));

            await _context.Discussions.AddAsync(discussionToCreate);
            await _context.SaveChangesAsync();

            return discussionToCreate;
        }

        /// <inheritdoc />
        public async Task<List<Discussions>> GetAll()
        {
            var discusssions = this.GetQueryable();

            return await discusssions.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Discussions> Get(long id)
        {
            var discusssion = await _context.Discussions
                .Include(d => d.User)
                .Include(d => d.DiscussionResponses)
                .ThenInclude(r => r.CreatedBy)
                .FirstOrDefaultAsync(x => x.Id == id);

            return discusssion;
        }

        /// <inheritdoc />
        public async Task<Discussions> Update(Discussions updateDiscussion)
        {
            var databaseDiscussion = await _context.Discussions.FindAsync(updateDiscussion.Id);
            if (databaseDiscussion == null)
                throw new ModelException(Discussions.DoesNotExist, true);

            if (string.IsNullOrWhiteSpace(updateDiscussion.Comment))
                throw new ModelException(updateDiscussion.InvalidFieldMessage(p => p.Comment));

            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == updateDiscussion.Id);

            discussion.Comment = updateDiscussion.Comment;

            await _context.SaveChangesAsync();

            return discussion;
        }

        /// <inheritdoc />
        public async Task Delete(long id)
        {
            var databaseDiscussion = await _context.Discussions.FindAsync(id);
            if (databaseDiscussion == null)
                throw new ModelException(Discussions.DoesNotExist, true);

            var discussion = await _context.Discussions.FindAsync(id);

            _context.Discussions.Remove(discussion);

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<PagedList<Discussions>> GetAll(DiscussionParameters parameters)
        {
            var discussions = this.GetQueryable();

            return await PagedList<Discussions>.CreateAsync(discussions, parameters.PageNumber, parameters.PageSize);
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
        private IQueryable<Discussions> GetQueryable()
        {
            return _context.Discussions
                .Include(d => d.DiscussionResponses)
                .Include(d => d.User);
        }
        #endregion
    }
}