using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Data.Context;
using EventManagementSystem.Data.Entities;

namespace EventManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly EventsDbContext _context;

        public UserController(EventsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _context.Users.ToList();
            return Ok(users);
        }

        [HttpPost]
        public IActionResult Create([FromBody] User newUser)
        {
            if (newUser == null)
                return BadRequest("משתמש לא תקין");

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAll), new { id = newUser.Id }, newUser);
        }
    }
}
