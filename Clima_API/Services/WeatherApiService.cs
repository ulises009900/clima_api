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
      return await _http.GetFromJsonAsync<WeatherResponse>(url);
    }
    catch (HttpRequestException ex)
    {
      _logger.LogError(ex, "An error occurred while calling the weather API. Status code: {StatusCode}", ex.StatusCode);
      return null;
    }
    catch (JsonException ex)
    {
      _logger.LogError(ex, "Failed to deserialize weather API response from {Url}", url);
      return null;
    }
  }
  public async Task<List<HourForecast>> GetNext24HoursAsync(double lat, double lon)
  {
    var url = FormattableString.Invariant($"{_baseUrl}forecast.json?key={_apiKey}&q={lat},{lon}&days=2&aqi=no&alerts=no");

    var result = await _http.GetFromJsonAsync<ForecastResponse>(url);

    if (result == null)
      return [];

    var now = DateTime.Parse(result.Location.Localtime);

    return result.Forecast.Forecastday
        .SelectMany(d => d.Hour)
        .Where(h =>
        {
          var hourTime = DateTime.Parse(h.Time);
          return hourTime >= now && hourTime <= now.AddHours(24);
        })
        .ToList();
  }
  public async Task<List<ForecastDay>> GetNext3DaysAsync(double lat, double lon)
  {
    var url = FormattableString.Invariant($"{_baseUrl}forecast.json?key={_apiKey}&q={lat},{lon}&days=3&aqi=no&alerts=no");

    var result = await _http.GetFromJsonAsync<ForecastResponse>(url);

    if (result == null)
      return [];

    return result.Forecast.Forecastday;
  }

}
