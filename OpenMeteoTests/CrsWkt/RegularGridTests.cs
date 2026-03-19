using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.CrsWkt;

namespace OpenMeteoTests.CrsWkt;

[TestClass]
public class RegularGridTests
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

    private const string StereographicWkt = """
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

    [TestMethod]
    public void Constructor_GeographicCrs_SetsShapeCorrectly()
    {
        var grid = new RegularGrid(Wgs84Wkt, (721, 1440));

        Assert.AreEqual((721, 1440), grid.Shape);
    }

    [TestMethod]
    public void Constructor_InvalidShape_Throws()
    {
        Assert.ThrowsException<ArgumentException>(() => new RegularGrid(Wgs84Wkt, (1, 1)));
    }

    [TestMethod]
    public void GetCoordinates_GeographicCrs_Origin_ReturnsSouthWest()
    {
        var grid = new RegularGrid(Wgs84Wkt, (721, 1441));

        var origin = grid.GetCoordinates(0, 0);

        Assert.AreEqual(-90.0, origin.Lat, 0.001);
        Assert.AreEqual(-180.0, origin.Lon, 0.001);
    }

    [TestMethod]
    public void GetCoordinates_GeographicCrs_LastPoint_ReturnsNorthEast()
    {
        var grid = new RegularGrid(Wgs84Wkt, (721, 1441));

        var last = grid.GetCoordinates(1440, 720);

        Assert.AreEqual(90.0, last.Lat, 0.001);
        Assert.AreEqual(180.0, last.Lon, 0.001);
    }

    [TestMethod]
    public void FindPointXY_GeographicCrs_CenterPoint_ReturnsCorrectIndices()
    {
        var grid = new RegularGrid(Wgs84Wkt, (721, 1441));

        var idx = grid.FindPointXY(0.0, 0.0);

        Assert.IsNotNull(idx);
        Assert.AreEqual(720, idx.Value.X);
        Assert.AreEqual(360, idx.Value.Y);
    }

    [TestMethod]
    public void FindPointXY_GeographicCrs_OutOfBounds_ReturnsNull()
    {
        var grid = new RegularGrid(Wgs84Wkt, (721, 1441));

        var idx = grid.FindPointXY(95.0, 0.0);

        Assert.IsNull(idx);
    }

    [TestMethod]
    public void Latitude_GeographicCrs_HasCorrectShape()
    {
        var grid = new RegularGrid(Wgs84Wkt, (3, 5));

        var lats = grid.Latitude;

        Assert.AreEqual(3, lats.GetLength(0));
        Assert.AreEqual(5, lats.GetLength(1));
    }

    [TestMethod]
    public void Longitude_GeographicCrs_HasCorrectShape()
    {
        var grid = new RegularGrid(Wgs84Wkt, (3, 5));

        var lons = grid.Longitude;

        Assert.AreEqual(3, lons.GetLength(0));
        Assert.AreEqual(5, lons.GetLength(1));
    }

    [TestMethod]
    public void Constructor_StereographicCrs_SetsShapeCorrectly()
    {
        var grid = new RegularGrid(StereographicWkt, (10, 20));

        Assert.AreEqual((10, 20), grid.Shape);
    }

    [TestMethod]
    public void FindPointXY_StereographicCrs_PointInBounds_ReturnsIndex()
    {
        var grid = new RegularGrid(StereographicWkt, (100, 200));

        var idx = grid.FindPointXY(30.0, -80.0);

        Assert.IsNotNull(idx);
        Assert.IsTrue(idx.Value.X >= 0 && idx.Value.X < 200);
        Assert.IsTrue(idx.Value.Y >= 0 && idx.Value.Y < 100);
    }

    [TestMethod]
    public void GetCoordinates_StereographicCrs_RoundTrip()
    {
        var grid = new RegularGrid(StereographicWkt, (100, 200));

        var idx = grid.FindPointXY(30.0, -80.0);
        Assert.IsNotNull(idx);

        var coords = grid.GetCoordinates(idx.Value.X, idx.Value.Y);

        Assert.AreEqual(30.0, coords.Lat, 1.0);
        // Normalize longitude to [-180, 180) for comparison
        double normLon = ((coords.Lon % 360) + 540) % 360 - 180;
        Assert.AreEqual(-80.0, normLon, 1.0);
    }
}
