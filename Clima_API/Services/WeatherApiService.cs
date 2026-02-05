using System.Net.Http.Json;
using System.Text.Json;
using System.Globalization;
using WeatherApi.Models.Responses;

namespace WeatherApi.Services;

public class WeatherApiService
{
  private readonly HttpClient _http;
  private readonly ILogger<WeatherApiService> _logger;
  private readonly string _baseUrl;
  private readonly string _apiKey;

  /// <summary>
  /// Inicializa una nueva instancia de la clase <see cref="WeatherApiService"/>.
  /// </summary>
  /// <param name="http">El HttpClient a utilizar para realizar las solicitudes.</param>
  /// <param name="config">La configuración para obtener la clave de API y la URL base.</param>
  /// <param name="logger">El logger.</param>
  /// <exception cref="InvalidOperationException">Se lanza si WeatherApi:BaseUrl o WeatherApi:ApiKey no están configurados.</exception>
  public WeatherApiService(HttpClient http, IConfiguration config, ILogger<WeatherApiService> logger)
  {
    _http = http;
    _logger = logger;
    _baseUrl = config["WeatherApi:BaseUrl"] ?? throw new InvalidOperationException("WeatherApi:BaseUrl is not configured.");
    _apiKey = config["WeatherApi:ApiKey"] ?? throw new InvalidOperationException("WeatherApi:ApiKey is not configured.");
  }

  /// <summary>
  /// Obtiene el clima actual para una latitud y longitud específicas.
  /// </summary>
  /// <param name="lat">La latitud.</param>
  /// <param name="lon">La longitud.</param>
  /// <returns>La respuesta del clima actual o nulo si no se encuentra.</returns>
  public async Task<WeatherResponse?> GetCurrentAsync(double lat, double lon)
  {
    var url = FormattableString.Invariant($"{_baseUrl}current.json?key={_apiKey}&q={lat},{lon}&lang=es");

    try
    {
      var response = await _http.GetFromJsonAsync<WeatherResponse>(url);
      if (response is null)
      {
        throw new InvalidOperationException("The weather API returned an empty response.");
      }
      return response;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while calling the weather API for current weather at {Lat},{Lon}", lat, lon);
      throw;
    }
  }
  /// <summary>
  /// Obtiene el pronóstico para las próximas 24 horas para una latitud y longitud específicas.
  /// </summary>
  /// <param name="lat">La latitud.</param>
  /// <param name="lon">La longitud.</param>
  /// <returns>Una lista de pronósticos por hora para las próximas 24 horas.</returns>
  public async Task<List<HourForecast>> GetNext24HoursAsync(double lat, double lon)
  {
    var url = FormattableString.Invariant($"{_baseUrl}forecast.json?key={_apiKey}&q={lat},{lon}&days=2&aqi=no&alerts=no&lang=es");

    try
    {
      var result = await _http.GetFromJsonAsync<ForecastResponse>(url);

      if (result == null)
        throw new InvalidOperationException("The weather API returned an empty forecast response.");

      // Usar InvariantCulture para evitar errores si el servidor tiene una configuración regional diferente
      var now = DateTime.Parse(result.Location.Localtime, CultureInfo.InvariantCulture);

      return result.Forecast.Forecastday
          .SelectMany(d => d.Hour)
          .Where(h =>
          {
            var time = DateTime.Parse(h.Time, CultureInfo.InvariantCulture);
            return time >= now && time <= now.AddHours(24);
          })
          .ToList();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting next 24h forecast for {Lat},{Lon}", lat, lon);
      throw;
    }
  }
  /// <summary>
  /// Obtiene el pronóstico para los próximos 3 días para una latitud y longitud específicas.
  /// </summary>
  /// <param name="lat">La latitud.</param>
  /// <param name="lon">La longitud.</param>
  /// <returns>Una lista de pronósticos diarios para los próximos 3 días.</returns>
  public async Task<List<ForecastDay>> GetNext3DaysAsync(double lat, double lon)
  {
    var url = FormattableString.Invariant($"{_baseUrl}forecast.json?key={_apiKey}&q={lat},{lon}&days=3&aqi=no&alerts=no&lang=es");

    try
    {
      var result = await _http.GetFromJsonAsync<ForecastResponse>(url);

      if (result == null)
        throw new InvalidOperationException("The weather API returned an empty forecast response.");

      return result.Forecast.Forecastday;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting next 3 days forecast for {Lat},{Lon}", lat, lon);
      throw;
    }
  }

  /// <summary>
  /// Obtiene el pronóstico por hora para un día específico.
  /// </summary>
  /// <param name="lat">La latitud.</param>
  /// <param name="lon">La longitud.</param>
  /// <param name="date">La fecha en formato yyyy-MM-dd.</param>
  /// <returns>Una lista de pronósticos por hora para el día especificado.</returns>
  public async Task<List<HourForecast>> GetForecastByDayAsync(double lat, double lon, string date)
  {
    var url = FormattableString.Invariant($"{_baseUrl}forecast.json?key={_apiKey}&q={lat},{lon}&dt={date}&aqi=no&alerts=no&lang=es");

    try
    {
      var result = await _http.GetFromJsonAsync<ForecastResponse>(url);

      if (result == null)
        throw new InvalidOperationException("The weather API returned an empty forecast response.");

      var dayForecast = result.Forecast.Forecastday.FirstOrDefault();

      if (dayForecast == null)
      {
        _logger.LogWarning("No se encontró pronóstico para la fecha {Date}. Nota: El endpoint 'forecast' solo soporta fechas futuras (hasta 14 días).", date);
        return [];
      }

      return dayForecast.Hour;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting forecast for date {Date} at {Lat},{Lon}", date, lat, lon);
      throw;
    }
  }
}
