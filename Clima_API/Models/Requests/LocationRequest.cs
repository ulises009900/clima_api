using System.ComponentModel.DataAnnotations;

namespace WeatherApi.Models.Requests;

public record LocationRequest(
    [Range(-90, 90, ErrorMessage = "El campo {0} debe estar en el rango de {1} a {2}.")] double Lat,
    [Range(-180, 180, ErrorMessage = "El campo {0} debe estar en el rango de {1} a {2}.")] double Lon);
