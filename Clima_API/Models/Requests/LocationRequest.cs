using System.ComponentModel.DataAnnotations;

namespace WeatherApi.Models.Requests;

/// <summary>
/// Representa una ubicación geográfica con latitud y longitud.
/// </summary>
/// <param name="Lat">Latitud. Debe estar entre -90 y 90.</param>
/// <param name="Lon">Longitud. Debe estar entre -180 y 180.</param>
public record LocationRequest(
    [Range(-90, 90, ErrorMessage = "El campo {0} debe estar en el rango de {1} a {2}.")] double Lat,
    [Range(-180, 180, ErrorMessage = "El campo {0} debe estar en el rango de {1} a {2}.")] double Lon);
