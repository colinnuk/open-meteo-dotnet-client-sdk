using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.CrsWkt;

namespace OpenMeteoTests.CrsWkt;

[TestClass]
public class GaussianGridTests
{
    private const string O320Wkt = """
        GEOGCRS["Reduced Gaussian Grid O320",
            DATUM["WGS 1984",
                ELLIPSOID["WGS 84",6378137,298.257223563]],
            CS[ellipsoidal,2],
                AXIS["latitude",north],
                AXIS["longitude",east],
                ANGLEUNIT["degree",0.0174532925199433],
            REMARK["O320"]]
        """;

    private const int O320TotalPoints = 4 * 320 * (320 + 9); // 421120

    [TestMethod]
    public void Constructor_O320_SetsShapeCorrectly()
    {
        var grid = new GaussianGrid(O320Wkt, (1, O320TotalPoints));

        Assert.AreEqual((1, O320TotalPoints), grid.Shape);
        Assert.AreEqual("O320", grid.GridType);
    }

    [TestMethod]
    public void Constructor_InvalidNy_Throws()
    {
        Assert.ThrowsException<ArgumentException>(
            () => new GaussianGrid(O320Wkt, (2, O320TotalPoints)));
    }

    [TestMethod]
    public void Constructor_WrongPointCount_Throws()
    {
        Assert.ThrowsException<ArgumentException>(
            () => new GaussianGrid(O320Wkt, (1, 12345)));
    }

    [TestMethod]
    public void GetCoordinates_O320_FirstPoint_ReturnsNorthPoleRegion()
    {
        var grid = new GaussianGrid(O320Wkt, (1, O320TotalPoints));

        var coords = grid.GetCoordinates(0, 0);

        // First latitude line near north pole
        Assert.IsTrue(coords.Lat > 80.0, $"Expected latitude > 80, got {coords.Lat}");
    }

    [TestMethod]
    public void GetCoordinates_O320_InvalidY_Throws()
    {
        var grid = new GaussianGrid(O320Wkt, (1, O320TotalPoints));

        Assert.ThrowsException<ArgumentException>(
            () => grid.GetCoordinates(0, 1));
    }

    [TestMethod]
    public void FindPointXY_O320_Equator_ReturnsValidIndex()
    {
        var grid = new GaussianGrid(O320Wkt, (1, O320TotalPoints));

        var idx = grid.FindPointXY(0.0, 0.0);

        Assert.IsNotNull(idx);
        Assert.AreEqual(0, idx.Value.Y);
        Assert.IsTrue(idx.Value.X >= 0 && idx.Value.X < O320TotalPoints);
    }

    [TestMethod]
    public void GetCoordinates_FindPointXY_RoundTrip()
    {
        var grid = new GaussianGrid(O320Wkt, (1, O320TotalPoints));

        // Get coordinates of a known grid point
        var coords = grid.GetCoordinates(1000, 0);

        // Find that point back
        var idx = grid.FindPointXY(coords.Lat, coords.Lon);

        Assert.IsNotNull(idx);
        // The round-trip should give back a nearby grid point
        var backCoords = grid.GetCoordinates(idx.Value.X, 0);
        Assert.AreEqual(coords.Lat, backCoords.Lat, 1.0);
        Assert.AreEqual(coords.Lon, backCoords.Lon, 1.0);
    }

    [TestMethod]
    public void Latitude_O320_HasCorrectShape()
    {
        var grid = new GaussianGrid(O320Wkt, (1, O320TotalPoints));

        var lats = grid.Latitude;

        Assert.AreEqual(1, lats.GetLength(0));
        Assert.AreEqual(O320TotalPoints, lats.GetLength(1));
    }

    [TestMethod]
    public void GetCoordinates_O320_LongitudeWithinRange()
    {
        var grid = new GaussianGrid(O320Wkt, (1, O320TotalPoints));

        // Check several grid points have valid lon range
        for (int i = 0; i < O320TotalPoints; i += O320TotalPoints / 10)
        {
            var coords = grid.GetCoordinates(i, 0);
            Assert.IsTrue(coords.Lon >= -180.0 && coords.Lon < 180.0,
                $"Longitude {coords.Lon} out of range at gridpoint {i}");
            Assert.IsTrue(coords.Lat >= -90.0 && coords.Lat <= 90.0,
                $"Latitude {coords.Lat} out of range at gridpoint {i}");
        }
    }

    [TestMethod]
    public void Constructor_N320ByPointCount_DetectsCorrectly()
    {
        // N320 has 542080 total points, detected by count
        var wkt = """GEOGCRS["Gaussian Grid N320", DATUM["D", ELLIPSOID["S",6371229,0]]]""";
        var grid = new GaussianGrid(wkt, (1, 542080));

        Assert.AreEqual("N320", grid.GridType);
    }

    [TestMethod]
    public void Constructor_N160ByPointCount_DetectsCorrectly()
    {
        // N160 has 138346 total points, detected by count
        var wkt = """GEOGCRS["Gaussian Grid N160", DATUM["D", ELLIPSOID["S",6371229,0]]]""";
        var grid = new GaussianGrid(wkt, (1, 138346));

        Assert.AreEqual("N160", grid.GridType);
    }
}
