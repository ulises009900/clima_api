using System.Text.Json.Serialization;

namespace WeatherApi.Models.Responses;

/// <summary>
/// Representa la respuesta del pronóstico desde la API del clima.
/// </summary>
public class ForecastResponse
{
  /// <summary>
  /// Obtiene o establece los datos del pronóstico.
  /// </summary>
  [JsonPropertyName("forecast")]
  public Forecast Forecast { get; set; } = default!;

  /// <summary>
  /// Obtiene o establece los datos de la ubicación.
  /// </summary>
  [JsonPropertyName("location")]
  public Location Location { get; set; } = default!;
}

/// <summary>
/// Representa el contenedor del pronóstico.
/// </summary>
public class Forecast
{
  /// <summary>
  /// Obtiene o establece la lista de días de pronóstico.
  /// </summary>
  [JsonPropertyName("forecastday")]
  public List<ForecastDay> Forecastday { get; set; } = [];
}

/// <summary>
/// Representa el pronóstico para un solo día.
/// </summary>
public class ForecastDay
{
  /// <summary>
  /// Obtiene o establece la fecha del pronóstico.
  /// </summary>
  [JsonPropertyName("date")]
  public string Date { get; set; } = "";

  /// <summary>
  /// Obtiene o establece los datos del pronóstico diario.
  /// </summary>
  [JsonPropertyName("day")]
  public Day Day { get; set; } = default!;

  /// <summary>
  /// Obtiene o establece la lista de pronósticos por hora para el día.
  /// </summary>
  [JsonPropertyName("hour")]
  public List<HourForecast> Hour { get; set; } = [];
}

/// <summary>
/// Representa el pronóstico para una sola hora.
/// </summary>
public class HourForecast
{
  /// <summary>
  /// Obtiene o establece la hora del pronóstico.
  /// </summary>
  [JsonPropertyName("time")]
  public string Time { get; set; } = "";

  /// <summary>
  /// Obtiene o establece la temperatura en Celsius.
  /// </summary>
  [JsonPropertyName("temp_c")]
  public double TempC { get; set; }

  /// <summary>
  /// Obtiene o establece la precipitación en milímetros.
  /// </summary>
  [JsonPropertyName("precip_mm")]
  public double PrecipMm { get; set; }

  /// <summary>
  /// Obtiene o establece la probabilidad de lluvia como un porcentaje.
  /// </summary>
  [JsonPropertyName("chance_of_rain")]
  public int ChanceOfRain { get; set; }

  /// <summary>
  /// Obtiene o establece la humedad como un porcentaje.
  /// </summary>
  [JsonPropertyName("humidity")]
  public int Humidity { get; set; }

  /// <summary>
  /// Obtiene o establece la condición climática.
  /// </summary>
  [JsonPropertyName("condition")]
  public Condition Condition { get; set; } = default!;
}

/// <summary>
/// Representa el resumen del pronóstico para un día.
/// </summary>
public class Day
{
  /// <summary>
  /// Obtiene o establece la temperatura máxima en Celsius.
  /// </summary>
  [JsonPropertyName("maxtemp_c")]
  public double MaxTempC { get; set; }

  /// <summary>
  /// Obtiene o establece la temperatura mínima en Celsius.
  /// </summary>
  [JsonPropertyName("mintemp_c")]
  public double MinTempC { get; set; }

  /// <summary>
  /// Obtiene o establece la condición climática para el día.
  /// </summary>
  [JsonPropertyName("condition")]
  public Condition Condition { get; set; } = default!;
}
