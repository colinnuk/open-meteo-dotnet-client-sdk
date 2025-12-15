using System;
using System.Collections.Generic;

namespace OpenMeteo.Weather.Ensemble.ResponseModel;

/// <summary>
/// Daily ensemble weather data with individual member forecasts
/// </summary>
public class EnsembleDaily
{
    /// <summary>
    /// Array of dates for daily data
    /// </summary>
    public DateOnly[]? Time { get; set; }

    // Temperature Variables
    public Dictionary<int, float?[]?>? Temperature_2m_mean { get; set; }
    public Dictionary<int, float?[]?>? Temperature_2m_min { get; set; }
    public Dictionary<int, float?[]?>? Temperature_2m_max { get; set; }
    
    // Apparent Temperature
    public Dictionary<int, float?[]?>? Apparent_temperature_mean { get; set; }
    public Dictionary<int, float?[]?>? Apparent_temperature_min { get; set; }
    public Dictionary<int, float?[]?>? Apparent_temperature_max { get; set; }

    // Wind Speed at 10m
    public Dictionary<int, float?[]?>? Windspeed_10m_mean { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_10m_min { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_10m_max { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_10m_dominant { get; set; }

    // Wind Gusts at 10m
    public Dictionary<int, float?[]?>? Windgusts_10m_mean { get; set; }
    public Dictionary<int, float?[]?>? Windgusts_10m_min { get; set; }
    public Dictionary<int, float?[]?>? Windgusts_10m_max { get; set; }

    // Wind Speed at 100m
    public Dictionary<int, float?[]?>? Windspeed_100m_mean { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_100m_min { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_100m_max { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_100m_dominant { get; set; }

    // Cloud Cover
    public Dictionary<int, int?[]?>? Cloudcover_mean { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_min { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_max { get; set; }

    // Precipitation
    public Dictionary<int, float?[]?>? Precipitation_sum { get; set; }
    public Dictionary<int, float?[]?>? Precipitation_hours { get; set; }
    public Dictionary<int, float?[]?>? Rain_sum { get; set; }
    public Dictionary<int, float?[]?>? Snowfall_sum { get; set; }

    // Pressure at Mean Sea Level
    public Dictionary<int, float?[]?>? Pressure_msl_mean { get; set; }
    public Dictionary<int, float?[]?>? Pressure_msl_min { get; set; }
    public Dictionary<int, float?[]?>? Pressure_msl_max { get; set; }

    // Surface Pressure
    public Dictionary<int, float?[]?>? Surface_pressure_mean { get; set; }
    public Dictionary<int, float?[]?>? Surface_pressure_min { get; set; }
    public Dictionary<int, float?[]?>? Surface_pressure_max { get; set; }

    // Relative Humidity
    public Dictionary<int, int?[]?>? Relativehumidity_2m_mean { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_2m_min { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_2m_max { get; set; }

    // CAPE (Convective Available Potential Energy)
    public Dictionary<int, float?[]?>? Cape_mean { get; set; }
    public Dictionary<int, float?[]?>? Cape_min { get; set; }
    public Dictionary<int, float?[]?>? Cape_max { get; set; }

    // Dewpoint
    public Dictionary<int, float?[]?>? Dewpoint_2m_mean { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_2m_min { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_2m_max { get; set; }

    // Evapotranspiration and Solar Radiation
    public Dictionary<int, float?[]?>? Et0_fao_evapotranspiration { get; set; }
    public Dictionary<int, float?[]?>? Shortwave_radiation_sum { get; set; }
}
