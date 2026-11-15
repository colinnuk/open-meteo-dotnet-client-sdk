using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenMeteo;
using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Ensemble.ResponseModel;

namespace OpenMeteoTests.Weather.Ensemble;

[TestClass]
public class WeatherEnsembleMultiModelTests
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    // Shape of a real response for models=gfs025,icon_seamless: the API adds its own model name to every key,
    // which is not always the requested name (gfs025 -> ncep_gefs025, icon_seamless -> icon_seamless_eps)
    private const string MultiModelJson = @"{
        ""latitude"": 47.5, ""longitude"": 8.5, ""generationtime_ms"": 0.9, ""utc_offset_seconds"": 0,
        ""timezone"": ""GMT"", ""timezone_abbreviation"": ""GMT"", ""elevation"": 461.0,
        ""hourly_units"": {
            ""time"": ""iso8601"",
            ""temperature_2m_ncep_gefs025"": ""°C"", ""temperature_2m_member01_ncep_gefs025"": ""°C"",
            ""weathercode_ncep_gefs025"": ""wmo code"",
            ""temperature_2m_icon_seamless_eps"": ""°C"", ""temperature_2m_member01_icon_seamless_eps"": ""°C"",
            ""temperature_2m_member02_icon_seamless_eps"": ""°C"",
            ""weathercode_icon_seamless_eps"": ""wmo code""
        },
        ""hourly"": {
            ""time"": [""2026-11-15T00:00"", ""2026-11-15T01:00""],
            ""temperature_2m_ncep_gefs025"": [1.0, 1.1], ""temperature_2m_member01_ncep_gefs025"": [1.2, 1.3],
            ""weathercode_ncep_gefs025"": [3, 61],
            ""temperature_2m_icon_seamless_eps"": [2.0, 2.1], ""temperature_2m_member01_icon_seamless_eps"": [2.2, 2.3],
            ""temperature_2m_member02_icon_seamless_eps"": [2.4, 2.5],
            ""weathercode_icon_seamless_eps"": [0, 1]
        },
        ""daily_units"": {
            ""time"": ""iso8601"",
            ""temperature_2m_max_ncep_gefs025"": ""°C"", ""temperature_2m_max_icon_seamless_eps"": ""°C""
        },
        ""daily"": {
            ""time"": [""2026-11-15""],
            ""temperature_2m_max_ncep_gefs025"": [5.0], ""temperature_2m_max_member01_ncep_gefs025"": [5.5],
            ""temperature_2m_max_icon_seamless_eps"": [6.0]
        }
    }";

    private static WeatherEnsembleOptions MultiModelOptions() => new()
    {
        Hourly = [WeatherEnsembleHourlyOptionsParameter.temperature_2m, WeatherEnsembleHourlyOptionsParameter.weathercode],
        Daily = [WeatherEnsembleDailyOptionsParameter.temperature_2m_max],
        Models = [EnsembleModelOptionsParameter.gfs025, EnsembleModelOptionsParameter.icon_seamless]
    };

    private static HttpResponseMessage JsonResponse(string json) => new(HttpStatusCode.OK) { Content = new StringContent(json) };

    [TestMethod]
    public async Task DeserializeJsonListAsync_MultipleModels_SplitsByModelInRequestOrder()
    {
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);

        var ensembles = await parser.DeserializeJsonListAsync(JsonResponse(MultiModelJson), MultiModelOptions());

        Assert.IsNotNull(ensembles);
        Assert.AreEqual(2, ensembles.Count);

        var gfs = ensembles[0];
        Assert.AreEqual(EnsembleModelOptionsParameter.gfs025, gfs.Model);
        Assert.AreEqual(47.5f, gfs.Latitude);
        Assert.AreEqual(2, gfs.Hourly!.Time!.Length);
        Assert.AreEqual(2, gfs.Hourly.Temperature_2m!.Count);
        Assert.AreEqual(1.0f, gfs.Hourly.Temperature_2m[0]![0]);
        Assert.AreEqual(1.3f, gfs.Hourly.Temperature_2m[1]![1]);
        Assert.AreEqual(61, gfs.Hourly.Weathercode![0]![1]);
        Assert.AreEqual(2, gfs.Daily!.Temperature_2m_max!.Count);
        Assert.AreEqual(5.5f, gfs.Daily.Temperature_2m_max[1]![0]);
        Assert.AreEqual("°C", gfs.HourlyUnits!.AdditionalData!["temperature_2m"].ToString());

        var icon = ensembles[1];
        Assert.AreEqual(EnsembleModelOptionsParameter.icon_seamless, icon.Model);
        Assert.AreEqual(3, icon.Hourly!.Temperature_2m!.Count);
        Assert.AreEqual(2.5f, icon.Hourly.Temperature_2m[2]![1]);
        Assert.AreEqual(1, icon.Hourly.Weathercode![0]![1]);
        Assert.AreEqual(1, icon.Daily!.Temperature_2m_max!.Count);
        Assert.AreEqual(6.0f, icon.Daily.Temperature_2m_max[0]![0]);
        Assert.IsTrue(icon.DailyUnits!.AdditionalData!.ContainsKey("temperature_2m_max"));
    }

    [TestMethod]
    public async Task DeserializeJsonAsync_MultipleModels_ReturnsFirstModel()
    {
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);

        var ensemble = await parser.DeserializeJsonAsync(JsonResponse(MultiModelJson), MultiModelOptions());

        Assert.IsNotNull(ensemble);
        Assert.AreEqual(EnsembleModelOptionsParameter.gfs025, ensemble.Model);
        Assert.AreEqual(1.0f, ensemble.Hourly!.Temperature_2m![0]![0]);
    }

    [TestMethod]
    public async Task DeserializeJsonListAsync_OverlappingVariableNames_UsesLongestMatch()
    {
        // temperature_2m and temperature_2m_max are both hourly variables, so
        // temperature_2m_max_<model> must not be read as temperature_2m with model "max_<model>"
        var json = @"{
            ""latitude"": 1, ""longitude"": 1, ""generationtime_ms"": 1, ""utc_offset_seconds"": 0, ""timezone"": ""GMT"", ""timezone_abbreviation"": ""GMT"", ""elevation"": 1,
            ""hourly"": {
                ""time"": [""2026-11-15T00:00""],
                ""temperature_2m_ncep_gefs025"": [1.0], ""temperature_2m_max_ncep_gefs025"": [3.0],
                ""temperature_2m_ecmwf_ifs025_ensemble"": [2.0], ""temperature_2m_max_ecmwf_ifs025_ensemble"": [4.0]
            }
        }";
        var options = new WeatherEnsembleOptions
        {
            Hourly = [WeatherEnsembleHourlyOptionsParameter.temperature_2m, WeatherEnsembleHourlyOptionsParameter.temperature_2m_max],
            Models = [EnsembleModelOptionsParameter.gfs025, EnsembleModelOptionsParameter.ecmwf_ifs025]
        };
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);

        var ensembles = await parser.DeserializeJsonListAsync(JsonResponse(json), options);

        Assert.IsNotNull(ensembles);
        Assert.AreEqual(2, ensembles.Count);
        Assert.AreEqual(1.0f, ensembles[0].Hourly!.Temperature_2m![0]![0]);
        Assert.AreEqual(3.0f, ensembles[0].Hourly!.Temperature_2m_max![0]![0]);
        Assert.AreEqual(2.0f, ensembles[1].Hourly!.Temperature_2m![0]![0]);
        Assert.AreEqual(4.0f, ensembles[1].Hourly!.Temperature_2m_max![0]![0]);
    }

    [TestMethod]
    public async Task DeserializeJsonListAsync_SingleModel_ReturnsOneEnsembleWithModel()
    {
        var json = @"{
            ""latitude"": 1, ""longitude"": 1, ""generationtime_ms"": 1, ""utc_offset_seconds"": 0, ""timezone"": ""GMT"", ""timezone_abbreviation"": ""GMT"", ""elevation"": 1,
            ""hourly"": { ""time"": [""2026-11-15T00:00""], ""temperature_2m"": [1.0], ""temperature_2m_member01"": [2.0] }
        }";
        var options = new WeatherEnsembleOptions
        {
            Hourly = [WeatherEnsembleHourlyOptionsParameter.temperature_2m],
            Models = [EnsembleModelOptionsParameter.google_weathernext2_ensemble]
        };
        var parser = new WeatherEnsembleResponseParser(_jsonSerializerOptions);

        var ensembles = await parser.DeserializeJsonListAsync(JsonResponse(json), options);

        Assert.IsNotNull(ensembles);
        Assert.AreEqual(1, ensembles.Count);
        Assert.AreEqual(EnsembleModelOptionsParameter.google_weathernext2_ensemble, ensembles[0].Model);
        Assert.AreEqual(2, ensembles[0].Hourly!.Temperature_2m!.Count);
    }

    [DataTestMethod]
    [TestCategory(TestCategoryConstants.Integration)]
    [DataRow(false)]
    [DataRow(true)]
    public async Task QueryEnsembleModelsApiAsync_MultipleModels_ReturnsEachModel(bool useFlatbuffers)
    {
        var client = new OpenMeteoClient { UseFlatbuffers = useFlatbuffers, RethrowExceptions = true };
        var options = new WeatherEnsembleOptions(47.37f, 8.54f) // Zurich
        {
            Hourly = [WeatherEnsembleHourlyOptionsParameter.temperature_2m],
            Daily = [WeatherEnsembleDailyOptionsParameter.temperature_2m_max],
            Models = [EnsembleModelOptionsParameter.gfs025, EnsembleModelOptionsParameter.google_weathernext2_ensemble, EnsembleModelOptionsParameter.icon_seamless],
            Forecast_Days = 2
        };

        var ensembles = await client.QueryEnsembleModelsApiAsync(options);

        Assert.IsNotNull(ensembles);
        Assert.AreEqual(3, ensembles.Count);
        CollectionAssert.AreEqual(
            new EnsembleModelOptionsParameter?[] { EnsembleModelOptionsParameter.gfs025, EnsembleModelOptionsParameter.google_weathernext2_ensemble, EnsembleModelOptionsParameter.icon_seamless },
            new[] { ensembles[0].Model, ensembles[1].Model, ensembles[2].Model });

        // GEFS 0.25° has 31 members, WeatherNext 2 has 64 and ICON-EPS has 40
        Assert.AreEqual(31, ensembles[0].Hourly!.Temperature_2m!.Count);
        Assert.AreEqual(64, ensembles[1].Hourly!.Temperature_2m!.Count);
        Assert.AreEqual(40, ensembles[2].Hourly!.Temperature_2m!.Count);
        foreach (var ensemble in ensembles)
        {
            Assert.IsTrue(ensemble.Hourly!.Time!.Length > 0);
            Assert.IsTrue(ensemble.Daily!.Temperature_2m_max!.Count > 0);
        }
    }
}
