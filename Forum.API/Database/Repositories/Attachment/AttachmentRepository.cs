using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emsa.Mared.Common;
using Emsa.Mared.Common.Database;
using Emsa.Mared.Discussions.API.Database.Repositories.Discussions;
using Emsa.Mared.Discussions.API.Database.Repositories.Participants;
using Microsoft.EntityFrameworkCore;

namespace Emsa.Mared.Discussions.API.Database.Repositories.Attachments
{
    /// <inheritdoc />
    public class AttachmentRepository : IAttachmentRepository
    {
        #region [Properties]
        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        private readonly DiscussionContext _context;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="AttachmentRepository"/> class.
        /// </summary>
        /// 
        /// <param name="context">The context.</param>
        public AttachmentRepository(DiscussionContext context)
        {
            _context = context;
        }
        #endregion

        #region [Methods] IRepository
        /// <inheritdoc />
        public async Task<Attachment> Create(Attachment attachmentToCreate, UserMembership membership = null)
        {
            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == attachmentToCreate.DiscussionId);
            if (discussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);
            if (discussion.UserId != membership.UserId)
                throw new ModelException(Discussion.DoesNotExist, true);

            await _context.Attachments.AddAsync(attachmentToCreate);
            await _context.SaveChangesAsync();

            return attachmentToCreate;
        }

        /// <inheritdoc />
        public async Task Delete(long attachmentId, UserMembership membership = null)
        {
            var attachment = await GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == attachmentId);

            if (attachment == null)
                throw new ModelException(Participant.DoesNotExist, true);

            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == attachment.DiscussionId);

            if (discussion.UserId != membership.UserId)
                throw new ModelException(Discussion.DoesNotExist, true);

            _context.Attachments.Remove(attachment);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public Task<bool> Exists(long entityId, UserMembership membership = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<Attachment> Get(long attachmentId, UserMembership membership = null)
        {
            var attachment = await GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == attachmentId);

            if (attachment == null)
                throw new ModelException(Participant.DoesNotExist, true);

            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == attachment.DiscussionId);

            if (discussion.UserId != membership.UserId)
                throw new ModelException(Discussion.DoesNotExist, true);

            return attachment;
        }

        /// <inheritdoc />
        public async Task<List<Attachment>> GetByDiscussion(long discussionId, AttachmentParameters parameters, UserMembership membership = null)
        {
            var discussion = await _context.Discussions
                .FirstOrDefaultAsync(x => x.Id == discussionId);

            if (discussion == null)
                throw new ModelException(Discussion.DoesNotExist, true);
            if (discussion.UserId != membership.UserId)
                throw new ModelException(Discussion.DoesNotExist, true);

            var attachments = GetQueryableByDiscussion(discussionId);

            return await PagedList<Attachment>.CreateAsync(attachments, parameters.PageNumber, parameters.PageSize);
        }

        /// <inheritdoc />
        public async Task<List<Attachment>> GetAll(string name = null, UserMembership membership = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task<PagedList<Attachment>> GetAll(AttachmentParameters parameters, UserMembership membership = null)
        {
            if (parameters == null)
            {
                parameters = new AttachmentParameters();
            }
            var attachments = GetQueryable()
                .Include(d => d.Discussion)
                .Where(i => i.DiscussionId == i.Discussion.Id
                    && i.Discussion.UserId == membership.UserId);

            return await PagedList<Attachment>.CreateAsync(attachments, parameters.PageNumber, parameters.PageSize);

        }

        /// <inheritdoc />
        public async Task<Attachment> Update(Attachment attachment, UserMembership membership = null)
        {
            var attachmentToUpdate = await GetQueryable()
                .FirstOrDefaultAsync(x => x.Id == attachment.Id);

            if (attachmentToUpdate == null)
                throw new ModelException(Participant.DoesNotExist, true);

            attachmentToUpdate = attachment;

            await _context.SaveChangesAsync();

            return attachmentToUpdate;
        }
        #endregion

        #region [Methods] Utility
        /// <summary>
        /// Gets the queryable.
        /// </summary>
        private IQueryable<Attachment> GetQueryable()
        {
            return _context.Attachments;
        }

        /// <summary>
        /// Gets the queryable by discussion.
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
        private IQueryable<Attachment> GetQueryableByDiscussion(long discussionId)
        {
            return GetQueryable()
                .Where(p => p.DiscussionId == discussionId);
        }
        #endregion
    }
}
