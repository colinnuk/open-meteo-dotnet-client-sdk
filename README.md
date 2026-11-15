
# 🌡️🌤️ Open-Meteo Dotnet Client SDK Library
[![build and test](https://github.com/colinnuk/open-meteo-dotnet-client-sdk/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/colinnuk/open-meteo-dotnet-client-sdk/actions/workflows/build-and-test.yml)
[![GitHub license](https://img.shields.io/github/license/colinnuk/open-meteo-dotnet-client-sdk)](https://github.com/colinnuk/open-meteo-dotnet-client-sdk/blob/master/LICENSE)
[![Nuget](https://img.shields.io/nuget/v/openmeteo.dotnet.client.sdk)](https://www.nuget.org/packages/OpenMeteo.dotnet.client.sdk)

A .NET 8 client library for the [Open-Meteo](https://open-meteo.com) weather API.
Forked from [AlienDwarf/open-meteo-dotnet](https://github.com/AlienDwarf/open-meteo-dotnet).

## ❕ Features

- Typed clients for the Open-Meteo [forecast](#weather-forecast), [ensemble](#ensemble-forecast), [seasonal](#seasonal-forecast), [previous runs and single runs](#previous-runs-and-single-runs), [air quality](#air-quality), [geocoding and elevation](#geocoding-and-elevation) APIs
- Offline [regional model coverage](#regional-model-coverage) checks for deterministic and ensemble models
- Most of the Open-Meteo weather models (as of October 2025)
- Look up a location by name, or query by latitude and longitude
- API key support, custom base URLs for self-hosted instances, and bring-your-own `HttpClient`
- [FlatBuffers](#flatbuffers) support for much faster parsing of large datasets
- Optional [exceptions](#error-handling) with HTTP status codes for detailed error handling

Pretty much all the library's classes are public, so you can extend the library as needed in your own code.

## 🔨 Installation

Add the [NuGet package](https://www.nuget.org/packages/OpenMeteo.dotnet.client.sdk/) to your project:

```bash
dotnet add package OpenMeteo.dotnet.client.sdk
```

Requires .NET 8 or later.

## 💻 Usage

### Quick start
Create one `OpenMeteoClient` and reuse it for every call.

```cs
using OpenMeteo;

var client = new OpenMeteoClient();

// Look up Tokyo by name and get its current weather
var weather = await client.QueryWeatherApiAsync("Tokyo");

Console.WriteLine($"Weather in Tokyo: {weather?.Current?.Temperature}{weather?.CurrentUnits?.Temperature}");
// Output: Weather in Tokyo: 28.1°C
```

Every query method returns `null` when the request fails, unless you turn on [exceptions](#error-handling).

### Weather forecast
Choose the variables, models, units and timezone with `WeatherForecastOptions`:

```cs
using OpenMeteo.Weather.Forecast.Options;

var options = new WeatherForecastOptions(52.52f, 13.41f)
{
    Current = [CurrentOptionsParameter.temperature_2m, CurrentOptionsParameter.windspeed_10m],
    Hourly = [HourlyOptionsParameter.temperature_2m, HourlyOptionsParameter.precipitation],
    Daily = [DailyOptionsParameter.temperature_2m_max, DailyOptionsParameter.temperature_2m_min],
    Models = [WeatherModelOptionsParameter.best_match],
    Timezone = "Europe/Berlin",
    Temperature_Unit = TemperatureUnitType.fahrenheit
};

var forecast = await client.QueryWeatherApiAsync(options);

float? maxToday = forecast?.Daily?.Temperature_2m_max?[0];
```

You can also pass a location name together with options (`QueryWeatherApiAsync("Berlin", options)`) or query several coordinates in one call (`QueryWeatherApiAsync(coordinates, options)`).

### Ensemble forecast
The [Ensemble API](https://open-meteo.com/en/docs/ensemble-api) returns every ensemble member. Values are keyed by member number, where `0` is the control run.

```cs
using OpenMeteo.Weather.Ensemble.Options;

var options = new WeatherEnsembleOptions(52.52f, 13.41f)
{
    Hourly = [WeatherEnsembleHourlyOptionsParameter.temperature_2m],
    Models = [EnsembleModelOptionsParameter.gfs025]
};

var ensemble = await client.QueryEnsembleApiAsync(options);

foreach (var (member, values) in ensemble!.Hourly!.Temperature_2m!)
    Console.WriteLine($"Member {member}: {values?[0]}");
```

To compare several ensemble models, for example GFS, ECMWF IFS and Google WeatherNext 2, list them all in `Models` and call `QueryEnsembleModelsApiAsync`. It makes one request and returns one `WeatherEnsemble` per model, in the order you listed them. Each result's `Model` property says which model it holds. (`QueryEnsembleApiAsync` returns only the first model.)

```cs
var options = new WeatherEnsembleOptions(52.52f, 13.41f)
{
    Hourly = [WeatherEnsembleHourlyOptionsParameter.temperature_2m],
    Models = [EnsembleModelOptionsParameter.gfs025, EnsembleModelOptionsParameter.ecmwf_ifs025, EnsembleModelOptionsParameter.google_weathernext2_ensemble]
};

var ensembles = await client.QueryEnsembleModelsApiAsync(options);

foreach (var ensemble in ensembles!)
    Console.WriteLine($"{ensemble.Model}: {ensemble.Hourly!.Temperature_2m!.Count} members");
```

### Seasonal forecast
The [Seasonal Forecast API](https://open-meteo.com/en/docs/seasonal-forecast-api) (ECMWF SEAS5 and EC46) forecasts up to 7 months ahead as hourly (6-hourly), daily, weekly and monthly data.

```cs
using OpenMeteo.Weather.Seasonal.Options;

var options = new SeasonalOptions(52.52f, 13.41f)
{
    Daily = [SeasonalDailyOptionsParameter.temperature_2m_max],
    Monthly = [SeasonalMonthlyOptionsParameter.temperature_2m_anomaly],
    Forecast_Days = 30
};

var seasonal = await client.QuerySeasonalApiAsync(options);

float?[]? control = seasonal!.Daily!.Get("temperature_2m_max");
IReadOnlyList<float?[]> members = seasonal.Daily.GetMembers("temperature_2m_max"); // one array per ensemble member
```

- Variables of a series are looked up by their API name. Use `Get` for the control run (or the ensemble mean) and `GetMembers` for the individual members.
- 6-hourly data is requested with `Hourly`, and weekly and monthly data with `Weekly` and `Monthly`. Choose a model with `Models`, for example `ecmwf_seas5_ensemble_mean` to get the ensemble mean instead of all 51 members.
- The API ignores `forecast_months`. Limit the range with `Forecast_Days` (up to 217), `Start_date`/`End_date` or `Past_Days`.

### Previous runs and single runs
`QueryPreviousRunsApiAsync` returns what earlier model runs predicted for the same hours (see the [Previous Runs API](https://open-meteo.com/en/docs/previous-runs-api)), and `QuerySingleRunsApiAsync` returns one specific model run (see the [Single Runs API](https://open-meteo.com/en/docs/single-runs-api)). Both take `PreviousRunsOptions` and `SingleRunsOptions`.

### Air quality
```cs
using OpenMeteo.AirQuality;

var options = new AirQualityOptions(52.52f, 13.41f)
{
    Hourly = [AirQualityOptions.HourlyOptionsParameter.pm10]
};

var airQuality = await client.QueryAirQualityAsync(options);
```

### Geocoding and elevation
```cs
var location = await client.GetLocationDataAsync("Berlin");
Console.WriteLine(location?.Locations?[0].Name);

var elevation = await client.QueryElevationAsync(52.52f, 13.41f);
Console.WriteLine(elevation?.Elevation?[0]);
```

### Regional model coverage
Regional models only cover part of the world. You can check whether a location falls inside a model's area without calling the API. The boundaries are built into the library. `IsLocationInModelCoverage` returns `true` or `false` for regional models and `null` for global ones.

```cs
using OpenMeteo.Weather.Ensemble.RegionalModelCoverage;
using OpenMeteo.Weather.Forecast.RegionalModelCoverage;

bool? inIconD2 = new RegionalModelCoverageHelper()
    .IsLocationInModelCoverage(WeatherModelOptionsParameter.icon_d2, 52.52f, 13.41f);   // true

bool? inEcmwfEurope = new EnsembleModelCoverageHelper()
    .IsLocationInModelCoverage(EnsembleModelOptionsParameter.ecmwf_ifs_europe_ensemble, 40.71f, -74.01f); // false
```

Both helpers have interfaces (`IRegionalModelCoverageHelper` and `IEnsembleModelCoverageHelper`) for dependency injection. The ICON-EU and ICON-D2 ensembles use the deterministic ICON boundaries, which are close to, but not exactly, their native grids.

## ⚙️ Configuration

### API key
Pass your key to the constructor to use the customer API servers, such as `customer-api.open-meteo.com`:

```cs
var client = new OpenMeteoClient("YourSecretApiKeyHere");
```

### Self-hosted instance
Point the client at your own Open-Meteo server, optionally combined with an API key:

```cs
var client = new OpenMeteoClient(new Uri("https://open-meteo.example.com"));
```

### Custom HttpClient
Each constructor has an overload that accepts a pre-configured `HttpClient`, for example one from `IHttpClientFactory`:

```cs
var client = new OpenMeteoClient(httpClient);
```

### FlatBuffers
Set `UseFlatbuffers` to request the binary [FlatBuffers](https://flatbuffers.dev) format, which parses much faster for large responses. The results are the same as with JSON.

```cs
var client = new OpenMeteoClient { UseFlatbuffers = true };
```

For the seasonal API this applies to hourly, daily and monthly data. The binary format has no start date for weekly data, so a seasonal request that includes `Weekly` variables always uses JSON.

### Error handling
By default a failed request returns `null`. Set `RethrowExceptions` to get an exception instead. API errors are thrown as `OpenMeteoClientException`, which carries the HTTP status code and the reason reported by the API.

```cs
var client = new OpenMeteoClient { RethrowExceptions = true };

try
{
    var forecast = await client.QueryWeatherApiAsync(options);
}
catch (OpenMeteoClientException ex)
{
    Console.WriteLine($"{ex.StatusCode}: {ex.Message}");
}
```

More usage examples are in the [tests](https://github.com/colinnuk/open-meteo-dotnet-client-sdk/tree/master/OpenMeteoTests).

## 🗂️ Scripts

| Script | Description |
|--------|-------------|
| [`Scripts/Generate-RegionalCoverageClasses.ps1`](Scripts/Generate-RegionalCoverageClasses.ps1) | Reads each `.geojson` boundary file under `OpenMeteo/Weather/Forecast/RegionalModelCoverage/` and regenerates the corresponding `*Coverage.cs` static C# class. Re-run this script and commit the output whenever a GeoJSON boundary file is added or updated. |

## 🤝 Contributing

Contributions are welcome. Please read the [contributing guide](CONTRIBUTING.md) first, and make sure `dotnet test` passes before you open a pull request.

## License

This project is open-source under the [MIT](https://github.com/colinnuk/open-meteo-dotnet-client-sdk/blob/master/LICENSE) license.

## Appendix

This library uses the public and freely available [Open-Meteo](https://open-meteo.com) API servers.
See also:
- [omgo - Open Meteo SDK written in Go](https://github.com/HectorMalot/omgo)
- [OpenMeteoPy](https://github.com/m0rp43us/openmeteopy)
- [Open-Meteo Kotlin Library](https://github.com/open-meteo/open-meteo-api-kotlin)
- [AlienDwarf/open-meteo-dotnet](https://github.com/AlienDwarf/open-meteo-dotnet)
