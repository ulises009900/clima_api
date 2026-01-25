using Microsoft.AspNetCore.Mvc;
using WeatherApi.Models.Requests;
using WeatherApi.Services;

namespace WeatherApi.Controllers;

[ApiController]
[Route("api/v1/weather")]
public class WeatherController : ControllerBase
{
  private readonly WeatherApiService _service;

  public WeatherController(WeatherApiService service)
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
}
