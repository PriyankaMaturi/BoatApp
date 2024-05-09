using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Boat_Backend.Models;
using Boat_Backend.contexts;

namespace Boat_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignupsController : ControllerBase
    {
        private readonly SignupContext _context;

        public SignupsController(SignupContext context)
        {
            _context = context;
        }

        // GET: api/Signups
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Signup>>> Getsignup()
        {
            return await _context.signup.ToListAsync();
        }

        // GET: api/Signups/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Signup>> GetSignup(int id)
        {
            var signup = await _context.signup.FindAsync(id);

            if (signup == null)
            {
                return NotFound();
            }

            return signup;
        }

        // PUT: api/Signups/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSignup(int id, Signup signup)
        {
            if (id != signup.Id)
            {
                return BadRequest();
            }

            _context.Entry(signup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SignupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Signups
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Signup>> PostSignup(Signup signup)
        {
            _context.signup.Add(signup);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSignup", new { id = signup.Id }, signup);
        }

        // DELETE: api/Signups/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSignup(int id)
        {
            var signup = await _context.signup.FindAsync(id);
            if (signup == null)
            {
                return NotFound();
            }

            _context.signup.Remove(signup);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SignupExists(int id)
        {
            return _context.signup.Any(e => e.Id == id);
        }
    }
}
