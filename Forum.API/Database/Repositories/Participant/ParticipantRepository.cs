using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emsa.Mared.Common.Database;

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
        public Task<Participant> Create(Participant entity)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<bool> Exists(long entityId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<Participant> Get(long entityId)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<List<Participant>> GetAll()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<PagedList<Participant>> GetAll(ParticipantParameters parameters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task<Participant> Update(Participant entity)
        {
            throw new NotImplementedException();
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
        #endregion
    }
}