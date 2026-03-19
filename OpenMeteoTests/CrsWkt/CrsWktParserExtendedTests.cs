using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.CrsWkt;
using ProjNet.CoordinateSystems;

namespace OpenMeteoTests.CrsWkt;

[TestClass]
public class CrsWktParserExtendedTests
{
    [TestMethod]
    public void IsGaussianGrid_ReducedGaussian_ReturnsTrue()
    {
        const string wkt = """GEOGCRS["Reduced Gaussian Grid O320"]""";
        Assert.IsTrue(CrsWktParser.IsGaussianGrid(wkt));
    }

    [TestMethod]
    public void IsGaussianGrid_PlainGaussian_ReturnsTrue()
    {
        const string wkt = """GEOGCRS["Gaussian Grid N320"]""";
        Assert.IsTrue(CrsWktParser.IsGaussianGrid(wkt));
    }

    [TestMethod]
    public void IsGaussianGrid_RegularGrid_ReturnsFalse()
    {
        const string wkt = """GEOGCRS["WGS 84", DATUM["WGS 1984", ELLIPSOID["WGS 84",6378137,298.257223563]]]""";
        Assert.IsFalse(CrsWktParser.IsGaussianGrid(wkt));
    }

    [TestMethod]
    public void IsProjectedCrs_Projcrs_ReturnsTrue()
    {
        const string wkt = """PROJCRS["Stereographic", BASEGEOGCRS["WGS 84"]]""";
        Assert.IsTrue(CrsWktParser.IsProjectedCrs(wkt));
    }

    [TestMethod]
    public void IsProjectedCrs_Geogcrs_ReturnsFalse()
    {
        const string wkt = """GEOGCRS["WGS 84"]""";
        Assert.IsFalse(CrsWktParser.IsProjectedCrs(wkt));
    }

    [TestMethod]
    public void ParseCoordinateSystem_GeographicWkt_ReturnsGeographicCoordinateSystem()
    {
        const string wkt = """
            GEOGCRS["WGS 84",
                DATUM["World Geodetic System 1984",
                    ELLIPSOID["WGS 84",6378137,298.257223563]],
                CS[ellipsoidal,2],
                    AXIS["latitude",north],
                    AXIS["longitude",east],
                    ANGLEUNIT["degree",0.0174532925199433]
                USAGE[
                    SCOPE["grid"],
                    BBOX[-90.0,-180.0,90.0,179.75]]]
            """;

        var crs = CrsWktParser.ParseCoordinateSystem(wkt);

        Assert.IsNotNull(crs);
        Assert.IsInstanceOfType(crs, typeof(GeographicCoordinateSystem));
    }

    [TestMethod]
    public void ParseCoordinateSystem_ProjectedWkt_ReturnsProjectedCoordinateSystem()
    {
        const string wkt = """
            PROJCRS["Stereographic",
                BASEGEOGCRS["WGS 84",
                    DATUM["World Geodetic System 1984",
                        ELLIPSOID["WGS 84",6378137,298.257223563]]],
                CONVERSION["Stereographic",
                    METHOD["Stereographic"],
                    PARAMETER["Latitude of natural origin", 90.0],
                    PARAMETER["Longitude of natural origin", 249.0],
                    PARAMETER["Scale factor at natural origin", 1.0],
                    PARAMETER["False easting", 0.0],
                    PARAMETER["False northing", 0.0]],
                CS[Cartesian,2],
                    AXIS["easting",east],
                    AXIS["northing",north],
                    LENGTHUNIT["metre",1.0],
                USAGE[
                    SCOPE["grid"],
                    BBOX[18.145027,-142.89252,45.40545,-10.174438]]]
            """;

        var crs = CrsWktParser.ParseCoordinateSystem(wkt);

        Assert.IsNotNull(crs);
        Assert.IsInstanceOfType(crs, typeof(ProjectedCoordinateSystem));
    }

    [TestMethod]
    public void ParseCoordinateSystem_NullString_ReturnsNull()
    {
        Assert.IsNull(CrsWktParser.ParseCoordinateSystem(null!));
    }

    [TestMethod]
    public void ParseCoordinateSystem_NoEllipsoid_ReturnsNull()
    {
        const string wkt = """GEOGCRS["WGS 84", DATUM["WGS 1984"]]""";
        Assert.IsNull(CrsWktParser.ParseCoordinateSystem(wkt));
    }
}
