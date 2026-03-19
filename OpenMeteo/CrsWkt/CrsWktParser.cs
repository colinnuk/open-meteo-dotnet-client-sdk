using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using ProjNet.CoordinateSystems;

namespace OpenMeteo.CrsWkt;

/// <summary>
/// Parses OGC WKT2 coordinate reference system strings as used by the Open-Meteo API,
/// and builds ProjNet <see cref="CoordinateSystem"/> objects for coordinate transformations.
/// </summary>
public class CrsWktParser : ICrsWktParser
{
    private static readonly Regex AreaOfUseRegex = new(
        @"BBOX\[(-?[\d.]+),\s*(-?[\d.]+),\s*(-?[\d.]+),\s*(-?[\d.]+)\]",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex EllipsoidRegex = new(
        @"ELLIPSOID\[""([^""]+)"",\s*([\d.]+),\s*([\d.]+)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex MethodRegex = new(
        @"METHOD\[""([^""]+)""\]",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex ParameterRegex = new(
        @"PARAMETER\[""([^""]+)"",\s*(-?[\d.]+)\]",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Dictionary<string, string> Wkt2ToProjNetParameterNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Latitude of natural origin"] = "latitude_of_origin",
        ["Longitude of natural origin"] = "central_meridian",
        ["Scale factor at natural origin"] = "scale_factor",
        ["False easting"] = "false_easting",
        ["False northing"] = "false_northing",
        ["Latitude of false origin"] = "latitude_of_origin",
        ["Longitude of false origin"] = "central_meridian",
        ["Latitude of 1st standard parallel"] = "standard_parallel_1",
        ["Latitude of 2nd standard parallel"] = "standard_parallel_2",
        ["Easting at false origin"] = "false_easting",
        ["Northing at false origin"] = "false_northing",
    };

    private static readonly Dictionary<string, string> Wkt2ToProjNetMethodNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Stereographic"] = "Oblique_Stereographic",
        ["Polar Stereographic"] = "Polar_Stereographic",
        ["Transverse Mercator"] = "Transverse_Mercator",
        ["Lambert Conformal Conic"] = "Lambert_Conformal_Conic_2SP",
        ["Mercator"] = "Mercator_1SP",
        ["Lambert Azimuthal Equal Area"] = "Lambert_Azimuthal_Equal_Area",
        ["Albers Equal Area"] = "Albers",
    };

    /// <inheritdoc />
    public (double West, double South, double East, double North)? ParseAreaOfUse(string wkt)
    {
        var match = AreaOfUseRegex.Match(wkt);
        if (!match.Success)
            return null;

        return (
            double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
            double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
            double.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture),
            double.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Checks whether the WKT string represents a Gaussian grid
    /// (reduced or regular).
    /// </summary>
    public bool IsGaussianGrid(string wkt) =>
        wkt.Contains("Reduced Gaussian Grid", StringComparison.OrdinalIgnoreCase) ||
        wkt.Contains("Gaussian Grid", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks whether the WKT string defines a projected CRS (PROJCRS).
    /// </summary>
    public bool IsProjectedCrs(string wkt) =>
        wkt.TrimStart().StartsWith("PROJCRS", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Parses a WKT2 string into a ProjNet <see cref="CoordinateSystem"/>.
    /// Returns a <see cref="ProjectedCoordinateSystem"/> for PROJCRS, or a
    /// <see cref="GeographicCoordinateSystem"/> for GEOGCRS.
    /// </summary>
    public CoordinateSystem? ParseCoordinateSystem(string wkt)
    {
        if (string.IsNullOrWhiteSpace(wkt))
            return null;

        var csFactory = new CoordinateSystemFactory();

        var ellipsoidMatch = EllipsoidRegex.Match(wkt);
        if (!ellipsoidMatch.Success)
            return null;

        var ellipsoidName = ellipsoidMatch.Groups[1].Value;
        var semiMajorAxis = double.Parse(ellipsoidMatch.Groups[2].Value, CultureInfo.InvariantCulture);
        var inverseFlattening = double.Parse(ellipsoidMatch.Groups[3].Value, CultureInfo.InvariantCulture);

        var ellipsoid = csFactory.CreateFlattenedSphere(ellipsoidName, semiMajorAxis, inverseFlattening, LinearUnit.Metre);
        var datum = csFactory.CreateHorizontalDatum(ellipsoidName, DatumType.HD_Geocentric, ellipsoid, null);
        var gcs = csFactory.CreateGeographicCoordinateSystem(
            ellipsoidName, AngularUnit.Degrees, datum, PrimeMeridian.Greenwich,
            new AxisInfo("Lat", AxisOrientationEnum.North),
            new AxisInfo("Lon", AxisOrientationEnum.East));

        if (!IsProjectedCrs(wkt))
            return gcs;

        var methodMatch = MethodRegex.Match(wkt);
        if (!methodMatch.Success)
            return gcs;

        var wkt2MethodName = methodMatch.Groups[1].Value;

        var parameters = new List<ProjectionParameter>();
        foreach (Match paramMatch in ParameterRegex.Matches(wkt))
        {
            var wkt2Name = paramMatch.Groups[1].Value;
            var value = double.Parse(paramMatch.Groups[2].Value, CultureInfo.InvariantCulture);

            var projNetName = Wkt2ToProjNetParameterNames.TryGetValue(wkt2Name, out var mapped)
                ? mapped
                : wkt2Name;

            parameters.Add(new ProjectionParameter(projNetName, value));
        }

        var projNetMethodName = ResolveProjNetMethodName(wkt2MethodName, parameters);

        var projection = csFactory.CreateProjection(wkt2MethodName, projNetMethodName, parameters);
        return csFactory.CreateProjectedCoordinateSystem(
            wkt2MethodName, gcs, projection, LinearUnit.Metre,
            new AxisInfo("E", AxisOrientationEnum.East),
            new AxisInfo("N", AxisOrientationEnum.North));
    }

    private static string ResolveProjNetMethodName(string wkt2Method, List<ProjectionParameter> parameters)
    {
        if (Wkt2ToProjNetMethodNames.TryGetValue(wkt2Method, out var mapped))
        {
            // Distinguish Polar vs Oblique Stereographic by latitude of origin
            if (string.Equals(mapped, "Oblique_Stereographic", StringComparison.OrdinalIgnoreCase))
            {
                var latOrigin = parameters.Find(p =>
                    string.Equals(p.Name, "latitude_of_origin", StringComparison.OrdinalIgnoreCase));
                if (latOrigin is not null && Math.Abs(latOrigin.Value) >= 89.99)
                    return "Polar_Stereographic";
            }

            return mapped;
        }

        return wkt2Method;
    }
}
