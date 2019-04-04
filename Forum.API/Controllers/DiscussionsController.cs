using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Forum.API.Data;
using Forum.API.Dtos;
using Forum.API.Models;
using Forum.API.Models.Repository.Discussions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.API.Controllers
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

        #region [Methods] Utility
        /// <summary>
        /// Get all discussions in repository.
        /// </summary>
        /// 
        /// <param name="parameters">The parameters.</param>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DiscussionParameters parameters = null)
        {
            var discussions = await _repo.GetAll();

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
            var discussion = await _repo.Get(id);

            if (discussion == null)
            {
                return BadRequest("Not found");
            }

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
            var discussion = _mapper.Map<Discussion>(discussionToCreateDto);
            var discussionCreated = await _repo.Create(discussion);
            
            return Created(new Uri($"{Request.GetDisplayUrl()}/{discussionCreated.Id}"), discussionCreated);
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
            var updateDiscussion = _mapper.Map<Discussion>(discussionToCreateDto);

            updateDiscussion.Id = id;

            var discussionUpdated = await _repo.Update(updateDiscussion);

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
            await _repo.Delete(id);

            //TODO hide a discussion, do not remove from db

            return Ok();
        }
        #endregion

    }
}