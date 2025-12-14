using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Forecast.Options;
using System;

namespace OpenMeteoTests.Utilities;

public static class EnsembleOptionsHelper
{
    public static WeatherEnsembleOptions GetOptions(float latitude, float longitude)
    {
        var utcDateNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var utcDateEnd = utcDateNow.AddDays(2);
        var options = new WeatherEnsembleOptions
        {
            Latitude = latitude,
            Longitude = longitude,
            Hourly = GetHourlyOptions(),
            Models = new EnsembleModelOptions(EnsembleModelOptionsParameter.gem_global),
            Temperature_Unit = TemperatureUnitType.celsius,
            Windspeed_Unit = WindspeedUnitType.kmh,
            Precipitation_Unit = PrecipitationUnitType.mm,
            Timeformat = TimeformatType.iso8601,
            Cell_Selection = CellSelectionType.land,
            Timezone = "GMT",
            Start_date = utcDateNow,
            End_date = utcDateEnd
        };
        return options;
    }

    public static WeatherEnsembleOptions GetAllOptions(float latitude, float longitude)
    {
        var utcDateNow = DateOnly.FromDateTime(DateTime.UtcNow);
        var utcDateEnd = utcDateNow.AddDays(2);
        var options = new WeatherEnsembleOptions
        {
            Latitude = latitude,
            Longitude = longitude,
            Hourly = WeatherEnsembleHourlyOptions.All,
            Daily = WeatherEnsembleDailyOptions.All,
            Models = new EnsembleModelOptions(EnsembleModelOptionsParameter.gem_global),
            Temperature_Unit = TemperatureUnitType.celsius,
            Windspeed_Unit = WindspeedUnitType.kmh,
            Precipitation_Unit = PrecipitationUnitType.mm,
            Timeformat = TimeformatType.iso8601,
            Cell_Selection = CellSelectionType.land,
            Timezone = "GMT",
            Start_date = utcDateNow,
            End_date = utcDateEnd
        };
        return options;
    }

    private static WeatherEnsembleHourlyOptions GetHourlyOptions()
    {
        var options = new WeatherEnsembleHourlyOptionsParameter[]
        {
             WeatherEnsembleHourlyOptionsParameter.temperature_2m,
             WeatherEnsembleHourlyOptionsParameter.precipitation,
             WeatherEnsembleHourlyOptionsParameter.windspeed_10m,
             WeatherEnsembleHourlyOptionsParameter.pressure_msl,
             WeatherEnsembleHourlyOptionsParameter.cloudcover,
        };
        return new WeatherEnsembleHourlyOptions(options);
    }
}
