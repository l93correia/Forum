using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Forum.API.Data;
using Forum.API.Dtos;
using Forum.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscussionController : ControllerBase
    {
        private readonly IResponseRepository _repo;
        private readonly IMapper _mapper;

        public DiscussionController(IResponseRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        // GET api/discussion
        [HttpGet]
        public async Task<IActionResult> GetDiscussions()
        {
            var discussions = await _repo.GetDiscussions();

            var discussionsToReturn = _mapper.Map<IEnumerable<DiscussionForListDto>>(discussions);

            return Ok(discussionsToReturn);
        }

        // GET api/discussion
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscussion(int id)
        {
            var discussion = await _repo.GetDiscussion(id);

            //var responses = await _repo.GetResponses(discussion.Id);

            if (discussion == null)
            {
                return BadRequest("Not found");
            }

            var discussionToReturn = _mapper.Map<DiscussionToReturnDto>(discussion);

            return Ok(discussionToReturn);
        }

        // POST api/discussion
        [HttpPost("PostDiscussion")]
        public async Task<IActionResult> PostDiscussion(DiscussionToCreateDto discussionToCreateDto)
        {
            // var discussionToCreate = discussion;
            var discussionCreated = await _repo.CreateDiscussion(discussionToCreateDto.Subject, 
                    discussionToCreateDto.Comment, discussionToCreateDto.UserId);

            return CreatedAtAction(nameof(GetDiscussion), new { id = discussionCreated.Id }, discussionCreated);
        }

        // POST api/discussion
        [HttpPost("PostResponse")]
        public async Task<IActionResult> PostResponse(ResponseToCreateDto responseToCreateDto)
        {
            var responseCreated = await _repo.AddResponse(responseToCreateDto.Response, 
                    responseToCreateDto.UserId, responseToCreateDto.DiscussionId);

            return StatusCode(201);
        }
    }
}