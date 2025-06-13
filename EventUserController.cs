using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Data.Context;
using EventManagementSystem.Data.Entities;

namespace EventManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventUserController : ControllerBase
    {
        private readonly EventsDbContext _context;

        public EventUserController(EventsDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Register(int userId, int eventId)
        {
            var ev = _context.Events.FirstOrDefault(e => e.Id == eventId);
            if (ev == null)
                return NotFound("אירוע לא קיים");

            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
                return NotFound("משתמש לא קיים");

            var currentCount = _context.EventUsers.Count(eu => eu.EventRef == eventId);
            if (currentCount >= ev.MaxRegistrations)
                return BadRequest("ההרשמה מלאה");

            var alreadyRegistered = _context.EventUsers.Any(eu => eu.EventRef == eventId && eu.UserRef == userId);
            if (alreadyRegistered)
                return BadRequest("משתמש כבר רשום לאירוע");

            var newRegistration = new EventUser
            {
                EventRef = eventId,
                UserRef = userId,
                Creation = DateTime.Now
            };

            _context.EventUsers.Add(newRegistration);
            _context.SaveChanges();

            return Ok("ההרשמה בוצעה בהצלחה");
        }
    }
}
