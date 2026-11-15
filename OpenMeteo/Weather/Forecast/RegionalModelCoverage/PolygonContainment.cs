namespace OpenMeteo.Weather.Forecast.RegionalModelCoverage;

internal static class PolygonContainment
{
    internal static bool IsPointInRing((double Longitude, double Latitude)[] ring, double latitude, double longitude)
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
