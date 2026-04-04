using OpenMeteo.Weather.Forecast.Options;

namespace OpenMeteo.Weather.Forecast.RegionalGeoJSON;

public interface IRegionalModelCoverageHelper
{
    /// <summary>
    /// Determines whether a given latitude/longitude point falls within the coverage area
    /// of the specified weather model's regional GeoJSON boundary.
    /// Returns <see langword="null"/> when no regional GeoJSON file exists for the model
    /// (e.g. global or seamless models).
    /// </summary>
    bool? IsLocationInModelCoverage(WeatherModelOptionsParameter weatherModel, float latitude, float longitude);
}
