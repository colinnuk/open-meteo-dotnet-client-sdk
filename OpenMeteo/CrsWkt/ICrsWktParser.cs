using ProjNet.CoordinateSystems;

namespace OpenMeteo.CrsWkt;

/// <summary>
/// Parses OGC WKT2 coordinate reference system strings and builds ProjNet
/// <see cref="CoordinateSystem"/> objects for coordinate transformations.
/// </summary>
public interface ICrsWktParser
{
    bool IsGaussianGrid(string wkt);
    bool IsProjectedCrs(string wkt);
    CoordinateSystem? ParseCoordinateSystem(string wkt);

    /// <summary>
    /// Extracts the geographic area of use from the WKT USAGE/BBOX clause.
    /// Equivalent to pyproj's <c>crs.area_of_use</c>.
    /// </summary>
    (double West, double South, double East, double North)? ParseAreaOfUse(string wkt);
}
