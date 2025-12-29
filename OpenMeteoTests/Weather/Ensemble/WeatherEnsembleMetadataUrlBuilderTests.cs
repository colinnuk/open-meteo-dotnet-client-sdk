using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Ensemble.Metadata;

namespace OpenMeteoTests.Weather.Ensemble;

[TestClass]
public class WeatherEnsembleMetadataUrlBuilderTests
{
    [TestMethod]
    public void Build_DefaultConstructor_WithModel_GemGlobal_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder()
            .WithModel(EnsembleModelOptionsParameter.gem_global)
            .Build();
        var expectedUrl = "https://ensemble-api.open-meteo.com/data/cmc_gem_geps/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_DefaultConstructor_WithModel_IconSeamless_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder()
            .WithModel(EnsembleModelOptionsParameter.icon_seamless)
            .Build();
        var expectedUrl = "https://ensemble-api.open-meteo.com/data/dwd_icon_eps/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_DefaultConstructor_WithModel_GfsSeamless_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder()
            .WithModel(EnsembleModelOptionsParameter.gfs_seamless)
            .Build();
        var expectedUrl = "https://ensemble-api.open-meteo.com/data/ncep_gefs05/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_DefaultConstructor_WithModel_EcmwfIfs025_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder()
            .WithModel(EnsembleModelOptionsParameter.ecmwf_ifs025)
            .Build();
        var expectedUrl = "https://ensemble-api.open-meteo.com/data/ecmwf_ifs025_ensemble/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_CustomBaseUri_WithModel_Test()
    {
        var customUri = new Uri("https://custom.example.com");
        var url = new WeatherEnsembleMetadataUrlBuilder(customUri)
            .WithModel(EnsembleModelOptionsParameter.gem_global)
            .Build();
        var expectedUrl = "https://custom.example.com/data/cmc_gem_geps/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_ApiKey_WithModel_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder("testApiKey")
            .WithModel(EnsembleModelOptionsParameter.gfs_seamless)
            .Build();
        var expectedUrl = "https://customer-ensemble-api.open-meteo.com/data/ncep_gefs05/static/meta.json?apikey=testApiKey";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_CustomBaseUri_ApiKey_WithModel_Test()
    {
        var customUri = new Uri("https://custom.example.com");
        var url = new WeatherEnsembleMetadataUrlBuilder(customUri, "testApiKey")
            .WithModel(EnsembleModelOptionsParameter.icon_global)
            .Build();
        var expectedUrl = "https://custom.example.com/data/dwd_icon_eps/static/meta.json?apikey=testApiKey";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_DefaultConstructor_WithModel_BomAccess_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder()
            .WithModel(EnsembleModelOptionsParameter.bom_access_global_ensemble)
            .Build();
        var expectedUrl = "https://ensemble-api.open-meteo.com/data/bom_access_global_ensemble/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_DefaultConstructor_WithModel_UkmoGlobal_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder()
            .WithModel(EnsembleModelOptionsParameter.ukmo_global_ensemble_20km)
            .Build();
        var expectedUrl = "https://ensemble-api.open-meteo.com/data/ukmo_global_ensemble_20km/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_DefaultConstructor_WithModel_MeteoSwissIconCh1_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder()
            .WithModel(EnsembleModelOptionsParameter.meteoswiss_icon_ch1)
            .Build();
        var expectedUrl = "https://ensemble-api.open-meteo.com/data/meteoswiss_icon_ch1_ensemble/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_DefaultConstructor_WithModel_Gfs025_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder()
            .WithModel(EnsembleModelOptionsParameter.gfs025)
            .Build();
        var expectedUrl = "https://ensemble-api.open-meteo.com/data/ncep_gefs025/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_DefaultConstructor_WithModel_IconEu_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder()
            .WithModel(EnsembleModelOptionsParameter.icon_eu)
            .Build();
        var expectedUrl = "https://ensemble-api.open-meteo.com/data/dwd_icon_eu_eps/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_DefaultConstructor_WithModel_IconD2_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder()
            .WithModel(EnsembleModelOptionsParameter.icon_d2)
            .Build();
        var expectedUrl = "https://ensemble-api.open-meteo.com/data/dwd_icon_d2_eps/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_DefaultConstructor_WithModel_EcmwfAifs025_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder()
            .WithModel(EnsembleModelOptionsParameter.ecmwf_aifs025)
            .Build();
        var expectedUrl = "https://ensemble-api.open-meteo.com/data/ecmwf_aifs025_ensemble/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_DefaultConstructor_WithModel_Gfs05_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder()
            .WithModel(EnsembleModelOptionsParameter.gfs05)
            .Build();
        var expectedUrl = "https://ensemble-api.open-meteo.com/data/ncep_gefs05/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_DefaultConstructor_WithModel_UkmoUk_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder()
            .WithModel(EnsembleModelOptionsParameter.ukmo_uk_ensemble_2km)
            .Build();
        var expectedUrl = "https://ensemble-api.open-meteo.com/data/ukmo_uk_ensemble_2km/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }

    [TestMethod]
    public void Build_DefaultConstructor_WithModel_MeteoSwissIconCh2_Test()
    {
        var url = new WeatherEnsembleMetadataUrlBuilder()
            .WithModel(EnsembleModelOptionsParameter.meteoswiss_icon_ch2)
            .Build();
        var expectedUrl = "https://ensemble-api.open-meteo.com/data/meteoswiss_icon_ch2_ensemble/static/meta.json";
        Assert.AreEqual(expectedUrl, url);
    }
}
