namespace WeatherApi.Models.Responses;

public class WeatherResponse
{
  public Location Location { get; set; } = default!;
  public Current Current { get; set; } = default!;
}

public class Location
{
  public string Name { get; set; } = "";
  public string Country { get; set; } = "";
}

public class Current
{
  public double Temp_C { get; set; }
  public double Wind_Kph { get; set; }
  public int Humidity { get; set; }
  public double Precip_Mm { get; set; }
  public Condition Condition { get; set; } = default!;
}

public class Condition
{
  public string Text { get; set; } = "";
}
