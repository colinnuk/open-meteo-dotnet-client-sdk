using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;
using OpenMeteo.Weather.Ensemble.Options;
using System.Text.Json;

namespace OpenMeteoTests.Weather.Ensemble;

[TestClass]
[TestCategory(TestCategoryConstants.Integration)]
public class EnsembleApiIntegrationTests
{
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public async Task QueryEnsembleApiAsync_WithLatitudeLongitude_ReturnsEnsembleData()
    {
        var client = new OpenMeteoClient();
        
        var result = await client.QueryEnsembleApiAsync(50.0769f, -122.948f);
        
        Assert.IsNotNull(result);
        Assert.AreEqual(50.0769f, result.Latitude, 0.1f);
        Assert.AreEqual(-122.948f, result.Longitude, 0.1f);
    }

    [DataTestMethod]
    [DataRow(EnsembleModelOptionsParameter.icon_seamless, 48.8566, 2.3522)] // Paris
    [DataRow(EnsembleModelOptionsParameter.icon_global, 48.8566, 2.3522)] // Paris
    [DataRow(EnsembleModelOptionsParameter.icon_eu, 52.52, 13.405)] // Berlin
    [DataRow(EnsembleModelOptionsParameter.icon_d2, 51.5074, -0.1278)] // London
    [DataRow(EnsembleModelOptionsParameter.gfs_seamless, 40.7128, -74.0060)] // New York
    [DataRow(EnsembleModelOptionsParameter.gfs025, 40.7128, -74.0060)] // New York
    [DataRow(EnsembleModelOptionsParameter.gfs05, 40.7128, -74.0060)] // New York
    [DataRow(EnsembleModelOptionsParameter.ncep_aigefs025, 40.7128, -74.0060)] // New York
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs025, 48.8566, 2.3522)] // Paris
    [DataRow(EnsembleModelOptionsParameter.ecmwf_aifs025, 48.8566, 2.3522)] // Paris
    [DataRow(EnsembleModelOptionsParameter.gem_global, 45.4215, -75.6997)] // Ottawa
    [DataRow(EnsembleModelOptionsParameter.bom_access_global_ensemble, -33.8688, 151.2093)] // Sydney
    [DataRow(EnsembleModelOptionsParameter.ukmo_global_ensemble_20km, 51.5074, -0.1278)] // London
    [DataRow(EnsembleModelOptionsParameter.ukmo_uk_ensemble_2km, 53.4808, -2.2426)] // Manchester
    [DataRow(EnsembleModelOptionsParameter.meteoswiss_icon_ch1, 47.3769, 8.5417)] // Zurich
    [DataRow(EnsembleModelOptionsParameter.meteoswiss_icon_ch2, 46.948, 7.4474)] // Bern
    public async Task QueryEnsembleApiAsync_WithOptions_ReturnsEnsembleData(EnsembleModelOptionsParameter model, double lat, double lon)
    {
        var client = new OpenMeteoClient();
        var options = new WeatherEnsembleOptions
        {
            Latitude = (float)lat,
            Longitude = (float)lon,
            Hourly = new WeatherEnsembleHourlyOptions([
                WeatherEnsembleHourlyOptionsParameter.temperature_2m,
                WeatherEnsembleHourlyOptionsParameter.precipitation
            ]),
            Models = new EnsembleModelOptions(model)
        };
        
        var result = await client.QueryEnsembleApiAsync(options);
        
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Hourly);
        Assert.IsNotNull(result.Hourly.Time);
        Assert.IsTrue(result.Hourly.Time.Length > 0);
        Assert.IsTrue(result.Hourly.Temperature_2m!.Count > 0);
        Assert.IsTrue(result.Hourly.Precipitation!.Count > 0);


        // Write the result to the test log as JSON
        if (TestContext != null)
        {
            var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            TestContext.WriteLine(json);
        }
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
