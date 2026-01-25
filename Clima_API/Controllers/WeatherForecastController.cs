using Microsoft.AspNetCore.Mvc;
using WeatherApi.Models.Requests;
using WeatherApi.Services;

namespace WeatherApi.Controllers;

[ApiController]
[Route("api/v1/weather")]
public class WeatherForecastController : ControllerBase
{
  private readonly WeatherApiService _service;

  public WeatherForecastController(WeatherApiService service)
  {
    _service = service;
  }

  [HttpPost("by-gps")]
  public async Task<IActionResult> ByGps([FromBody] LocationRequest request)
  {
    var weather = await _service.GetCurrentAsync(request.Lat, request.Lon);

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
    var hours = await _service.GetNext24HoursAsync(request.Lat, request.Lon);

    return Ok(hours);
  }

  [HttpPost("forecast/next-3d")]
  public async Task<IActionResult> Next3Days([FromBody] LocationRequest request)
  {
    var days = await _service.GetNext3DaysAsync(request.Lat, request.Lon);

    return Ok(days);
  }
}
