using System.Text.Json.Serialization;

namespace WeatherApi.Models.Responses;

public record WeatherResponse(
    [property: JsonPropertyName("location")] Location Location,
    [property: JsonPropertyName("current")] Current Current
);

public record Location(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("region")] string Region,
    [property: JsonPropertyName("country")] string Country,
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lon")] double Lon,
    [property: JsonPropertyName("localtime")] string Localtime
);

public record Current(
    [property: JsonPropertyName("last_updated")] string LastUpdated,
    [property: JsonPropertyName("temp_c")] double TempC,
    [property: JsonPropertyName("temp_f")] double TempF,
    [property: JsonPropertyName("is_day")] int IsDay,
    [property: JsonPropertyName("condition")] Condition Condition,
    [property: JsonPropertyName("wind_kph")] double WindKph,
    [property: JsonPropertyName("humidity")] int Humidity,
    [property: JsonPropertyName("feelslike_c")] double FeelsLikeC,
    [property: JsonPropertyName("uv")] double Uv
);

public record Condition(
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("icon")] string Icon,
    [property: JsonPropertyName("code")] int Code
);