using System;

namespace OpenMeteo.Weather.Forecast.Options
{
    public interface IWeatherForecastOptions
    {
        TemperatureUnitType Temperature_Unit { get; }
        WindspeedUnitType Windspeed_Unit { get; }
        PrecipitationUnitType Precipitation_Unit { get; }
        CellSelectionType Cell_Selection { get; }
        string Timezone { get; }
        HourlyOptions Hourly { get; }
        DailyOptions Daily { get; }
        WeatherModelOptions Models { get; }
        CurrentOptions Current { get; }
        Minutely15Options Minutely_15 { get; }
        TimeformatType Timeformat { get; }
        int? Past_Days { get; }
        DateOnly? Start_date { get; }
        DateOnly? End_date { get; }
    }
}
