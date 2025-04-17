using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InvestmentPortfolioAPI.Data;
using InvestmentPortfolioAPI.Models;
using InvestmentPortfolioAPI.Services;

namespace InvestmentPortfolioAPI.Controllers
{
    [Authorize] //Protects all endpoints in this controller
    [ApiController]
    [Route("api/[controller]")]
   public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        
        // POST: api/users
            [HttpPost]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> AddUser([FromBody] User user)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _context.Users.Add(user);
                user.PasswordHash=BCrypt.Net.BCrypt.HashPassword(user.PasswordHash.ToString());
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }

            // GET: api/users/5
            [HttpGet("{id}")]
            [Authorize(Roles = "Admin")]
            public async Task<ActionResult<User>> GetUserById(int id)
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound();

                return user;
            }

            [HttpPut("{id}")]
            [Authorize(Roles = "Admin")]
            public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
            {
                var existingUser = await _context.Users.FindAsync(id);
                if (existingUser == null)
                    return NotFound();

                existingUser.Username = updatedUser.Username;

                await _context.SaveChangesAsync();

                return NoContent();
            }

            //Delete all
            [HttpDelete("{id}")]
            [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }
    }
}