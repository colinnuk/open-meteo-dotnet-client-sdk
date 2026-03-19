using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.CrsWkt;

namespace OpenMeteoTests.CrsWkt;

[TestClass]
public class CrsWktParserTests
{
    private readonly CrsWktParser _parser = new();

    [TestMethod]
    public void ParseAreaOfUse_RegularGeographic_ReturnsCorrectBounds()
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

        var result = _parser.ParseAreaOfUse(wkt);

        Assert.IsNotNull(result);
        Assert.AreEqual(-90.0, result.Value.South);
        Assert.AreEqual(-180.0, result.Value.West);
        Assert.AreEqual(90.0, result.Value.North);
        Assert.AreEqual(179.75, result.Value.East);
    }

    [TestMethod]
    public void ParseAreaOfUse_RotatedLatLon_ReturnsCorrectBounds()
    {
        const string wkt = """
            GEOGCRS["Rotated Lat/Lon",
                BASEGEOGCRS["GCS_Sphere",
                    DATUM["D_Sphere",
                        ELLIPSOID["Sphere",6371229.0,0.0]]],
                DERIVINGCONVERSION["Rotated Lat/Lon",
                    METHOD["PROJ ob_tran o_proj=longlat"],
                    PARAMETER["o_lon_p",0],
                    PARAMETER["o_lat_p",-33.443382],
                    PARAMETER["lon_0",86.46358]]
                CS[ellipsoidal,2],
                    AXIS["latitude",north],
                    AXIS["longitude",east],
                    ANGLEUNIT["degree",0.0174532925199433],
                USAGE[
                    SCOPE["grid"],
                    BBOX[45.92686,-126.25641,60.2894,-114.45587]]]
            """;

        var result = _parser.ParseAreaOfUse(wkt);

        Assert.IsNotNull(result);
        Assert.AreEqual(45.92686, result.Value.South, 1e-6);
        Assert.AreEqual(-126.25641, result.Value.West, 1e-6);
        Assert.AreEqual(60.2894, result.Value.North, 1e-6);
        Assert.AreEqual(-114.45587, result.Value.East, 1e-6);
    }

    [TestMethod]
    public void ParseAreaOfUse_Stereographic_ReturnsCorrectBounds()
    {
        const string wkt = """
            PROJCRS["Stereographic",
                BASEGEOGCRS["WGS 84",
                    DATUM["World Geodetic System 1984",
                        ELLIPSOID["WGS 84",6371229.0,298.257223563]]],
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

        var result = _parser.ParseAreaOfUse(wkt);

        Assert.IsNotNull(result);
        Assert.AreEqual(18.145027, result.Value.South, 1e-6);
        Assert.AreEqual(-142.89252, result.Value.West, 1e-6);
        Assert.AreEqual(45.40545, result.Value.North, 1e-6);
        Assert.AreEqual(-10.174438, result.Value.East, 1e-6);
    }

    [TestMethod]
    public void ParseAreaOfUse_EmptyString_ReturnsNull()
    {
        var result = _parser.ParseAreaOfUse(string.Empty);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void ParseAreaOfUse_NoBboxPresent_ReturnsNull()
    {
        const string wkt = """GEOGCRS["WGS 84", DATUM["World Geodetic System 1984"]]""";

        var result = _parser.ParseAreaOfUse(wkt);

        Assert.IsNull(result);
    }
}
