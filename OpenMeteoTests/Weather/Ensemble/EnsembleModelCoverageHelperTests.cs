using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Ensemble.RegionalModelCoverage;

namespace OpenMeteoTests.Weather.Ensemble;

[TestClass]
public class EnsembleModelCoverageHelperTests
{
    private readonly EnsembleModelCoverageHelper _sut = new();

    [DataTestMethod]
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs_europe_ensemble, 48.85f, 2.35f)]   // Paris, France
    [DataRow(EnsembleModelOptionsParameter.ecmwf_aifs_europe_ensemble, 48.85f, 2.35f)]  // Paris, France
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs_europe_ensemble, 33.2f, 0f)]       // Just inside the southern edge
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs_europe_ensemble, 70.8f, 10f)]      // Just inside the northern edge
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs_europe_ensemble, 50f, -10.8f)]     // Just inside the western edge
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs_europe_ensemble, 50f, 36.8f)]      // Just inside the eastern edge
    [DataRow(EnsembleModelOptionsParameter.ukmo_uk_ensemble_2km, 51.51f, -0.13f)]       // London, UK
    [DataRow(EnsembleModelOptionsParameter.meteoswiss_icon_ch1, 47.38f, 8.54f)]         // Zurich, Switzerland
    [DataRow(EnsembleModelOptionsParameter.meteoswiss_icon_ch2, 47.38f, 8.54f)]         // Zurich, Switzerland
    [DataRow(EnsembleModelOptionsParameter.ncep_rrfs_conus_ensemble, 39.73f, -104.99f)] // Denver, CO
    public void IsLocationInModelCoverage_PointInsideCoverage_ReturnsTrue(
        EnsembleModelOptionsParameter model, float latitude, float longitude)
    {
        Assert.AreEqual(true, _sut.IsLocationInModelCoverage(model, latitude, longitude));
    }

    [DataTestMethod]
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs_europe_ensemble, 40.71f, -74.01f)] // New York, USA
    [DataRow(EnsembleModelOptionsParameter.ecmwf_aifs_europe_ensemble, 35.69f, 139.69f)] // Tokyo, Japan
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs_europe_ensemble, 32.7f, 0f)]       // Just outside the southern edge
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs_europe_ensemble, 71.3f, 10f)]      // Just outside the northern edge
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs_europe_ensemble, 50f, -11.5f)]     // Just outside the western edge
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs_europe_ensemble, 50f, 37.5f)]      // Just outside the eastern edge
    [DataRow(EnsembleModelOptionsParameter.ukmo_uk_ensemble_2km, 40.71f, -74.01f)]      // New York, USA
    [DataRow(EnsembleModelOptionsParameter.meteoswiss_icon_ch1, 52.52f, 13.41f)]        // Berlin, Germany
    [DataRow(EnsembleModelOptionsParameter.meteoswiss_icon_ch2, 52.52f, 13.41f)]        // Berlin, Germany
    [DataRow(EnsembleModelOptionsParameter.ncep_rrfs_conus_ensemble, 51.51f, -0.13f)]   // London, UK
    public void IsLocationInModelCoverage_PointOutsideCoverage_ReturnsFalse(
        EnsembleModelOptionsParameter model, float latitude, float longitude)
    {
        Assert.AreEqual(false, _sut.IsLocationInModelCoverage(model, latitude, longitude));
    }

    [DataTestMethod]
    [DataRow(EnsembleModelOptionsParameter.icon_seamless)]
    [DataRow(EnsembleModelOptionsParameter.icon_global)]
    [DataRow(EnsembleModelOptionsParameter.gfs_seamless)]
    [DataRow(EnsembleModelOptionsParameter.gfs025)]
    [DataRow(EnsembleModelOptionsParameter.gfs05)]
    [DataRow(EnsembleModelOptionsParameter.ncep_aigefs025)]
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs025)]
    [DataRow(EnsembleModelOptionsParameter.ecmwf_aifs025)]
    [DataRow(EnsembleModelOptionsParameter.gem_global)]
    [DataRow(EnsembleModelOptionsParameter.ukmo_global_ensemble_20km)]
    [DataRow(EnsembleModelOptionsParameter.google_weathernext2_ensemble)]
    public void IsLocationInModelCoverage_GlobalModel_ReturnsNull(EnsembleModelOptionsParameter model)
    {
        Assert.IsNull(_sut.IsLocationInModelCoverage(model, 51.51f, -0.13f));
    }
}
