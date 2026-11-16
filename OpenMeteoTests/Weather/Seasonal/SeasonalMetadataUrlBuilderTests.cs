using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Seasonal.Metadata;
using OpenMeteo.Weather.Seasonal.Options;

namespace OpenMeteoTests.Weather.Seasonal;

[TestClass]
public class SeasonalMetadataUrlBuilderTests
{
    [DataTestMethod]
    [DataRow(SeasonalModelOptionsParameter.ecmwf_seas5, "ecmwf_seas5")]
    [DataRow(SeasonalModelOptionsParameter.ecmwf_seas5_ensemble_mean, "ecmwf_seas5_ensemble_mean")]
    [DataRow(SeasonalModelOptionsParameter.ecmwf_ec46, "ecmwf_ec46")]
    [DataRow(SeasonalModelOptionsParameter.ecmwf_ec46_ensemble_mean, "ecmwf_ec46_ensemble_mean")]
    public void Build_DefaultConstructor_WithModel_Test(SeasonalModelOptionsParameter model, string name)
    {
        var url = new SeasonalMetadataUrlBuilder().WithModel(model).Build();
        Assert.AreEqual($"https://seasonal-api.open-meteo.com/data/{name}/static/meta.json", url);
    }

    [TestMethod]
    public void WithModel_SeamlessModelWithoutMetadata_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            new SeasonalMetadataUrlBuilder().WithModel(SeasonalModelOptionsParameter.ecmwf_seasonal_seamless));
    }

    [TestMethod]
    public void Build_CustomBaseUri_WithModel_Test()
    {
        var url = new SeasonalMetadataUrlBuilder(new Uri("https://custom.example.com"))
            .WithModel(SeasonalModelOptionsParameter.ecmwf_seas5)
            .Build();
        Assert.AreEqual("https://custom.example.com/data/ecmwf_seas5/static/meta.json", url);
    }

    [TestMethod]
    public void Build_ApiKey_WithModel_Test()
    {
        var url = new SeasonalMetadataUrlBuilder("testApiKey")
            .WithModel(SeasonalModelOptionsParameter.ecmwf_ec46)
            .Build();
        Assert.AreEqual("https://customer-seasonal-api.open-meteo.com/data/ecmwf_ec46/static/meta.json?apikey=testApiKey", url);
    }

    [TestMethod]
    public void Build_CustomBaseUri_ApiKey_WithModel_Test()
    {
        var url = new SeasonalMetadataUrlBuilder(new Uri("https://custom.example.com"), "testApiKey")
            .WithModel(SeasonalModelOptionsParameter.ecmwf_ec46)
            .Build();
        Assert.AreEqual("https://custom.example.com/data/ecmwf_ec46/static/meta.json?apikey=testApiKey", url);
    }
}
