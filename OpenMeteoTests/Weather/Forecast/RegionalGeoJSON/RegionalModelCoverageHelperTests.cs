using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Forecast.Options;
using OpenMeteo.Weather.Forecast.RegionalModelCoverage;

namespace OpenMeteoTests.Weather.Forecast.RegionalModelCoverage;

[TestClass]
public class RegionalModelCoverageHelperTests
{
    private readonly RegionalModelCoverageHelper _sut = new();

    [DataTestMethod]
    [DataRow(WeatherModelOptionsParameter.icon_d2, 52.52f, 13.41f)]                    // Berlin, Germany
    [DataRow(WeatherModelOptionsParameter.icon_eu, 48.85f, 2.35f)]                     // Paris, France
    [DataRow(WeatherModelOptionsParameter.gfs_hrrr, 39.73f, -104.99f)]                 // Denver, CO
    [DataRow(WeatherModelOptionsParameter.ncep_nam_conus, 39.73f, -104.99f)]           // Denver, CO
    [DataRow(WeatherModelOptionsParameter.ncep_nbm_conus, 39.73f, -104.99f)]           // Denver, CO
    [DataRow(WeatherModelOptionsParameter.meteofrance_arome_france, 48.85f, 2.35f)]    // Paris, France
    [DataRow(WeatherModelOptionsParameter.meteofrance_arome_france_hd, 48.85f, 2.35f)] // Paris, France
    [DataRow(WeatherModelOptionsParameter.meteofrance_arpege_europe, 48.85f, 2.35f)]   // Paris, France
    [DataRow(WeatherModelOptionsParameter.metno_nordic, 59.91f, 10.75f)]               // Oslo, Norway
    [DataRow(WeatherModelOptionsParameter.jma_msm, 35.69f, 139.69f)]                   // Tokyo, Japan
    [DataRow(WeatherModelOptionsParameter.meteoswiss_icon_ch1, 47.38f, 8.54f)]         // Zurich, Switzerland
    [DataRow(WeatherModelOptionsParameter.meteoswiss_icon_ch2, 47.38f, 8.54f)]         // Zurich, Switzerland
    [DataRow(WeatherModelOptionsParameter.ukmo_uk_deterministic_2km, 51.51f, -0.13f)]  // London, UK
    [DataRow(WeatherModelOptionsParameter.gem_hrdps_continental, 45.42f, -75.69f)]     // Ottawa, Canada
    [DataRow(WeatherModelOptionsParameter.gem_regional, 45.42f, -75.69f)]              // Ottawa, Canada
    [DataRow(WeatherModelOptionsParameter.italia_meteo_arpae_icon_2i, 41.90f, 12.50f)] // Rome, Italy
    public void IsLocationInModelCoverage_PointInsideCoverage_ReturnsTrue(
        WeatherModelOptionsParameter model, float latitude, float longitude)
    {
        bool? result = _sut.IsLocationInModelCoverage(model, latitude, longitude);

        Assert.AreEqual(true, result);
    }

    [DataTestMethod]
    [DataRow(WeatherModelOptionsParameter.icon_d2, 40.71f, -74.01f)]                    // New York, USA
    [DataRow(WeatherModelOptionsParameter.icon_eu, 35.69f, 139.69f)]                    // Tokyo, Japan
    [DataRow(WeatherModelOptionsParameter.gfs_hrrr, 51.51f, -0.13f)]                    // London, UK
    [DataRow(WeatherModelOptionsParameter.ncep_nam_conus, 51.51f, -0.13f)]              // London, UK
    [DataRow(WeatherModelOptionsParameter.ncep_nbm_conus, 51.51f, -0.13f)]              // London, UK
    [DataRow(WeatherModelOptionsParameter.meteofrance_arome_france, 55.75f, 37.62f)]    // Moscow, Russia
    [DataRow(WeatherModelOptionsParameter.meteofrance_arome_france_hd, 55.75f, 37.62f)] // Moscow, Russia
    [DataRow(WeatherModelOptionsParameter.meteofrance_arpege_europe, 35.69f, 139.69f)]  // Tokyo, Japan
    [DataRow(WeatherModelOptionsParameter.metno_nordic, 41.90f, 12.50f)]                // Rome, Italy
    [DataRow(WeatherModelOptionsParameter.jma_msm, 51.51f, -0.13f)]                     // London, UK
    [DataRow(WeatherModelOptionsParameter.meteoswiss_icon_ch1, 52.52f, 13.41f)]         // Berlin, Germany
    [DataRow(WeatherModelOptionsParameter.meteoswiss_icon_ch2, 52.52f, 13.41f)]         // Berlin, Germany
    [DataRow(WeatherModelOptionsParameter.ukmo_uk_deterministic_2km, 40.71f, -74.01f)]  // New York, USA
    [DataRow(WeatherModelOptionsParameter.gem_hrdps_continental, 51.51f, -0.13f)]       // London, UK
    [DataRow(WeatherModelOptionsParameter.gem_regional, 51.51f, -0.13f)]                // London, UK
    [DataRow(WeatherModelOptionsParameter.italia_meteo_arpae_icon_2i, 52.52f, 13.41f)]  // Berlin, Germany
    public void IsLocationInModelCoverage_PointOutsideCoverage_ReturnsFalse(
        WeatherModelOptionsParameter model, float latitude, float longitude)
    {
        bool? result = _sut.IsLocationInModelCoverage(model, latitude, longitude);

        Assert.AreEqual(false, result);
    }

    [DataTestMethod]
    [DataRow(WeatherModelOptionsParameter.icon_global)]
    [DataRow(WeatherModelOptionsParameter.gfs_global)]
    [DataRow(WeatherModelOptionsParameter.ecmwf_ifs)]
    [DataRow(WeatherModelOptionsParameter.gem_global)]
    [DataRow(WeatherModelOptionsParameter.jma_gsm)]
    [DataRow(WeatherModelOptionsParameter.bom_access_global)]
    [DataRow(WeatherModelOptionsParameter.ukmo_global_deterministic_10km)]
    public void IsLocationInModelCoverage_GlobalModelWithNoGeoJsonFile_ReturnsNull(
        WeatherModelOptionsParameter model)
    {
        bool? result = _sut.IsLocationInModelCoverage(model, 51.51f, -0.13f);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void IsLocationInModelCoverage_UnmappedModel_ReturnsNull()
    {
        bool? result = _sut.IsLocationInModelCoverage((WeatherModelOptionsParameter)int.MaxValue, 51.51f, -0.13f);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void IsLocationInModelCoverage_CalledTwice_ReturnsSameResult()
    {
        bool? firstResult = _sut.IsLocationInModelCoverage(WeatherModelOptionsParameter.icon_d2, 52.52f, 13.41f);
        bool? secondResult = _sut.IsLocationInModelCoverage(WeatherModelOptionsParameter.icon_d2, 52.52f, 13.41f);

        Assert.AreEqual(firstResult, secondResult);
    }
}
