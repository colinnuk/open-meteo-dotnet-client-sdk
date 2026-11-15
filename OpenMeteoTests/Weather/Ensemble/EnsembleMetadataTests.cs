using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;
using OpenMeteo.Weather.Ensemble.Options;
using System.Threading.Tasks;

namespace OpenMeteoTests.Weather.Ensemble;

[TestClass]
[TestCategory(TestCategoryConstants.Integration)]
public class EnsembleMetadataTests
{
    [DataTestMethod]
    [DataRow(EnsembleModelOptionsParameter.icon_seamless)]
    [DataRow(EnsembleModelOptionsParameter.icon_global)]
    [DataRow(EnsembleModelOptionsParameter.icon_eu)]
    [DataRow(EnsembleModelOptionsParameter.icon_d2)]
    [DataRow(EnsembleModelOptionsParameter.gfs_seamless)]
    [DataRow(EnsembleModelOptionsParameter.gfs025)]
    [DataRow(EnsembleModelOptionsParameter.gfs05)]
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs025)]
    [DataRow(EnsembleModelOptionsParameter.ecmwf_aifs025)]
    [DataRow(EnsembleModelOptionsParameter.gem_global)]
    [DataRow(EnsembleModelOptionsParameter.ukmo_global_ensemble_20km)]
    [DataRow(EnsembleModelOptionsParameter.ukmo_uk_ensemble_2km)]
    [DataRow(EnsembleModelOptionsParameter.meteoswiss_icon_ch1)]
    [DataRow(EnsembleModelOptionsParameter.meteoswiss_icon_ch2)]
    [DataRow(EnsembleModelOptionsParameter.ncep_aigefs025)]
    [DataRow(EnsembleModelOptionsParameter.google_weathernext2_ensemble)]
    public async Task EnsembleMetadata_Async_Test(EnsembleModelOptionsParameter model)
    {
        OpenMeteoClient client = new();
        var res = await client.QueryWeatherEnsembleMetadata(model);

        Assert.IsNotNull(res);
        Assert.IsTrue(res.TemporalResolutionSeconds > 0);
        Assert.IsTrue(res.UpdateIntervalSeconds > 0);
        Assert.IsFalse(string.IsNullOrEmpty(res.CrsWkt));
    }
}
