using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.CrsWkt;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace OpenMeteoTests.CrsWkt;

[TestClass]
public class CrsWktParserExtendedTests
{
    private readonly CrsWktParser _parser = new();
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
                BBOX[-90.0,-180.0,90.0,179.75]]]
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

    private const string SphereWkt = """
        GEOGCRS["GCS_Sphere",
            DATUM["D_Sphere",
                ELLIPSOID["Sphere",6371229.0,0.0]],
            CS[ellipsoidal,2],
                AXIS["latitude",north],
                AXIS["longitude",east],
                ANGLEUNIT["degree",0.0174532925199433]
            USAGE[
                SCOPE["grid"],
                BBOX[-90.0,-180.0,90.0,180.0]]]
        """;

    // ── IsGaussianGrid ─────────────────────────────────────────────────

    [TestMethod]
    public void IsGaussianGrid_ReducedGaussian_ReturnsTrue()
    {
        const string wkt = """GEOGCRS["Reduced Gaussian Grid O320"]""";
        Assert.IsTrue(_parser.IsGaussianGrid(wkt));
    }

    [TestMethod]
    public void IsGaussianGrid_PlainGaussian_ReturnsTrue()
    {
        const string wkt = """GEOGCRS["Gaussian Grid N320"]""";
        Assert.IsTrue(_parser.IsGaussianGrid(wkt));
    }

    [TestMethod]
    public void IsGaussianGrid_RegularGrid_ReturnsFalse()
    {
        const string wkt = """GEOGCRS["WGS 84", DATUM["WGS 1984", ELLIPSOID["WGS 84",6378137,298.257223563]]]""";
        Assert.IsFalse(_parser.IsGaussianGrid(wkt));
    }

    // ── IsProjectedCrs

    [TestMethod]
    public void IsProjectedCrs_Projcrs_ReturnsTrue()
    {
        const string wkt = """PROJCRS["Stereographic", BASEGEOGCRS["WGS 84"]]""";
        Assert.IsTrue(_parser.IsProjectedCrs(wkt));
    }

    [TestMethod]
    public void IsProjectedCrs_Geogcrs_ReturnsFalse()
    {
        const string wkt = """GEOGCRS["WGS 84"]""";
        Assert.IsFalse(_parser.IsProjectedCrs(wkt));
    }

    // ── ParseCoordinateSystem: null / missing

    [TestMethod]
    public void ParseCoordinateSystem_NullString_ReturnsNull()
    {
        Assert.IsNull(_parser.ParseCoordinateSystem(null!));
    }

    [TestMethod]
    public void ParseCoordinateSystem_NoEllipsoid_ReturnsNull()
    {
        const string wkt = """GEOGCRS["WGS 84", DATUM["WGS 1984"]]""";
        Assert.IsNull(_parser.ParseCoordinateSystem(wkt));
    }

    // ── Geographic CRS

    [TestMethod]
    public void ParseCoordinateSystem_Wgs84_ReturnsGeographicCoordinateSystem()
    {
        var crs = _parser.ParseCoordinateSystem(Wgs84Wkt);

        Assert.IsNotNull(crs);
        Assert.IsInstanceOfType(crs, typeof(GeographicCoordinateSystem));
    }

    [TestMethod]
    public void ParseCoordinateSystem_Wgs84_EllipsoidSemiMajorAxisIsCorrect()
    {
        var gcs = _parser.ParseCoordinateSystem(Wgs84Wkt) as GeographicCoordinateSystem;

        Assert.IsNotNull(gcs);
        Assert.AreEqual(6378137.0, gcs.HorizontalDatum.Ellipsoid.SemiMajorAxis, 0.1);
    }

    [TestMethod]
    public void ParseCoordinateSystem_Wgs84_EllipsoidInverseFlatteningIsCorrect()
    {
        var gcs = _parser.ParseCoordinateSystem(Wgs84Wkt) as GeographicCoordinateSystem;

        Assert.IsNotNull(gcs);
        Assert.AreEqual(298.257223563, gcs.HorizontalDatum.Ellipsoid.InverseFlattening, 1e-6);
    }

    [TestMethod]
    public void ParseCoordinateSystem_Wgs84_SemiMinorAxisDerivedCorrectly()
    {
        var gcs = _parser.ParseCoordinateSystem(Wgs84Wkt) as GeographicCoordinateSystem;

        Assert.IsNotNull(gcs);
        // b = a * (1 - 1/f)
        Assert.AreEqual(6356752.3142, gcs.HorizontalDatum.Ellipsoid.SemiMinorAxis, 1.0);
    }

    [TestMethod]
    public void ParseCoordinateSystem_Sphere_InverseFlatteningIsZero()
    {
        var gcs = _parser.ParseCoordinateSystem(SphereWkt) as GeographicCoordinateSystem;

        Assert.IsNotNull(gcs);
        Assert.AreEqual(6371229.0, gcs.HorizontalDatum.Ellipsoid.SemiMajorAxis, 0.1);
        Assert.AreEqual(0.0, gcs.HorizontalDatum.Ellipsoid.InverseFlattening, 1e-10);
    }

    [TestMethod]
    public void ParseCoordinateSystem_Sphere_SemiMinorEqualsSemiMajor()
    {
        var gcs = _parser.ParseCoordinateSystem(SphereWkt) as GeographicCoordinateSystem;

        Assert.IsNotNull(gcs);
        // A sphere has equal axes
        Assert.AreEqual(
            gcs.HorizontalDatum.Ellipsoid.SemiMajorAxis,
            gcs.HorizontalDatum.Ellipsoid.SemiMinorAxis,
            0.1);
    }

    [TestMethod]
    public void ParseCoordinateSystem_Wgs84_AngularUnitIsDegrees()
    {
        var gcs = _parser.ParseCoordinateSystem(Wgs84Wkt) as GeographicCoordinateSystem;

        Assert.IsNotNull(gcs);
        Assert.AreEqual("degree", gcs.AngularUnit.Name, true);
    }

    // ── Projected CRS: structure and projection parameters ─────────────

    [TestMethod]
    public void ParseCoordinateSystem_Stereographic_ReturnsProjectedCoordinateSystem()
    {
        var crs = _parser.ParseCoordinateSystem(StereographicWkt);

        Assert.IsNotNull(crs);
        Assert.IsInstanceOfType(crs, typeof(ProjectedCoordinateSystem));
    }

    [TestMethod]
    public void ParseCoordinateSystem_Stereographic_BaseGcsHasCorrectEllipsoid()
    {
        var pcs = _parser.ParseCoordinateSystem(StereographicWkt) as ProjectedCoordinateSystem;

        Assert.IsNotNull(pcs);
        var ellipsoid = pcs.GeographicCoordinateSystem.HorizontalDatum.Ellipsoid;
        Assert.AreEqual(6378137.0, ellipsoid.SemiMajorAxis, 0.1);
        Assert.AreEqual(298.257223563, ellipsoid.InverseFlattening, 1e-6);
    }

    [TestMethod]
    public void ParseCoordinateSystem_Stereographic_ProjectionClassName_IsPolarStereographic()
    {
        var pcs = _parser.ParseCoordinateSystem(StereographicWkt) as ProjectedCoordinateSystem;

        Assert.IsNotNull(pcs);
        // Latitude of origin = 90 → resolved to Polar_Stereographic
        Assert.AreEqual("Polar_Stereographic", pcs.Projection.ClassName);
    }

    [TestMethod]
    public void ParseCoordinateSystem_Stereographic_LatitudeOfOrigin_Is90()
    {
        var pcs = _parser.ParseCoordinateSystem(StereographicWkt) as ProjectedCoordinateSystem;

        Assert.IsNotNull(pcs);
        var param = pcs.Projection.GetParameter("latitude_of_origin");
        Assert.IsNotNull(param);
        Assert.AreEqual(90.0, param.Value, 1e-10);
    }

    [TestMethod]
    public void ParseCoordinateSystem_Stereographic_CentralMeridian_Is249()
    {
        var pcs = _parser.ParseCoordinateSystem(StereographicWkt) as ProjectedCoordinateSystem;

        Assert.IsNotNull(pcs);
        var param = pcs.Projection.GetParameter("central_meridian");
        Assert.IsNotNull(param);
        Assert.AreEqual(249.0, param.Value, 1e-10);
    }

    [TestMethod]
    public void ParseCoordinateSystem_Stereographic_ScaleFactor_Is1()
    {
        var pcs = _parser.ParseCoordinateSystem(StereographicWkt) as ProjectedCoordinateSystem;

        Assert.IsNotNull(pcs);
        var param = pcs.Projection.GetParameter("scale_factor");
        Assert.IsNotNull(param);
        Assert.AreEqual(1.0, param.Value, 1e-10);
    }

    [TestMethod]
    public void ParseCoordinateSystem_Stereographic_FalseEastingNorthing_AreZero()
    {
        var pcs = _parser.ParseCoordinateSystem(StereographicWkt) as ProjectedCoordinateSystem;

        Assert.IsNotNull(pcs);
        Assert.AreEqual(0.0, pcs.Projection.GetParameter("false_easting")!.Value, 1e-10);
        Assert.AreEqual(0.0, pcs.Projection.GetParameter("false_northing")!.Value, 1e-10);
    }

    [TestMethod]
    public void ParseCoordinateSystem_Stereographic_LinearUnitIsMetres()
    {
        var pcs = _parser.ParseCoordinateSystem(StereographicWkt) as ProjectedCoordinateSystem;

        Assert.IsNotNull(pcs);
        Assert.AreEqual(1.0, pcs.LinearUnit.MetersPerUnit, 1e-10);
    }

    // ── Polar vs Oblique Stereographic distinction ─────────────────────

    [TestMethod]
    public void ParseCoordinateSystem_ObliqueStereographic_ClassName_IsObliqueStereographic()
    {
        const string wkt = """
            PROJCRS["Oblique Stereographic",
                BASEGEOGCRS["WGS 84",
                    DATUM["World Geodetic System 1984",
                        ELLIPSOID["WGS 84",6378137,298.257223563]]],
                CONVERSION["Oblique Stereographic",
                    METHOD["Stereographic"],
                    PARAMETER["Latitude of natural origin", 52.1562],
                    PARAMETER["Longitude of natural origin", 5.3876],
                    PARAMETER["Scale factor at natural origin", 0.9999079],
                    PARAMETER["False easting", 155000.0],
                    PARAMETER["False northing", 463000.0]],
                CS[Cartesian,2],
                    AXIS["easting",east],
                    AXIS["northing",north],
                    LENGTHUNIT["metre",1.0],
                USAGE[
                    SCOPE["grid"],
                    BBOX[50.75,3.37,53.47,7.21]]]
            """;

        var pcs = _parser.ParseCoordinateSystem(wkt) as ProjectedCoordinateSystem;

        Assert.IsNotNull(pcs);
        // Latitude of origin ≈ 52° → stays Oblique_Stereographic
        Assert.AreEqual("Oblique_Stereographic", pcs.Projection.ClassName);
    }

    // ── Coordinate transformations: mathematical correctness ───────────

    [TestMethod]
    public void Stereographic_Transform_NorthPole_MapsToOrigin()
    {
        var pcs = _parser.ParseCoordinateSystem(StereographicWkt) as ProjectedCoordinateSystem;
        Assert.IsNotNull(pcs);

        var ctFactory = new CoordinateTransformationFactory();
        var toProj = ctFactory.CreateFromCoordinateSystems(
            GeographicCoordinateSystem.WGS84, pcs).MathTransform;

        // The north pole (lat=90) at any longitude should map near the projection origin.
        // Central meridian is 249° = -111°; at the pole, all meridians converge.
        var (x, y) = toProj.Transform(249.0, 90.0);

        Assert.AreEqual(0.0, x, 1.0, "North pole easting should be ~0");
        Assert.AreEqual(0.0, y, 1.0, "North pole northing should be ~0");
    }

    [TestMethod]
    public void Stereographic_Transform_RoundTrip_PreservesCoordinates()
    {
        var pcs = _parser.ParseCoordinateSystem(StereographicWkt) as ProjectedCoordinateSystem;
        Assert.IsNotNull(pcs);

        var ctFactory = new CoordinateTransformationFactory();
        var toProj = ctFactory.CreateFromCoordinateSystems(
            GeographicCoordinateSystem.WGS84, pcs).MathTransform;
        var toWgs = ctFactory.CreateFromCoordinateSystems(
            pcs, GeographicCoordinateSystem.WGS84).MathTransform;

        double lon = -100.0, lat = 45.0;
        var (px, py) = toProj.Transform(lon, lat);
        var (lonBack, latBack) = toWgs.Transform(px, py);

        // Normalize longitude to [-180, 180)
        lonBack = ((lonBack % 360) + 540) % 360 - 180;

        Assert.AreEqual(lat, latBack, 1e-6, "Latitude should survive round-trip");
        Assert.AreEqual(lon, lonBack, 1e-6, "Longitude should survive round-trip");
    }

    [TestMethod]
    public void Stereographic_Transform_PointsAwayFromPole_HaveLargerRadius()
    {
        var pcs = _parser.ParseCoordinateSystem(StereographicWkt) as ProjectedCoordinateSystem;
        Assert.IsNotNull(pcs);

        var ctFactory = new CoordinateTransformationFactory();
        var toProj = ctFactory.CreateFromCoordinateSystems(
            GeographicCoordinateSystem.WGS84, pcs).MathTransform;

        // Points further from the pole should be further from the origin in projection space
        var (x60, y60) = toProj.Transform(249.0, 60.0);
        var (x30, y30) = toProj.Transform(249.0, 30.0);

        double r60 = Math.Sqrt(x60 * x60 + y60 * y60);
        double r30 = Math.Sqrt(x30 * x30 + y30 * y30);

        Assert.IsTrue(r30 > r60,
            $"lat=30° radius ({r30:F0}m) should be larger than lat=60° radius ({r60:F0}m)");
    }

    [TestMethod]
    public void Stereographic_Transform_KnownPoint_ProducesReasonableMetricValues()
    {
        var pcs = _parser.ParseCoordinateSystem(StereographicWkt) as ProjectedCoordinateSystem;
        Assert.IsNotNull(pcs);

        var ctFactory = new CoordinateTransformationFactory();
        var toProj = ctFactory.CreateFromCoordinateSystems(
            GeographicCoordinateSystem.WGS84, pcs).MathTransform;

        // Transform a mid-latitude North American point
        var (x, y) = toProj.Transform(-100.0, 45.0);

        // Should produce values in the millions-of-metres range (continental scale)
        double dist = Math.Sqrt(x * x + y * y);
        Assert.IsTrue(dist > 1_000_000, $"Distance from pole {dist:F0}m should be > 1,000 km");
        Assert.IsTrue(dist < 20_000_000, $"Distance from pole {dist:F0}m should be < 20,000 km");
    }

    [TestMethod]
    public void ObliqueStereographic_Transform_RoundTrip_PreservesCoordinates()
    {
        const string wkt = """
            PROJCRS["Oblique Stereographic",
                BASEGEOGCRS["WGS 84",
                    DATUM["World Geodetic System 1984",
                        ELLIPSOID["WGS 84",6378137,298.257223563]]],
                CONVERSION["Oblique Stereographic",
                    METHOD["Stereographic"],
                    PARAMETER["Latitude of natural origin", 52.1562],
                    PARAMETER["Longitude of natural origin", 5.3876],
                    PARAMETER["Scale factor at natural origin", 0.9999079],
                    PARAMETER["False easting", 155000.0],
                    PARAMETER["False northing", 463000.0]],
                CS[Cartesian,2],
                    AXIS["easting",east],
                    AXIS["northing",north],
                    LENGTHUNIT["metre",1.0],
                USAGE[
                    SCOPE["grid"],
                    BBOX[50.75,3.37,53.47,7.21]]]
            """;

        var pcs = _parser.ParseCoordinateSystem(wkt) as ProjectedCoordinateSystem;
        Assert.IsNotNull(pcs);

        var ctFactory = new CoordinateTransformationFactory();
        var toProj = ctFactory.CreateFromCoordinateSystems(
            GeographicCoordinateSystem.WGS84, pcs).MathTransform;
        var toWgs = ctFactory.CreateFromCoordinateSystems(
            pcs, GeographicCoordinateSystem.WGS84).MathTransform;

        // Amsterdam: 52.3676°N, 4.9041°E
        double lon = 4.9041, lat = 52.3676;
        var (px, py) = toProj.Transform(lon, lat);
        var (lonBack, latBack) = toWgs.Transform(px, py);

        Assert.AreEqual(lat, latBack, 1e-6);
        Assert.AreEqual(lon, lonBack, 1e-6);
    }

    [TestMethod]
    public void ObliqueStereographic_Transform_Origin_MapsToFalseEastingNorthing()
    {
        const string wkt = """
            PROJCRS["Oblique Stereographic",
                BASEGEOGCRS["WGS 84",
                    DATUM["World Geodetic System 1984",
                        ELLIPSOID["WGS 84",6378137,298.257223563]]],
                CONVERSION["Oblique Stereographic",
                    METHOD["Stereographic"],
                    PARAMETER["Latitude of natural origin", 52.1562],
                    PARAMETER["Longitude of natural origin", 5.3876],
                    PARAMETER["Scale factor at natural origin", 0.9999079],
                    PARAMETER["False easting", 155000.0],
                    PARAMETER["False northing", 463000.0]],
                CS[Cartesian,2],
                    AXIS["easting",east],
                    AXIS["northing",north],
                    LENGTHUNIT["metre",1.0],
                USAGE[
                    SCOPE["grid"],
                    BBOX[50.75,3.37,53.47,7.21]]]
            """;

        var pcs = _parser.ParseCoordinateSystem(wkt) as ProjectedCoordinateSystem;
        Assert.IsNotNull(pcs);

        var ctFactory = new CoordinateTransformationFactory();
        var toProj = ctFactory.CreateFromCoordinateSystems(
            GeographicCoordinateSystem.WGS84, pcs).MathTransform;

        // The projection's natural origin should map to (false_easting, false_northing)
        var (x, y) = toProj.Transform(5.3876, 52.1562);

        Assert.AreEqual(155000.0, x, 1.0, "Origin easting should equal false_easting");
        Assert.AreEqual(463000.0, y, 1.0, "Origin northing should equal false_northing");
    }

    // ── HRDPS (Rotated Lat/Lon) real-world WKT ────────────────────────

    private const string HrdpsWkt = """
        GEOGCRS["Rotated Lat/Lon",
            BASEGEOGCRS["GCS_Sphere",
                DATUM["D_Sphere",
                    ELLIPSOID["Sphere",6371229.0,0.0]]],
            DERIVINGCONVERSION["Rotated Lat/Lon",
                METHOD["PROJ ob_tran o_proj=longlat"],
                PARAMETER["o_lon_p",0],
                PARAMETER["o_lat_p",36.0885],
                PARAMETER["lon_0",245.305]]
            CS[ellipsoidal,2],
                AXIS["latitude",north],
                AXIS["longitude",east],
                ANGLEUNIT["degree",0.0174532925199433],
            USAGE[
                SCOPE["grid"],
                BBOX[39.626034,-133.62952,47.87646,-40.708527]]]
        """;

    [TestMethod]
    public void Hrdps_IsNotGaussianGrid()
    {
        Assert.IsFalse(_parser.IsGaussianGrid(HrdpsWkt));
    }

    [TestMethod]
    public void Hrdps_IsNotProjectedCrs()
    {
        Assert.IsFalse(_parser.IsProjectedCrs(HrdpsWkt));
    }

    [TestMethod]
    public void Hrdps_ParseCoordinateSystem_ReturnsSphereGeographicCrs()
    {
        var gcs = _parser.ParseCoordinateSystem(HrdpsWkt) as GeographicCoordinateSystem;

        Assert.IsNotNull(gcs);
        Assert.AreEqual(6371229.0, gcs.HorizontalDatum.Ellipsoid.SemiMajorAxis, 0.1);
        Assert.AreEqual(0.0, gcs.HorizontalDatum.Ellipsoid.InverseFlattening, 1e-10);
        Assert.AreEqual(
            gcs.HorizontalDatum.Ellipsoid.SemiMajorAxis,
            gcs.HorizontalDatum.Ellipsoid.SemiMinorAxis,
            0.1,
            "Sphere should have equal semi-major and semi-minor axes");
    }

    [TestMethod]
    public void Hrdps_CoordinateSystem_ContainsYVR()
    {
        var gcs = _parser.ParseCoordinateSystem(HrdpsWkt) as GeographicCoordinateSystem;
        Assert.IsNotNull(gcs, "HRDPS WKT should parse to a GeographicCoordinateSystem");

        var ctFactory = new CoordinateTransformationFactory();
        var toHrdps = ctFactory.CreateFromCoordinateSystems(
            GeographicCoordinateSystem.WGS84, gcs).MathTransform;

        // Vancouver International Airport (YVR): 49.1947°N, 123.1788°W
        const double yvrLon = -123.1788;
        const double yvrLat = 49.1947;
        var (lon, lat) = toHrdps.Transform(yvrLon, yvrLat);

        Assert.IsFalse(double.IsNaN(lat) || double.IsInfinity(lat),
            "YVR latitude should be a valid coordinate in the HRDPS CRS");
        Assert.IsFalse(double.IsNaN(lon) || double.IsInfinity(lon),
            "YVR longitude should be a valid coordinate in the HRDPS CRS");

        // Tolerance of 0.5° accounts for the geodetic-to-geocentric latitude shift
        // when transforming from WGS84 ellipsoid to sphere (~0.19° at mid-latitudes)
        Assert.AreEqual(yvrLat, lat, 0.5,
            "YVR latitude should be representable in the HRDPS sphere CRS");
        Assert.AreEqual(yvrLon, lon, 0.5,
            "YVR longitude should be representable in the HRDPS sphere CRS");
    }

    [TestMethod]
    public void Hrdps_CoordinateSystem_ContainsYUL()
    {
        var gcs = _parser.ParseCoordinateSystem(HrdpsWkt) as GeographicCoordinateSystem;
        Assert.IsNotNull(gcs, "HRDPS WKT should parse to a GeographicCoordinateSystem");

        var ctFactory = new CoordinateTransformationFactory();
        var toHrdps = ctFactory.CreateFromCoordinateSystems(
            GeographicCoordinateSystem.WGS84, gcs).MathTransform;

        // Montréal–Trudeau International Airport (YUL): 45.4706°N, 73.7408°W
        const double yulLon = -73.7408;
        const double yulLat = 45.4706;
        var (lon, lat) = toHrdps.Transform(yulLon, yulLat);

        Assert.IsFalse(double.IsNaN(lat) || double.IsInfinity(lat),
            "YUL latitude should be a valid coordinate in the HRDPS CRS");
        Assert.IsFalse(double.IsNaN(lon) || double.IsInfinity(lon),
            "YUL longitude should be a valid coordinate in the HRDPS CRS");

        // Tolerance of 0.5° accounts for the geodetic-to-geocentric latitude shift
        // when transforming from WGS84 ellipsoid to sphere (~0.19° at mid-latitudes)
        Assert.AreEqual(yulLat, lat, 0.5,
            "YUL latitude should be representable in the HRDPS sphere CRS");
        Assert.AreEqual(yulLon, lon, 0.5,
            "YUL longitude should be representable in the HRDPS sphere CRS");
    }

    [TestMethod]
    public void Hrdps_CoordinateSystem_DoesNotContainLondonUK()
    {
        var grid = new RegularGrid(_parser, HrdpsWkt, (100, 200));

        // London Heathrow (LHR): 51.4775°N, 0.4614°W
        var idx = grid.FindPointXY(51.4775, -0.4614);

        Assert.IsNull(idx,
            "London should fall outside the HRDPS grid domain");
    }
}
