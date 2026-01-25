# Clima API

Una API de clima simple construida con ASP.NET Core para obtener datos meteorológicos actuales y pronósticos.

## Características

-   **Clima Actual:** Obtiene las condiciones climáticas actuales para una ubicación geográfica específica (latitud y longitud).
-   **Pronóstico por Hora:** Proporciona un pronóstico del tiempo para las próximas 24 horas.
-   **Pronóstico por Días:** Ofrece un pronóstico del tiempo para los próximos 3 días.
-   **Resiliencia:** Utiliza Polly para implementar políticas de reintento, circuit breaker y timeout para manejar fallas en la API externa.
-   **Caché:** Implementa un caché en memoria para reducir la cantidad de llamadas a la API externa y mejorar el rendimiento. Sirve datos obsoletos (stale) si el servicio no está disponible.
-   **Documentación:** Documentación de API interactiva a través de Swagger UI.

## Prerrequisitos

-   .NET 8 SDK o superior.
-   Una clave de API de [WeatherAPI.com](https://www.weatherapi.com/).

## Configuración

1.  Clona este repositorio.
2.  Crea un archivo `appsettings.Development.json` en el directorio raíz del proyecto (`Clima_API`).
3.  Agrega tu configuración de WeatherAPI al archivo:

    ```json
    {
      "WeatherApi": {
        "BaseUrl": "https://api.weatherapi.com/v1/",
        "ApiKey": "TU_API_KEY_AQUI"
      }
    }
    ```

    Reemplaza `"TU_API_KEY_AQUI"` con tu clave de API real de WeatherAPI.com.

## Cómo ejecutar la aplicación

Navega al directorio del proyecto en tu terminal y ejecuta el siguiente comando:

```bash
dotnet run
```

La API estará disponible en `https://localhost:<puerto>` y `http://localhost:<puerto>`.

## Uso de la API

Una vez que la aplicación se está ejecutando, puedes acceder a la documentación interactiva de Swagger en tu navegador en la siguiente URL para probar los endpoints:

`https://localhost:<puerto>/swagger`

### Endpoints

Todos los endpoints que esperan una ubicación reciben un cuerpo de solicitud con la siguiente estructura:

```json
{
  "lat": 40.7128,
  "lon": -74.0060
}
```

#### Obtener Clima Actual
-   **`POST /api/v1/weather/by-gps`**: Devuelve el objeto del clima actual.

#### Obtener Pronóstico de 24 Horas
-   **`POST /api/v1/weather/forecast/next-24h`**: Devuelve una lista de pronósticos por hora.

#### Obtener Pronóstico de 3 Días
-   **`POST /api/v1/weather/forecast/next-3d`**: Devuelve una lista de pronósticos por día.