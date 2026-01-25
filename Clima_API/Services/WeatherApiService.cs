using System.Net.Http.Json;
using System.Text.Json;
using WeatherApi.Models.Responses;

namespace WeatherApi.Services;

public class WeatherApiService
{
  private readonly HttpClient _http;
  private readonly ILogger<WeatherApiService> _logger;
  private readonly string _baseUrl;
  private readonly string _apiKey;

  public WeatherApiService(HttpClient http, IConfiguration config, ILogger<WeatherApiService> logger)
  {
    _http = http;
    _logger = logger;
    _baseUrl = config["WeatherApi:BaseUrl"] ?? throw new InvalidOperationException("WeatherApi:BaseUrl is not configured.");
    _apiKey = config["WeatherApi:ApiKey"] ?? throw new InvalidOperationException("WeatherApi:ApiKey is not configured.");
  }

  public async Task<WeatherResponse?> GetCurrentAsync(double lat, double lon)
  {
    var url = FormattableString.Invariant($"{_baseUrl}current.json?key={_apiKey}&q={lat},{lon}");

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
  public async Task<List<HourForecast>> GetNext24HoursAsync(double lat, double lon)
  {
    var url = FormattableString.Invariant($"{_baseUrl}forecast.json?key={_apiKey}&q={lat},{lon}&days=2&aqi=no&alerts=no");

    try
    {
      var result = await _http.GetFromJsonAsync<ForecastResponse>(url);

      if (result == null)
        throw new InvalidOperationException("The weather API returned an empty forecast response.");

      var now = DateTime.Parse(result.Location.Localtime);

      return result.Forecast.Forecastday
          .SelectMany(d => d.Hour)
          .Where(h => DateTime.Parse(h.Time) >= now && DateTime.Parse(h.Time) <= now.AddHours(24))
          .ToList();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting next 24h forecast for {Lat},{Lon}", lat, lon);
      throw;
    }
  }
  public async Task<List<ForecastDay>> GetNext3DaysAsync(double lat, double lon)
  {
    var url = FormattableString.Invariant($"{_baseUrl}forecast.json?key={_apiKey}&q={lat},{lon}&days=3&aqi=no&alerts=no");

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

}
