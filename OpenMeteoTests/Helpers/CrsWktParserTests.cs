using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Helpers;

namespace OpenMeteoTests.Helpers;

[TestClass]
public class CrsWktParserTests
{
    [TestMethod]
    public void ParseBoundingBox_RegularGeographic_ReturnsCorrectBbox()
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

        var result = CrsWktParser.ParseBoundingBox(wkt);

        Assert.IsNotNull(result);
        Assert.AreEqual(-90.0m, result.South);
        Assert.AreEqual(-180.0m, result.West);
        Assert.AreEqual(90.0m, result.North);
        Assert.AreEqual(179.75m, result.East);
    }

    [TestMethod]
    public void ParseBoundingBox_RotatedLatLon_ReturnsCorrectBbox()
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

        var result = CrsWktParser.ParseBoundingBox(wkt);

        Assert.IsNotNull(result);
        Assert.AreEqual(45.92686m, result.South);
        Assert.AreEqual(-126.25641m, result.West);
        Assert.AreEqual(60.2894m, result.North);
        Assert.AreEqual(-114.45587m, result.East);
    }

    [TestMethod]
    public void ParseBoundingBox_Stereographic_ReturnsCorrectBbox()
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

        var result = CrsWktParser.ParseBoundingBox(wkt);

        Assert.IsNotNull(result);
        Assert.AreEqual(18.145027m, result.South);
        Assert.AreEqual(-142.89252m, result.West);
        Assert.AreEqual(45.40545m, result.North);
        Assert.AreEqual(-10.174438m, result.East);
    }

    [TestMethod]
    public void ParseBoundingBox_EmptyString_ReturnsNull()
    {
        var result = CrsWktParser.ParseBoundingBox(string.Empty);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void ParseBoundingBox_NoBboxPresent_ReturnsNull()
    {
        const string wkt = """GEOGCRS["WGS 84", DATUM["World Geodetic System 1984"]]""";

        var result = CrsWktParser.ParseBoundingBox(wkt);

        Assert.IsNull(result);
    }
}
