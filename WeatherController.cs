using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;


namespace EventManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public WeatherController(IHttpClientFactory httpClientFactory, IMemoryCache cache, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("יש לספק מיקום");

            string cacheKey = $"weather_{location.ToLower()}";

            if (_cache.TryGetValue(cacheKey, out string cachedWeather))
            {
                return Content(cachedWeather, "application/json");
            }

            var client = _httpClientFactory.CreateClient();

            var baseUrl = _configuration["WeatherApi:BaseUrl"];
            var apiKey = _configuration["WeatherApi:ApiKey"];
            var url = $"{baseUrl}?q={location}&appid={apiKey}&units=metric&lang=he";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, "שגיאה בקבלת נתוני מזג אוויר");

            var weatherJson = await response.Content.ReadAsStringAsync();

            _cache.Set(cacheKey, weatherJson, TimeSpan.FromMinutes(10));

            return Content(weatherJson, "application/json");
        }
    }
}
