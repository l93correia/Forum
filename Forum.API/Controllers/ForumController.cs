using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Forum.API.Data;
using Forum.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Forum.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumController : ControllerBase
    {
        private readonly IForumRepository _repo;

        public ForumController(IForumRepository repo)
        {
            _repo = repo;
        }

        // GET api/forum
        [HttpGet]
        public async Task<IActionResult> GetDiscussions()
        {
            var discussions = await _repo.GetDiscussions();

            return Ok(discussions);
        }

        // GET api/forum
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscussion(int id)
        {
            var discussion = await _repo.GetDiscussion(id);

            //var responses = await _repo.GetResponses(discussion.Id);

            if (discussion == null)
            {
                return BadRequest("Not found");
            }

            return Ok(discussion);
        }

        // POST api/forum
        [HttpPost("PostDiscussion")]
        public async Task<ActionResult> PostDiscussion(Discussions discussion)
        {
            var discussionToCreate = discussion;
            var discussionCreated = await _repo.CreateDiscussion(discussion);

            return CreatedAtAction(nameof(GetDiscussion), new { id = discussionCreated.Id }, discussionCreated);
        }

        // POST api/forum
        [HttpPost("PostResponse")]
        public async Task<ActionResult> PostResponse(DiscussionResponses response)
        {
            var responseToCreate = response;
            var responseCreated = await _repo.AddResponse(response);

            return StatusCode(201);
        }
    }
}