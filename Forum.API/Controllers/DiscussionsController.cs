using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emsa.Mared.Common;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Discussions.API.Contracts;
using Emsa.Mared.Discussions.API.Database.Repositories;
using Emsa.Mared.Discussions.API.Database.Repositories.Discussions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Emsa.Mared.Discussions.API.Controllers
{
    /// <summary>
    /// The discussion api controller allows to create, get, update and delete discussions.
    /// </summary>
    /// 
    /// <seealso cref="ControllerBase" />
    [Route("api/[controller]")]
    [ApiController]
    public class DiscussionsController : ControllerBase
    {
        #region [Attributes]
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IDiscussionRepository _repo;

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper _mapper;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscussionsController"/> class.
        /// </summary>
        /// 
        /// <param name="mapper">The mapper.</param>
        /// <param name="repo">The repository.</param>
        public DiscussionsController(IDiscussionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
		#endregion

		#region [Interface Methods]
		/// <summary>
		/// Get all discussions in repository.
		/// </summary>
		/// 
		/// <param name="parameters">The parameters.</param>
		[HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DiscussionParameters parameters = null)
        {
            var membership = CreateMembership();

			var discussions = await _repo.GetAllAsync(parameters: null, membership: membership);

            var discussionsToReturn = _mapper.Map<IEnumerable<DiscussionForListDto>>(discussions);

            return Ok(discussionsToReturn);
        }


        /// <summary>
        /// Get a discussion by id.
        /// </summary>
        /// 
        /// <param name="id">The discussion id.</param>
		/// <param name="parameters">The parameters.</param>
        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id, [FromQuery] DiscussionParameters parameters = null)
        {
            var membership = CreateMembership();

			var discussion = await _repo.GetAsync(id, membership);

            var discussionToReturn = _mapper.Map<DiscussionToReturnDto>(discussion);

            return Ok(discussionToReturn);
        }

        /// <summary>
        /// Create a discussions in repository.
        /// </summary>
        /// 
        /// <param name="discussionToCreateDto">The discussion information.</param>
        [HttpPost]
        public async Task<IActionResult> Create(DiscussionToCreateDto discussionToCreateDto)
        {
            var membership = CreateMembership();
			var discussion = _mapper.Map<Discussion>(discussionToCreateDto);
            var discussionCreated = await _repo.CreateAsync(discussion, membership);

            var discussionToReturn = _mapper.Map<DiscussionToReturnDto>(discussionCreated);

            return Created(new Uri($"{Request.GetDisplayUrl()}/{discussionToReturn.Id}"), discussionToReturn);
        }

        /// <summary>
        /// Update a discussions in repository.
        /// </summary>
        /// 
        /// <param name="id">The Discussion Id.</param>
        /// <param name="discussionToCreateDto">The discussion information.</param>
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, UpdateDiscussionDto discussionToCreateDto)
        {
            var membership = CreateMembership();

			var updateDiscussion = _mapper.Map<Discussion>(discussionToCreateDto);

            updateDiscussion.Id = id;

            var discussionUpdated = await _repo.UpdateAsync(updateDiscussion, membership);

            return Created(new Uri($"{Request.GetDisplayUrl()}/{discussionUpdated.Id}"), discussionUpdated);
        }

        /// <summary>
        /// Delete a discussions in repository.
        /// </summary>
        /// 
        /// <param name="id">The Discussion Id.</param>
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var membership = CreateMembership();

			await _repo.DeleteAsync(id, membership);

            return Ok();
        }
		#endregion

		#region [Methods] Utility
		/// <summary>
		/// Create a membership.
		/// </summary>
		public UserMembership CreateMembership()
		{
			long userId = 1;
			long[] groupsIds = new long[0];
			long[] organizationIds = new long[0];

			var headers = this.Request.Headers;
			if (headers.TryGetValue("UserId", out StringValues values))
			{
				userId = long.Parse(values.FirstOrDefault());
			}
			if (headers.TryGetValue("GroupId", out values))
			{
				groupsIds = values.Select(value => long.Parse(value)).ToArray();
			}
			if (headers.TryGetValue("OrganizationId", out values))
			{
				organizationIds = values.Select(value => long.Parse(value)).ToArray();
			}

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