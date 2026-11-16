using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;
using OpenMeteo.Weather.Seasonal.Options;
using System.Threading.Tasks;

namespace OpenMeteoTests.Weather.Seasonal;

[TestClass]
[TestCategory(TestCategoryConstants.Integration)]
public class SeasonalMetadataTests
{
    [DataTestMethod]
    [DataRow(SeasonalModelOptionsParameter.ecmwf_seas5)]
    [DataRow(SeasonalModelOptionsParameter.ecmwf_seas5_ensemble_mean)]
    [DataRow(SeasonalModelOptionsParameter.ecmwf_ec46)]
    [DataRow(SeasonalModelOptionsParameter.ecmwf_ec46_ensemble_mean)]
    public async Task SeasonalMetadata_Async_Test(SeasonalModelOptionsParameter model)
    {
        OpenMeteoClient client = new();
        var res = await client.QueryWeatherSeasonalMetadata(model);

        Assert.IsNotNull(res);
        Assert.IsTrue(res.TemporalResolutionSeconds > 0);
        Assert.IsTrue(res.UpdateIntervalSeconds > 0);
        Assert.IsFalse(string.IsNullOrEmpty(res.CrsWkt));
    }
}
