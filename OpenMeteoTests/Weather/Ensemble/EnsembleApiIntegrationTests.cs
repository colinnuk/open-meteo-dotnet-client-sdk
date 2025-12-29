using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;
using OpenMeteo.Weather.Ensemble.Options;

namespace OpenMeteoTests.Weather.Ensemble;

[TestClass]
[TestCategory(TestCategoryConstants.Integration)]
public class EnsembleApiIntegrationTests
{
    [TestMethod]
    public async Task QueryEnsembleApiAsync_WithLatitudeLongitude_ReturnsEnsembleData()
    {
        var client = new OpenMeteoClient();
        
        var result = await client.QueryEnsembleApiAsync(50.0769f, -122.948f);
        
        Assert.IsNotNull(result);
        Assert.AreEqual(50.0769f, result.Latitude, 0.1f);
        Assert.AreEqual(-122.948f, result.Longitude, 0.1f);
    }

    [TestMethod]
    public async Task QueryEnsembleApiAsync_WithOptions_ReturnsEnsembleData()
    {
        var client = new OpenMeteoClient();
        var options = new WeatherEnsembleOptions
        {
            Latitude = 50.0769f,
            Longitude = -122.948f,
            Hourly = new WeatherEnsembleHourlyOptions([
                WeatherEnsembleHourlyOptionsParameter.temperature_2m,
                WeatherEnsembleHourlyOptionsParameter.precipitation
            ]),
            Models = new EnsembleModelOptions(EnsembleModelOptionsParameter.ukmo_global_ensemble_20km)
        };
        
        var result = await client.QueryEnsembleApiAsync(options);
        
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Hourly);
        Assert.IsNotNull(result.Hourly.Time);
        Assert.IsTrue(result.Hourly.Time.Length > 0);
    }

    [TestMethod]
    public async Task QueryEnsembleApiAsync_WithLocation_ReturnsEnsembleData()
    {
        var client = new OpenMeteoClient();
        var options = new WeatherEnsembleOptions
        {
            Hourly = new WeatherEnsembleHourlyOptions([
                WeatherEnsembleHourlyOptionsParameter.temperature_2m
            ])
        };
        
        var result = await client.QueryEnsembleApiAsync("Whistler", options);
        
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Hourly);
        Assert.IsTrue(result.Latitude > 0);
        Assert.IsTrue(result.Longitude < 0);
    }

    [TestMethod]
    public async Task QueryEnsembleApiAsync_WithFlatbuffers_ReturnsEnsembleData()
    {
        var client = new OpenMeteoClient { UseFlatbuffers = true };
        var options = new WeatherEnsembleOptions
        {
            Latitude = 50.0769f,
            Longitude = -122.948f,
            Hourly = new WeatherEnsembleHourlyOptions([
                WeatherEnsembleHourlyOptionsParameter.temperature_2m,
                WeatherEnsembleHourlyOptionsParameter.precipitation
            ])
        };
        
        var result = await client.QueryEnsembleApiAsync(options);
        
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Hourly);
        Assert.IsNotNull(result.Hourly.Time);
        Assert.IsTrue(result.Hourly.Time.Length > 0);
    }

    [TestMethod]
    public async Task QueryEnsembleApiAsync_WithInvalidOptions_ReturnsNull()
    {
        var client = new OpenMeteoClient();
        var options = new WeatherEnsembleOptions
        {
            Latitude = 1000f, // Invalid latitude
            Longitude = 1000f  // Invalid longitude
        };
        
        var result = await client.QueryEnsembleApiAsync(options);
        
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task QueryEnsembleApiAsync_WithRethrowExceptions_ThrowsException()
    {
        var client = new OpenMeteoClient { RethrowExceptions = true };
        var options = new WeatherEnsembleOptions
        {
            Latitude = 1000f, // Invalid latitude
            Longitude = 1000f  // Invalid longitude
        };
        
        await Assert.ThrowsExceptionAsync<OpenMeteoClientException>(
            async () => await client.QueryEnsembleApiAsync(options));
    }
}
