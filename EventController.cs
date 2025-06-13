using Microsoft.AspNetCore.Mvc;
using EventManagementSystem.Data.Context;
using EventManagementSystem.Data.Entities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace EventManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly EventsDbContext _context;

        public EventController(EventsDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll(int id)
        {
            var ev = _context.Events.FirstOrDefault(e => e.Id == id);

            if (ev == null)
                return NotFound("אירוע לא נמצא");

            return Ok(ev);
        }
        [HttpPost]
        public IActionResult Create([FromBody] Event newEvent)
        {
            if (newEvent == null)
                return BadRequest("אירוע לא תקין");
            newEvent.EventUsers = new List<EventUser>();

            _context.Events.Add(newEvent);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAll), new { id = newEvent.Id }, newEvent);
        }
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Event updatedEvent)
        {
            var existingEvent = _context.Events.FirstOrDefault(e => e.Id == id);

            if (existingEvent == null)
                return NotFound("אירוע לא נמצא");

            existingEvent.Name = updatedEvent.Name;
            existingEvent.StartDate = updatedEvent.StartDate;
            existingEvent.EndDate = updatedEvent.EndDate;
            existingEvent.MaxRegistrations = updatedEvent.MaxRegistrations;
            existingEvent.Location = updatedEvent.Location;

            _context.SaveChanges();

            return Ok(existingEvent);
        }
       



        [HttpGet("{id}/registration")]
        public IActionResult GetEventRegistrations(int id)
        {
            var eventUsers = _context.EventUsers
                .Where(eu => eu.EventRef == id)
                .Select(eu => new
                {
                    eu.UserRef,
                    eu.UserRefNavigation.Name,
                    eu.Creation
                })
                .ToList();

            return Ok(eventUsers);
        }



        [HttpDelete("by_id")]
        public IActionResult Delete(int id)
        {
            var ev = _context.Events
                .Include(e => e.EventUsers)
                .FirstOrDefault(e => e.Id == id);

            if (ev == null)
                return NotFound("אירוע לא נמצא");

            // מוחק קודם את המשתתפים
            _context.EventUsers.RemoveRange(ev.EventUsers);

            _context.Events.Remove(ev);
            _context.SaveChanges();

            return NoContent(); // 204
        }

        [HttpGet("by-location")]
        public IActionResult GetByLocation(string location)
        {
            var events = _context.Events
                .Where(e => e.Location.ToLower() == location.ToLower())
                .ToList();

            return Ok(events);
        }
        [HttpGet("by-dates")]
        public IActionResult GetByDateRange(DateTime from, DateTime to)
        {
            var events = _context.Events
                .Where(e => e.StartDate >= from && e.EndDate <= to)
                .ToList();

            return Ok(events);
        }
        [HttpGet("by-user_id")]
        public IActionResult GetByUser(int userId)
        {
            var events = _context.EventUsers
                .Where(eu => eu.UserRef == userId)
                .Select(eu => eu.EventRef)
                .ToList();

            var result = _context.Events
                .Where(e => events.Contains(e.Id))
                .ToList();

            return Ok(result);
        }
        [HttpGet("weatherbyeventid")]
       
        public async Task<IActionResult> GetEventWeather(int id, [FromServices] IHttpClientFactory clientFactory, [FromServices] IMemoryCache cache, [FromServices] IConfiguration config)
        {
            var ev = _context.Events.FirstOrDefault(e => e.Id == id);
            if (ev == null)
                return NotFound("אירוע לא נמצא");

            var location = ev.Location;
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("לא הוגדר מיקום לאירוע");

            var cacheKey = $"weather_{location.ToLower()}";
            if (cache.TryGetValue(cacheKey, out string cachedWeather))
            {
                return Content(cachedWeather, "application/json");
            }

            var baseUrl = config["WeatherApi:BaseUrl"];
            var apiKey = config["WeatherApi:ApiKey"];

            var client = clientFactory.CreateClient();
            var url = $"{baseUrl}?q={location}&appid={apiKey}&units=metric&lang=he";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "שגיאה בקבלת נתוני מזג אוויר");

            var weatherJson = await response.Content.ReadAsStringAsync();
            cache.Set(cacheKey, weatherJson, TimeSpan.FromMinutes(10));

            return Content(weatherJson, "application/json");
        }







    }
}
