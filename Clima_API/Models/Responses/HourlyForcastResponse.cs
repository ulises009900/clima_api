using System.Text.Json.Serialization;

namespace WeatherApi.Models.Responses;

public class ForecastResponse
{
  [JsonPropertyName("forecast")]
  public Forecast Forecast { get; set; } = default!;

  [JsonPropertyName("location")]
  public Location Location { get; set; } = default!;
}

public class Forecast
{
  [JsonPropertyName("forecastday")]
  public List<ForecastDay> Forecastday { get; set; } = [];
}

public class ForecastDay
{
  [JsonPropertyName("date")]
  public string Date { get; set; } = "";

  [JsonPropertyName("day")]
  public Day Day { get; set; } = default!;

  [JsonPropertyName("hour")]
  public List<HourForecast> Hour { get; set; } = [];
}

public class HourForecast
{
  [JsonPropertyName("time")]
  public string Time { get; set; } = "";

  [JsonPropertyName("temp_c")]
  public double TempC { get; set; }

  [JsonPropertyName("precip_mm")]
  public double PrecipMm { get; set; }

  [JsonPropertyName("chance_of_rain")]
  public int ChanceOfRain { get; set; }

  [JsonPropertyName("humidity")]
  public int Humidity { get; set; }

  [JsonPropertyName("condition")]
  public Condition Condition { get; set; } = default!;
}

public class Day
{
  [JsonPropertyName("maxtemp_c")]
  public double MaxTempC { get; set; }

  [JsonPropertyName("mintemp_c")]
  public double MinTempC { get; set; }

  [JsonPropertyName("condition")]
  public Condition Condition { get; set; } = default!;
}
