using System.ComponentModel.DataAnnotations;

namespace WeatherApi.Models.Requests;

public record LocationRequest(
    [Range(-90, 90)] double Lat,
    [Range(-180, 180)] double Lon);
