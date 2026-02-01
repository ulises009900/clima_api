using System.ComponentModel.DataAnnotations;

namespace WeatherApi.Models.Requests;

/// <summary>
/// Representa una solicitud de ubicación con una fecha específica.
/// </summary>
/// <param name="Lat">Latitud. Debe estar entre -90 y 90.</param>
/// <param name="Lon">Longitud. Debe estar entre -180 y 180.</param>
/// <param name="Date">Fecha en formato yyyy-MM-dd.</param>
public record DateLocationRequest(
    [Range(-90, 90, ErrorMessage = "El campo {0} debe estar en el rango de {1} a {2}.")] double Lat,
    [Range(-180, 180, ErrorMessage = "El campo {0} debe estar en el rango de {1} a {2}.")] double Lon,
    DateOnly Date);

/// <summary>
/// Wrapper para coincidir con la estructura JSON { "request": { ... } }
/// </summary>
/// <param name="Request">El objeto de solicitud interno.</param>
public record DateLocationRequestWrapper(DateLocationRequest Request);
