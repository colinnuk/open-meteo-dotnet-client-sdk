using GeoJSON.Text.Feature;
using GeoJSON.Text.Geometry;
using OpenMeteo.Weather.Forecast.Metadata;
using OpenMeteo.Weather.Forecast.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace OpenMeteo.Weather.Forecast.RegionalGeoJSON;

public class RegionalModelCoverageHelper : IRegionalModelCoverageHelper
{
    private static readonly string _geojsonDirectory = Path.Combine(
        AppContext.BaseDirectory, "Weather", "Forecast", "RegionalGeoJSON");

    private readonly ConcurrentDictionary<string, FeatureCollection?> _cache = new();

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

        FeatureCollection? featureCollection = _cache.GetOrAdd(metaName, LoadFeatureCollection);

        if (featureCollection?.Features == null || featureCollection.Features.Count == 0)
            return null;

        foreach (Feature feature in featureCollection.Features)
        {
            if (feature.Geometry is Polygon polygon && IsPointInPolygon(polygon, latitude, longitude))
                return true;
        }

        return false;
    }

    private static FeatureCollection? LoadFeatureCollection(string metaName)
    {
        string filePath = Path.Combine(_geojsonDirectory, $"{metaName}.geojson");
        if (!File.Exists(filePath))
            return null;

        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<FeatureCollection>(json);
    }

    private static bool IsPointInPolygon(Polygon polygon, double latitude, double longitude)
    {
        if (polygon.Coordinates.Count == 0)
            return false;

        // Check against the exterior ring (index 0); ignore holes for coverage purposes
        return IsPointInRing(polygon.Coordinates[0].Coordinates, latitude, longitude);
    }

    private static bool IsPointInRing(IReadOnlyList<IPosition> ring, double latitude, double longitude)
    {
        // Ray-casting algorithm
        int n = ring.Count;
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
