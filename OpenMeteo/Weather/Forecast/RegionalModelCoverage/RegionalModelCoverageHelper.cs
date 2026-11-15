using OpenMeteo.Weather.Forecast.Options;

namespace OpenMeteo.Weather.Forecast.RegionalModelCoverage;

public class RegionalModelCoverageHelper : IRegionalModelCoverageHelper
{
    /// <inheritdoc/>
    public bool? IsLocationInModelCoverage(WeatherModelOptionsParameter weatherModel, float latitude, float longitude)
    {
        var ring = RegionalModelPolygons.GetExteriorRing(weatherModel);
        if (ring == null)
            return null;

        return PolygonContainment.IsPointInRing(ring, latitude, longitude);
    }
}
