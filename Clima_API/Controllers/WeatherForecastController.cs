using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;
using Polly.CircuitBreaker;
using WeatherApi.Models.Requests;
using WeatherApi.Services;

namespace WeatherApi.Controllers;

[ApiController]
[Route("api/v1/weather")]
public class WeatherForecastController : ControllerBase
{
  private readonly WeatherApiService _service;
  private readonly IMemoryCache _cache;
  private readonly ILogger<WeatherForecastController> _logger;
  private static readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions()
      .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));

  public WeatherForecastController(WeatherApiService service, IMemoryCache cache, ILogger<WeatherForecastController> logger)
  {
    _service = service;
    _cache = cache;
    _logger = logger;
  }

  /// <summary>
  /// Obtiene el clima actual para una ubicación dada por GPS.
  /// </summary>
  /// <param name="request">La latitud y longitud de la ubicación.</param>
  /// <returns>Las condiciones climáticas actuales.</returns>
  /// <response code="200">Retorna el clima actual.</response>
  /// <response code="502">Si el proveedor de clima externo tiene un error.</response>
  /// <response code="503">Si el servicio de clima no está disponible temporalmente.</response>
  [HttpPost("by-gps")]
  public async Task<IActionResult> ByGps([FromBody] LocationRequest request)
  {
    var lat = Math.Round(request.Lat, 4);
    var lon = Math.Round(request.Lon, 4);
    var cacheKey = $"current_{lat.ToString(CultureInfo.InvariantCulture)}_{lon.ToString(CultureInfo.InvariantCulture)}";

    try
    {
      var weather = await _service.GetCurrentAsync(lat, lon);
      _cache.Set(cacheKey, weather, _cacheOptions);
      return Ok(weather);
    }
    catch (Exception ex)
    {
      var logLevel = ex is BrokenCircuitException ? LogLevel.Warning : LogLevel.Error;
      _logger.Log(logLevel, ex, "Failed to get current weather for {Lat},{Lon}. Attempting to use fallback cache.", lat, lon);

      if (_cache.TryGetValue(cacheKey, out var weather))
      {
        Response.Headers.Append("Warning", "110 Stale Response");
        return Ok(weather);
      }

      if (ex is BrokenCircuitException)
        return StatusCode(503, "Weather service is temporarily unavailable. Please try again later.");

      return StatusCode(502, "Weather provider error. Please try again later.");
    }
  }

  /// <summary>
  /// Devuelve la ubicación proporcionada. Útil para pruebas.
  /// </summary>
  /// <param name="request">La latitud y longitud de la ubicación.</param>
  /// <returns>La ubicación que fue proporcionada.</returns>
  [HttpGet("echo-location")]
  public IActionResult EchoLocation([FromQuery] LocationRequest request)
  {
    return Ok(request);
  }

  /// <summary>
  /// Obtiene el pronóstico del tiempo para las próximas 24 horas.
  /// </summary>
  /// <param name="request">La latitud y longitud de la ubicación.</param>
  /// <returns>Una lista de pronósticos por hora para las próximas 24 horas.</returns>
  /// <response code="200">Retorna el pronóstico de 24 horas.</response>
  [HttpPost("forecast/next-24h")]
  public async Task<IActionResult> Next24Hours([FromBody] LocationRequest request)
  {
    var lat = Math.Round(request.Lat, 4);
    var lon = Math.Round(request.Lon, 4);
    var cacheKey = $"next24h_{lat.ToString(CultureInfo.InvariantCulture)}_{lon.ToString(CultureInfo.InvariantCulture)}";
    try
    {
      var hours = await _service.GetNext24HoursAsync(lat, lon);
      _cache.Set(cacheKey, hours, _cacheOptions);
      return Ok(hours);
    }
    catch (Exception ex)
    {
      var logLevel = ex is BrokenCircuitException ? LogLevel.Warning : LogLevel.Error;
      _logger.Log(logLevel, ex, "Failed to get 24h forecast for {Lat},{Lon}. Attempting to use fallback cache.", lat, lon);

      if (_cache.TryGetValue(cacheKey, out var hours))
      {
        Response.Headers.Append("Warning", "110 Stale Response");
        return Ok(hours);
      }

      if (ex is BrokenCircuitException)
        return StatusCode(503, "Weather service is temporarily unavailable. Please try again later.");

      return StatusCode(502, "Weather provider error. Please try again later.");
    }
  }

  /// <summary>
  /// Obtiene el pronóstico del tiempo para los próximos 3 días.
  /// </summary>
  /// <param name="request">La latitud y longitud de la ubicación.</param>
  /// <returns>Una lista de pronósticos diarios para los próximos 3 días.</returns>
  /// <response code="200">Retorna el pronóstico de 3 días.</response>
  [HttpPost("forecast/next-3d")]
  public async Task<IActionResult> Next3Days([FromBody] LocationRequest request)
  {
    var lat = Math.Round(request.Lat, 4);
    var lon = Math.Round(request.Lon, 4);
    var cacheKey = $"next3d_{lat.ToString(CultureInfo.InvariantCulture)}_{lon.ToString(CultureInfo.InvariantCulture)}";
    try
    {
      var days = await _service.GetNext3DaysAsync(lat, lon);
      _cache.Set(cacheKey, days, _cacheOptions);
      return Ok(days);
    }
    catch (Exception ex)
    {
      var logLevel = ex is BrokenCircuitException ? LogLevel.Warning : LogLevel.Error;
      _logger.Log(logLevel, ex, "Failed to get 3-day forecast for {Lat},{Lon}. Attempting to use fallback cache.", lat, lon);

      if (_cache.TryGetValue(cacheKey, out var days))
      {
        Response.Headers.Append("Warning", "110 Stale Response");
        return Ok(days);
      }

      if (ex is BrokenCircuitException)
        return StatusCode(503, "Weather service is temporarily unavailable. Please try again later.");

      return StatusCode(502, "Weather provider error. Please try again later.");
    }
  }
}
