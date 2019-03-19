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
        private readonly DataContext _context;

        public ForumController(DataContext context)
        {
            _context = context;
        }

        // GET api/forum
        [HttpGet]
        public async Task<IActionResult> GetDiscussions()
        {
            var discussions = await _context.Discussions.ToListAsync();

            return Ok(discussions);
        }

        // GET api/forum
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDiscussion(int id)
        {
            var discussion = await _context.Discussions.FirstOrDefaultAsync(x => x.Id == id);

            if (discussion == null)
            {
                return BadRequest("Not found");
            }

            return Ok(discussion);
        }

        // POST api/forum
        [HttpPost]
        public async Task<ActionResult<Discussions>> PostUser(Discussions discussion)
        {
            _context.Discussions.Add(discussion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDiscussion), new { id = discussion.Id }, discussion);
        }
    }
}