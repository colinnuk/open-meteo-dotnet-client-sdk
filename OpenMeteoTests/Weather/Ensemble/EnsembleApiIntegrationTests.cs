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
    [DataRow(EnsembleModelOptionsParameter.ukmo_global_ensemble_20km, 51.5074, -0.1278)] // London
    [DataRow(EnsembleModelOptionsParameter.ukmo_uk_ensemble_2km, 53.4808, -2.2426)] // Manchester
    [DataRow(EnsembleModelOptionsParameter.meteoswiss_icon_ch1, 47.3769, 8.5417)] // Zurich
    [DataRow(EnsembleModelOptionsParameter.meteoswiss_icon_ch2, 46.948, 7.4474)] // Bern
    [DataRow(EnsembleModelOptionsParameter.ncep_rrfs_conus_ensemble, 39.7392, -104.9903)] // Denver
    [DataRow(EnsembleModelOptionsParameter.ecmwf_ifs_europe_ensemble, 48.8566, 2.3522)] // Paris
    [DataRow(EnsembleModelOptionsParameter.ecmwf_aifs_europe_ensemble, 48.8566, 2.3522)] // Paris
    [DataRow(EnsembleModelOptionsParameter.google_weathernext2_ensemble, 51.5074, -0.1278)] // London
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

    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public async Task QueryEnsembleApiAsync_AllDailyVariables_AreAcceptedByApi(bool useFlatbuffers)
    {
        // Fails if any daily option name is not one the API accepts.
        // Every hourly variable at once would use up the free API's per-minute allowance, so only a few are requested here.
        var client = new OpenMeteoClient { RethrowExceptions = true, UseFlatbuffers = useFlatbuffers };
        var options = new WeatherEnsembleOptions(47.3769f, 8.5417f) // Zurich
        {
            Hourly = [
                WeatherEnsembleHourlyOptionsParameter.temperature_2m,
                WeatherEnsembleHourlyOptionsParameter.is_day,
                WeatherEnsembleHourlyOptionsParameter.soil_moisture_100_to_200cm
            ],
            Daily = WeatherEnsembleDailyOptions.All,
            Models = new EnsembleModelOptions(EnsembleModelOptionsParameter.gfs025),
            Forecast_Days = 1
        };

        var result = await client.QueryEnsembleApiAsync(options);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Hourly!.Temperature_2m!.Count > 1);
        Assert.IsTrue(result.Daily!.Temperature_2m_max!.Count > 1);
        Assert.IsTrue(result.Daily.Windspeed_100m_max!.Count > 1);
        Assert.IsTrue(result.Daily.Relativehumidity_2m_mean!.Count > 1);
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
            Longitude = 1000f, // Invalid longitude
            Models = new EnsembleModelOptions(EnsembleModelOptionsParameter.gfs025)
        };
        
        await Assert.ThrowsExactlyAsync<OpenMeteoClientException>(
            async () => await client.QueryEnsembleApiAsync(options));
    }
}
