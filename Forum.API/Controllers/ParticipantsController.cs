using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Discussions.API.Contracts.Participants;
using Emsa.Mared.Common;
using Emsa.Mared.Discussions.API.Database.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Discussions.API.Controllers
{
    /// <summary>
    /// The participant api controller allows to create, get, update and delete responses from discussions.
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

        #region [Methods] Utility
        /// <summary>
        /// Create a discussion response in repository.
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
        /// <param name="participantToCreateDto">The response information.</param>
        [HttpPost]
        public async Task<IActionResult> Create(long discussionId, ParticipantToCreateDto participantToCreateDto)
        {
            var membership = new UserMembership
            {
                UserId = 1,
                GroupIds = new long[0],//new long[] { 1 }, //
                OrganizationsIds = new long[0]
            };

            var participant = _mapper.Map<Participant>(participantToCreateDto);

            participant.DiscussionId = discussionId;

            var participantCreated = await _repo.Create(participant, membership);

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
            var membership = new UserMembership
            {
                UserId = 10,
                GroupIds = new long[] { 1 }, //new long[0],
                OrganizationsIds = new long[0]
            };

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
        /// Get all responses. 
        /// </summary>
        /// 
		/// <param name="parameters">The parameters.</param>
        [HttpGet("/api/discussions/participants")]
        public async Task<IActionResult> GetAll([FromQuery] ParticipantParameters parameters = null)
        {
            var membership = new UserMembership
            {
                UserId = 10,
                GroupIds = new long[] { 1 }, //new long[0],
                OrganizationsIds = new long[0]
            };

            var participants = await _repo.GetAll(parameters: null, membership: membership);

            var participantsToReturn = _mapper.Map<IEnumerable<ParticipantToReturnDto>>(participants);

            return Ok(participantsToReturn);
        }

        /// <summary>
        /// Get a response by id. 
        /// </summary>
        /// 
        /// <param name="id">The response id.</param>
        [HttpGet("{participantId:long}")]
        public async Task<IActionResult> Get(long id)
        {
            var membership = new UserMembership
            {
                UserId = 10,
                GroupIds = new long[] { 1 }, //new long[0],
                OrganizationsIds = new long[0]
            };

            var participant = await _repo.Get(id, membership);

            var participantToReturn = _mapper.Map<ParticipantToReturnDto>(participant);

            return Ok(participantToReturn);
        }

        /// <summary>
        /// Update a response in repository. 
        /// </summary>
        /// 
        /// <param name="id">The response id.</param>
		/// <param name="participantToUpdateDto">The response information.</param>
        [HttpPut("{participantId:long}")]
        public async Task<IActionResult> Update(long id, ParticipantToUpdateDto participantToUpdateDto)
        {
            var membership = new UserMembership
            {
                UserId = 10,
                GroupIds = new long[] { 1 }, //new long[0],
                OrganizationsIds = new long[0]
            };

            var updateParticipant = _mapper.Map<Participant>(participantToUpdateDto);

            updateParticipant.Id = id;

            var participantUpdated = await _repo.Update(updateParticipant, membership);

            return Created(new Uri($"{Request.GetDisplayUrl()}/{participantUpdated.Id}"), participantUpdated);
        }

        /// <summary>
        /// Delete a response in repository. 
        /// </summary>
        /// 
        /// <param name="discussionId">The discussion id.</param>
        /// <param name="participantId">The response id.</param>
        [HttpDelete("{participantId:long}")]
        public async Task<IActionResult> Delete(long discussionId, long participantId)
        {
            var membership = new UserMembership
            {
                UserId = 1,
                GroupIds = new long[0],
                OrganizationsIds = new long[0]
            };

            await _repo.Delete(participantId, membership);

            return Ok();
        }
        #endregion
    }
}