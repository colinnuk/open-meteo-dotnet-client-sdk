using OpenMeteo.Weather.Forecast.Metadata;
using OpenMeteo.Weather.Forecast.Options;
using System;

namespace OpenMeteo.Weather.Forecast.RegionalGeoJSON;

public class RegionalModelCoverageHelper : IRegionalModelCoverageHelper
{
    /// <inheritdoc/>
    public bool? IsLocationInModelCoverage(WeatherModelOptionsParameter weatherModel, float latitude, float longitude)
    {
        string metaName;
        try
        {
            metaName = MetadataNameHelper.GetMetadataUrlName(weatherModel);
        }
        catch (ArgumentOutOfRangeException)
        {
            return null;
        }

        var ring = RegionalModelPolygons.GetExteriorRing(metaName);
        if (ring == null)
            return null;

        return IsPointInRing(ring, latitude, longitude);
    }

    private static bool IsPointInRing((double Longitude, double Latitude)[] ring, double latitude, double longitude)
    {
        // Ray-casting algorithm
        int n = ring.Length;
        bool inside = false;

        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            double xi = ring[i].Longitude;
            double yi = ring[i].Latitude;
            double xj = ring[j].Longitude;
            double yj = ring[j].Latitude;

            if (((yi > latitude) != (yj > latitude)) &&
                (longitude < (xj - xi) * (latitude - yi) / (yj - yi) + xi))
            {
                inside = !inside;
            }
        }

        return inside;
    }
}
