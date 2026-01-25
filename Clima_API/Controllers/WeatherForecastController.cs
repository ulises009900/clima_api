using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using WeatherApi.Models.Requests;
using WeatherApi.Services;

namespace WeatherApi.Controllers;

[ApiController]
[Route("api/v1/weather")]
public class WeatherForecastController : ControllerBase
{
  private readonly WeatherApiService _service;
  private readonly IMemoryCache _cache;
  private static readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions()
      .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

  public WeatherForecastController(WeatherApiService service, IMemoryCache cache)
  {
    _service = service;
    _cache = cache;
  }

  [HttpPost("by-gps")]
  public async Task<IActionResult> ByGps([FromBody] LocationRequest request)
  {
    var lat = Math.Round(request.Lat, 4);
    var lon = Math.Round(request.Lon, 4);
    var cacheKey = $"current_{lat.ToString(CultureInfo.InvariantCulture)}_{lon.ToString(CultureInfo.InvariantCulture)}";

    var weather = await _cache.GetOrCreateAsync(cacheKey, async entry =>
    {
      entry.SetOptions(_cacheOptions);
      return await _service.GetCurrentAsync(lat, lon);
    });

    if (weather is null)
      return StatusCode(502, "Weather provider error");

    return Ok(weather);
  }

  [HttpGet("echo-location")]
  public IActionResult EchoLocation([FromQuery] LocationRequest request)
  {
    return Ok(request);
  }

  [HttpPost("forecast/next-24h")]
  public async Task<IActionResult> Next24Hours([FromBody] LocationRequest request)
  {
    var lat = Math.Round(request.Lat, 4);
    var lon = Math.Round(request.Lon, 4);
    var cacheKey = $"next24h_{lat.ToString(CultureInfo.InvariantCulture)}_{lon.ToString(CultureInfo.InvariantCulture)}";

    var hours = await _cache.GetOrCreateAsync(cacheKey, async entry =>
    {
      entry.SetOptions(_cacheOptions);
      return await _service.GetNext24HoursAsync(lat, lon);
    });

    return Ok(hours);
  }

  [HttpPost("forecast/next-3d")]
  public async Task<IActionResult> Next3Days([FromBody] LocationRequest request)
  {
    var lat = Math.Round(request.Lat, 4);
    var lon = Math.Round(request.Lon, 4);
    var cacheKey = $"next3d_{lat.ToString(CultureInfo.InvariantCulture)}_{lon.ToString(CultureInfo.InvariantCulture)}";

    var days = await _cache.GetOrCreateAsync(cacheKey, async entry =>
    {
      entry.SetOptions(_cacheOptions);
      return await _service.GetNext3DaysAsync(lat, lon);
    });

    return Ok(days);
  }
}
