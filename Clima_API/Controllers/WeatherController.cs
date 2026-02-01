using Microsoft.AspNetCore.Mvc;
using WeatherApi.Models.Requests;
using WeatherApi.Services;
using System.Globalization;

namespace WeatherApi.Controllers;

/// <summary>
/// Controlador para gestionar las peticiones del clima.
/// </summary>
[ApiController]
[Route("api/v1/weather")]
public class WeatherController : ControllerBase
{
  private readonly WeatherApiService _weatherService;

  public WeatherController(WeatherApiService weatherService)
  {
    _weatherService = weatherService;
  }

  /// <summary>
  /// Obtiene el clima actual basado en coordenadas GPS.
  /// </summary>
  /// <param name="request">Objeto con latitud y longitud.</param>
  /// <returns>Datos del clima actual.</returns>
  [HttpPost("by-gps")]
  public async Task<IActionResult> GetCurrentWeather([FromBody] LocationRequest request)
  {
    var result = await _weatherService.GetCurrentAsync(request.Lat, request.Lon);
    return result != null ? Ok(result) : NotFound();
  }

  /// <summary>
  /// Obtiene el pronóstico para las próximas 24 horas.
  /// </summary>
  /// <param name="request">Objeto con latitud y longitud.</param>
  /// <returns>Lista de pronósticos por hora.</returns>
  [HttpPost("forecast/next-24h")]
  public async Task<IActionResult> GetNext24Hours([FromBody] LocationRequest request)
  {
    var result = await _weatherService.GetNext24HoursAsync(request.Lat, request.Lon);
    return Ok(result);
  }

  /// <summary>
  /// Obtiene el pronóstico para los próximos 3 días.
  /// </summary>
  /// <param name="request">Objeto con latitud y longitud.</param>
  /// <returns>Lista de pronósticos diarios.</returns>
  [HttpPost("forecast/next-3d")]
  public async Task<IActionResult> GetNext3Days([FromBody] LocationRequest request)
  {
    var result = await _weatherService.GetNext3DaysAsync(request.Lat, request.Lon);
    return Ok(result);
  }

  /// <summary>
  /// Obtiene el pronóstico por hora para una fecha específica.
  /// </summary>
  /// <param name="wrapper">Envoltorio que contiene la solicitud.</param>
  /// <returns>Lista de pronósticos por hora para el día especificado.</returns>
  [HttpPost("forecast/by-date")]
  public async Task<IActionResult> GetForecastByDate([FromBody] DateLocationRequestWrapper wrapper)
  {
    // Convertir DateOnly a string formato yyyy-MM-dd para la API
    var dateString = wrapper.Request.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    var result = await _weatherService.GetForecastByDayAsync(wrapper.Request.Lat, wrapper.Request.Lon, dateString);
    return Ok(result);
  }
}