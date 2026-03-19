using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.CrsWkt;

namespace OpenMeteoTests.CrsWkt;

[TestClass]
public class OmGridTests
{
    private const string Wgs84Wkt = """
        GEOGCRS["WGS 84",
            DATUM["World Geodetic System 1984",
                ELLIPSOID["WGS 84",6378137,298.257223563]],
            CS[ellipsoidal,2],
                AXIS["latitude",north],
                AXIS["longitude",east],
                ANGLEUNIT["degree",0.0174532925199433]
            USAGE[
                SCOPE["grid"],
                BBOX[-90.0,-180.0,90.0,180.0]]]
        """;

    private const string GaussianWkt = """
        GEOGCRS["Reduced Gaussian Grid O320",
            DATUM["WGS 1984",
                ELLIPSOID["WGS 84",6378137,298.257223563]],
            CS[ellipsoidal,2],
                AXIS["latitude",north],
                AXIS["longitude",east],
                ANGLEUNIT["degree",0.0174532925199433],
            REMARK["O320"]]
        """;

    [TestMethod]
    public void Constructor_RegularGrid_IsNotGaussian()
    {
        var grid = new OmGrid(Wgs84Wkt, (721, 1441));

        Assert.IsFalse(grid.IsGaussian);
    }

    [TestMethod]
    public void Constructor_GaussianGrid_IsGaussian()
    {
        int o320Points = 4 * 320 * (320 + 9);
        var grid = new OmGrid(GaussianWkt, (1, o320Points));

        Assert.IsTrue(grid.IsGaussian);
    }

    [TestMethod]
    public void IsGaussianGrid_RegularWkt_ReturnsFalse()
    {
        Assert.IsFalse(CrsWktParser.IsGaussianGrid(Wgs84Wkt));
    }

    [TestMethod]
    public void IsGaussianGrid_GaussianWkt_ReturnsTrue()
    {
        Assert.IsTrue(CrsWktParser.IsGaussianGrid(GaussianWkt));
    }

    [TestMethod]
    public void IsProjectedCrs_GeographicWkt_ReturnsFalse()
    {
        Assert.IsFalse(CrsWktParser.IsProjectedCrs(Wgs84Wkt));
    }

    [TestMethod]
    public void IsProjectedCrs_ProjectedWkt_ReturnsTrue()
    {
        const string projectedWkt = """
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

        Assert.IsTrue(CrsWktParser.IsProjectedCrs(projectedWkt));
    }

    [TestMethod]
    public void ParseCoordinateSystem_GeographicWkt_ReturnsGeographicCrs()
    {
        var crs = CrsWktParser.ParseCoordinateSystem(Wgs84Wkt);

        Assert.IsNotNull(crs);
        Assert.IsInstanceOfType(crs,
            typeof(ProjNet.CoordinateSystems.GeographicCoordinateSystem));
    }

    [TestMethod]
    public void ParseCoordinateSystem_ProjectedWkt_ReturnsProjectedCrs()
    {
        const string projectedWkt = """
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

        var crs = CrsWktParser.ParseCoordinateSystem(projectedWkt);

        Assert.IsNotNull(crs);
        Assert.IsInstanceOfType(crs,
            typeof(ProjNet.CoordinateSystems.ProjectedCoordinateSystem));
    }

    [TestMethod]
    public void ParseCoordinateSystem_EmptyString_ReturnsNull()
    {
        var crs = CrsWktParser.ParseCoordinateSystem(string.Empty);

        Assert.IsNull(crs);
    }

    [TestMethod]
    public void GetMeshgrid_ReturnsCoordinateArrays()
    {
        var grid = new OmGrid(Wgs84Wkt, (3, 5));

        var (lons, lats) = grid.GetMeshgrid();

        Assert.AreEqual(3, lons.GetLength(0));
        Assert.AreEqual(5, lons.GetLength(1));
        Assert.AreEqual(3, lats.GetLength(0));
        Assert.AreEqual(5, lats.GetLength(1));
    }

    [TestMethod]
    public void Constructor_InvalidShape_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() => new OmGrid(Wgs84Wkt, (0, 10)));
    }
}
