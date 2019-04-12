using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Emsa.Mared.Common;
using Emsa.Mared.Common.Security;
using Emsa.Mared.Discussions.API.Contracts.Participants;
using Emsa.Mared.Discussions.API.Database.Repositories;
using Emsa.Mared.Discussions.API.Database.Repositories.Participants;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Discussions.API.Controllers
{
    /// <summary>
    /// The participant api controller allows to create, get, update and delete participants from discussions.
    /// </summary>
    /// 
    /// <seealso cref="ControllerBase" />
    [Route("api/discussions/[controller]")]
    [Route("api/discussions/{discussionId:long}/[controller]")]
    [ApiController]
    public class ParticipantsController : ControllerBase
    {
        #region [Attributes]
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IParticipantRepository _repo;

        /// <summary>
        /// The mapper.
        /// </summary>
        private readonly IMapper _mapper;
        #endregion

        #region [Constructors]
        /// <summary>
        /// Initializes a new instance of the <see cref="ParticipantsController"/> class.
        /// </summary>
        /// 
        /// <param name="mapper">The mapper.</param>
        /// <param name="repo">The repository.</param>
        public ParticipantsController(IParticipantRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
		#endregion

		#region [Interface Methods]
		/// <summary>
		/// Create a discussion participant in repository.
		/// </summary>
		/// 
		/// <param name="discussionId">The discussion id.</param>
		/// <param name="participantToCreateDto">The participant information.</param>
		[HttpPost]
        public async Task<IActionResult> Create(long discussionId, ParticipantToCreateDto participantToCreateDto)
        {
			var membership = CreateMembership();

			var participant = _mapper.Map<Participant>(participantToCreateDto);

            participant.DiscussionId = discussionId;

            var participantCreated = await _repo.CreateAsync(participant, membership);

            var participantToReturn = _mapper.Map<ParticipantToReturnDto>(participantCreated);

            return Created(new Uri($"{Request.GetDisplayUrl()}/{participantToReturn.Id}"), participantToReturn
);
        }

        /// <summary>
        /// Get all discussion participants. 
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
		/// <param name="parameters">The parameters.</param>
        [HttpGet("/api/discussions/{discussionId:long}/participants")]
        public async Task<IActionResult> GetAllByDiscussion(long discussionId, [FromQuery] ParticipantParameters parameters = null)
		{
			var membership = CreateMembership();

			var participants = await _repo.GetByDiscussion(discussionId, parameters, membership);

            var participantToReturn = participants.Select(p => _mapper.Map<ParticipantToReturnDto>(p));

            //this.Response.AddHeader("Pagination", new PaginationHeader
            //(
            //    responses.PageNumber,
            //    responses.PageSize,
            //    responses.TotalPages,
            //    responses.TotalCount
            //));

            return Ok(participantToReturn);
        }

		/// <summary>
		/// Get a participant by id. 
		/// </summary>
		/// 
		/// <param name="discussionId">The discussion id.</param>
		/// <param name="participantId">The participant id.</param>
		[HttpGet("/api/discussions/{discussionId:long}/participants/{participantId:long}")]
		public async Task<IActionResult> Get(long discussionId, long participantId)
        {
			var membership = CreateMembership();

			var participant = await _repo.GetAsync(participantId, membership);

            var participantToReturn = _mapper.Map<ParticipantToReturnDto>(participant);

            return Ok(participantToReturn);
        }

        /// <summary>
        /// Update a participant in repository. 
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
        /// <param name="participantId">The participant id.</param>
		/// <param name="participantToUpdateDto">The participant information.</param>
        [HttpPut("/api/discussions/{discussionId:long}/participants/{participantId:long}")]
        public async Task<IActionResult> Update(long discussionId, long participantId, ParticipantToUpdateDto participantToUpdateDto)
        {
			var membership = CreateMembership();

			var updateParticipant = _mapper.Map<Participant>(participantToUpdateDto);

            updateParticipant.Id = participantId;
            updateParticipant.DiscussionId = discussionId;

            var participantUpdated = await _repo.UpdateAsync(updateParticipant, membership);

            var participantToReturn = _mapper.Map<ParticipantToReturnDto>(participantUpdated);

            return Created(new Uri($"{Request.GetDisplayUrl()}/{participantToReturn.Id}"), participantToReturn);
        }

		/// <summary>
		/// Delete a participant in repository. 
		/// </summary>
		/// 
		/// <param name="discussionId">The discussion id.</param>
		/// <param name="participantId">The participant id.</param>
		[HttpDelete("/api/discussions/{discussionId:long}/participants/{participantId:long}")]
		public async Task<IActionResult> Delete(long discussionId, long participantId)
        {
			var membership = CreateMembership();

            await _repo.DeleteAsync(participantId, membership);

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