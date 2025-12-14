using OpenMeteo.Weather.Forecast.Options;
using System;

namespace OpenMeteoTests.Utilities
{
    public static class ForecastOptionsHelper
    {
        public static WeatherForecastOptions GetOptions(float latitude, float longitude)
        {
            var utcDateNow = DateOnly.FromDateTime(DateTime.UtcNow);
            var utcDateEnd = utcDateNow.AddDays(2);
            var options = new WeatherForecastOptions()
            {
                Latitude = latitude,
                Longitude = longitude,
                Hourly = GetHourlyOptions(),
                Models = new WeatherModelOptions(WeatherModelOptionsParameter.gfs_hrrr),
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

        public static WeatherForecastOptions GetAllOptions(float latitude, float longitude)
        {
            var utcDateNow = DateOnly.FromDateTime(DateTime.UtcNow);
            var utcDateEnd = utcDateNow.AddDays(2);
            var options = new WeatherForecastOptions()
            {
                Latitude = latitude,
                Longitude = longitude,
                Hourly = HourlyOptions.All,
                Current = CurrentOptions.All,
                Minutely_15 = Minutely15Options.All,
                Daily = DailyOptions.All,
                Models = new WeatherModelOptions(WeatherModelOptionsParameter.gfs_hrrr),
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

        private static HourlyOptions GetHourlyOptions()
        {
            var detailedPressureLevelMetrics = new HourlyOptionsParameter[]
            {
                 HourlyOptionsParameter.cloudcover_975hPa,
                 HourlyOptionsParameter.cloudcover_925hPa,
                 HourlyOptionsParameter.cloudcover_875hPa,
                 HourlyOptionsParameter.cloudcover_825hPa,
                 HourlyOptionsParameter.cloudcover_775hPa,
                 HourlyOptionsParameter.cloudcover_725hPa,
                 HourlyOptionsParameter.cloudcover_675hPa,
                 HourlyOptionsParameter.cloudcover_625hPa,
                 HourlyOptionsParameter.cloudcover_575hPa,
                 HourlyOptionsParameter.cloudcover_525hPa,
                 HourlyOptionsParameter.relativehumidity_975hPa,
                 HourlyOptionsParameter.relativehumidity_925hPa,
                 HourlyOptionsParameter.relativehumidity_875hPa,
                 HourlyOptionsParameter.relativehumidity_825hPa,
                 HourlyOptionsParameter.relativehumidity_775hPa,
                 HourlyOptionsParameter.relativehumidity_725hPa,
                 HourlyOptionsParameter.relativehumidity_675hPa,
                 HourlyOptionsParameter.relativehumidity_625hPa,
                 HourlyOptionsParameter.relativehumidity_575hPa,
                 HourlyOptionsParameter.relativehumidity_525hPa,
            };
            var options = new HourlyOptionsParameter[]
            {
                 HourlyOptionsParameter.temperature_2m,
                 HourlyOptionsParameter.relativehumidity_2m,
                 HourlyOptionsParameter.surface_pressure,
                 HourlyOptionsParameter.precipitation,
                 HourlyOptionsParameter.rain,
                 HourlyOptionsParameter.showers,
                 HourlyOptionsParameter.snowfall,
                 HourlyOptionsParameter.snowfall_height,
                 HourlyOptionsParameter.freezing_level_height,
                 HourlyOptionsParameter.weathercode,
                 HourlyOptionsParameter.cloudcover,
                 HourlyOptionsParameter.cloudcover_low,
                 HourlyOptionsParameter.cloudcover_mid,
                 HourlyOptionsParameter.cloudcover_high,
                 HourlyOptionsParameter.windspeed_10m,
                 HourlyOptionsParameter.winddirection_10m,
                 HourlyOptionsParameter.windgusts_10m,
                 HourlyOptionsParameter.windspeed_900hPa,
                 HourlyOptionsParameter.windspeed_850hPa,
                 HourlyOptionsParameter.windspeed_800hPa,
                 HourlyOptionsParameter.windspeed_700hPa,
                 HourlyOptionsParameter.windspeed_600hPa,
                 HourlyOptionsParameter.windspeed_500hPa,
                 HourlyOptionsParameter.winddirection_900hPa,
                 HourlyOptionsParameter.winddirection_850hPa,
                 HourlyOptionsParameter.winddirection_800hPa,
                 HourlyOptionsParameter.winddirection_700hPa,
                 HourlyOptionsParameter.winddirection_600hPa,
                 HourlyOptionsParameter.winddirection_500hPa,
                 HourlyOptionsParameter.temperature_1000hPa,
                 HourlyOptionsParameter.temperature_975hPa,
                 HourlyOptionsParameter.temperature_950hPa,
                 HourlyOptionsParameter.temperature_925hPa,
                 HourlyOptionsParameter.temperature_900hPa,
                 HourlyOptionsParameter.temperature_850hPa,
                 HourlyOptionsParameter.temperature_800hPa,
                 HourlyOptionsParameter.temperature_700hPa,
                 HourlyOptionsParameter.temperature_600hPa,
                 HourlyOptionsParameter.temperature_500hPa,
                 HourlyOptionsParameter.temperature_400hPa,
                 HourlyOptionsParameter.geopotential_height_1000hPa,
                 HourlyOptionsParameter.geopotential_height_975hPa,
                 HourlyOptionsParameter.geopotential_height_950hPa,
                 HourlyOptionsParameter.geopotential_height_925hPa,
                 HourlyOptionsParameter.geopotential_height_900hPa,
                 HourlyOptionsParameter.geopotential_height_850hPa,
                 HourlyOptionsParameter.geopotential_height_800hPa,
                 HourlyOptionsParameter.geopotential_height_700hPa,
                 HourlyOptionsParameter.geopotential_height_600hPa,
                 HourlyOptionsParameter.geopotential_height_500hPa,
                 HourlyOptionsParameter.geopotential_height_400hPa,
                 HourlyOptionsParameter.cloudcover_1000hPa,
                 HourlyOptionsParameter.cloudcover_950hPa,
                 HourlyOptionsParameter.cloudcover_900hPa,
                 HourlyOptionsParameter.cloudcover_850hPa,
                 HourlyOptionsParameter.cloudcover_800hPa,
                 HourlyOptionsParameter.cloudcover_750hPa,
                 HourlyOptionsParameter.cloudcover_700hPa,
                 HourlyOptionsParameter.cloudcover_650hPa,
                 HourlyOptionsParameter.cloudcover_600hPa,
                 HourlyOptionsParameter.cloudcover_550hPa,
                 HourlyOptionsParameter.cloudcover_500hPa,
                 HourlyOptionsParameter.relativehumidity_1000hPa,
                 HourlyOptionsParameter.relativehumidity_950hPa,
                 HourlyOptionsParameter.relativehumidity_900hPa,
                 HourlyOptionsParameter.relativehumidity_850hPa,
                 HourlyOptionsParameter.relativehumidity_800hPa,
                 HourlyOptionsParameter.relativehumidity_750hPa,
                 HourlyOptionsParameter.relativehumidity_700hPa,
                 HourlyOptionsParameter.relativehumidity_650hPa,
                 HourlyOptionsParameter.relativehumidity_600hPa,
                 HourlyOptionsParameter.relativehumidity_550hPa,
                 HourlyOptionsParameter.relativehumidity_500hPa,
            };
            return new HourlyOptions([.. options, .. detailedPressureLevelMetrics]);
        }
    }
}
