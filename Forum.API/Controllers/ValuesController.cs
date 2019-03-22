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
    public class ValuesController : ControllerBase
    {
        private readonly DataContext _context;

        public ValuesController(DataContext context)
        {
            _context = context;
        }
        // GET api/values
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            var users = await _context.User.ToListAsync();

            return Ok(users);
        }

        // GET api/values
        [HttpGet("{id}")]
        public async Task<IActionResult> GetValue(long id)
        {
            var user = await _context.User.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return BadRequest("Not found");
            }

            return Ok(user);
        }

        // POST api/values
        [HttpPost]
        public async Task<ActionResult<User>> PostValue(User user)
        {
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetValue), new { id = user.Id }, user);
        }

        // PUT api/values
        [HttpPut("{id}")]
        public void Put(long id, [FromBody] string value)
        {
        }

        // DELETE api/values
        [HttpDelete("{id}")]
        public void Delete(long id)
        {
        }
    }
}
