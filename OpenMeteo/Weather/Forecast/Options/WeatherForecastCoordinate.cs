namespace OpenMeteo.Weather.Forecast.Options
{
    /// <summary>
    /// A geographical WGS84 coordinate used for a weather forecast request.
    /// </summary>
    public record WeatherForecastCoordinate(float Latitude, float Longitude);
}
