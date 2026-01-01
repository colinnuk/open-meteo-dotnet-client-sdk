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
    [DataRow(EnsembleModelOptionsParameter.icon_global)]
    [DataRow(EnsembleModelOptionsParameter.gfs025)]
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs025)]
    [DataRow(EnsembleModelOptionsParameter.gem_global)]
    [DataRow(EnsembleModelOptionsParameter.ukmo_global_ensemble_20km)]
    [DataRow(EnsembleModelOptionsParameter.meteoswiss_icon_ch1)]
    public async Task EnsembleMetadata_Async_Test(EnsembleModelOptionsParameter model)
    {
        OpenMeteoClient client = new();
        var res = await client.QueryWeatherEnsembleMetadata(model);

        Assert.IsNotNull(res);
        Assert.IsTrue(res.TemporalResolutionSeconds > 0);
        Assert.IsTrue(res.UpdateIntervalSeconds > 0);
    }
}
