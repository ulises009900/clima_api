using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherApi.Models.Requests;
using WeatherApi.Services;

namespace Clima_API.Controllers
{
    [ApiController]
    [Route("api/v1/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherApiService _weatherService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _openWeatherApiKey;

        public WeatherController(WeatherApiService weatherService, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _weatherService = weatherService;
            _httpClientFactory = httpClientFactory;
            _openWeatherApiKey = config["OpenWeather:ApiKey"];
        }

        [HttpGet("echo-location")]
        public IActionResult EchoLocation([FromQuery] double lat, [FromQuery] double lon)
        {
            return Ok(new { lat, lon });
        }

        [HttpPost("by-gps")]
        public async Task<IActionResult> GetCurrentWeather([FromBody] LocationRequest request)
        {
            var result = await _weatherService.GetCurrentAsync(request.Lat, request.Lon);
            return result is not null ? Ok(result) : NotFound();
        }

        [HttpPost("forecast/next-24h")]
        public async Task<IActionResult> GetNext24Hours([FromBody] LocationRequest request)
        {
            var result = await _weatherService.GetNext24HoursAsync(request.Lat, request.Lon);
            return Ok(result);
        }

        [HttpPost("forecast/next-3d")]
        public async Task<IActionResult> GetNext3Days([FromBody] LocationRequest request)
        {
            var result = await _weatherService.GetNext3DaysAsync(request.Lat, request.Lon);
            return Ok(result);
        }

        [HttpPost("forecast/by-date")]
        public async Task<IActionResult> GetForecastByDate([FromBody] DateLocationRequestWrapper wrapper)
        {
            var result = await _weatherService.GetForecastByDayAsync(wrapper.Request.Lat, wrapper.Request.Lon, wrapper.Request.Date.ToString("yyyy-MM-dd"));
            return Ok(result);
        }

    }
}