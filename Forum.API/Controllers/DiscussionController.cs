using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Forum.API.Data;
using Forum.API.Dtos;
using Forum.API.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscussionController : ControllerBase
    {
        private readonly IDiscussionRepository _repo;
        private readonly IMapper _mapper;

        public DiscussionController(IDiscussionRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // GET api/discussion
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var discussions = await _repo.GetAll();

            var discussionsToReturn = _mapper.Map<IEnumerable<DiscussionForListDto>>(discussions);

            return Ok(discussionsToReturn);
        }

        // GET api/discussion
        [HttpGet("{id:long}")]
        public async Task<IActionResult> Get(long id)
        {
            var discussion = await _repo.Get(id);

            //var responses = await _repo.GetResponses(discussion.Id);

            if (discussion == null)
            {
                return BadRequest("Not found");
            }

            var discussionToReturn = _mapper.Map<DiscussionToReturnDto>(discussion);

            return Ok(discussionToReturn);
        }

        // POST api/discussion
        [HttpPost]
        public async Task<IActionResult> Create(DiscussionToCreateDto discussionToCreateDto)
        {
            // var discussionToCreate = discussion;
            var discussion = _mapper.Map<Discussions>(discussionToCreateDto);
            var discussionCreated = await _repo.Create(discussion);

            //return CreatedAtAction(nameof(Get), new { id = discussionCreated.Id }, discussionCreated);
            return this.Created(new Uri($"{this.Request.GetDisplayUrl()}/{discussionCreated.Id}"), discussionCreated);
        }

        //PUT api/discussion
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, UpdateDiscussionDto discussionToCreateDto)
        {
            var updateDiscussion = _mapper.Map<Discussions>(discussionToCreateDto);

            updateDiscussion.Id = id;

            var discussionUpdated = await _repo.Update(updateDiscussion);

            return this.Created(new Uri($"{this.Request.GetDisplayUrl()}/{discussionUpdated.Id}"), discussionUpdated);
        }

        // DELETE api/discussion
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _repo.Delete(id);

            return Ok();
        }

    }
}