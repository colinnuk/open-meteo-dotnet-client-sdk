namespace OpenMeteo.Weather.Seasonal.Options;

// Enum member names are the exact API identifiers; they are sent to the API via ToString().

/// <summary>
/// 6-hourly weather variables for the Seasonal Forecast API. The API serves 6-hourly data through the "hourly" parameter.
/// </summary>
public enum SeasonalHourlyOptionsParameter
{
    temperature_2m,
    dew_point_2m,
    relative_humidity_2m,
    apparent_temperature,
    pressure_msl,
    weather_code,
    precipitation,
    rain,
    snowfall,
    cloud_cover,
    wind_speed_10m,
    wind_speed_100m,
    wind_direction_10m,
    sea_surface_temperature,
    soil_temperature_0_to_7cm,
    shortwave_radiation,
    direct_radiation,
    vapour_pressure_deficit,
    et0_fao_evapotranspiration
}

/// <summary>
/// Daily weather variables for the Seasonal Forecast API
/// </summary>
public enum SeasonalDailyOptionsParameter
{
    temperature_2m_max,
    temperature_2m_min,
    temperature_2m_mean,
    precipitation_sum,
    rain_sum,
    snowfall_sum,
    wind_speed_10m_max,
    wind_speed_10m_mean,
    wind_gusts_10m_max,
    shortwave_radiation_sum,
    et0_fao_evapotranspiration,
    cloud_cover_mean,
    pressure_msl_mean,
    weather_code,
    sunshine_duration
}

/// <summary>
/// Weekly weather variables (means, anomalies and probabilities) for the Seasonal Forecast API
/// </summary>
public enum SeasonalWeeklyOptionsParameter
{
    temperature_2m_mean,
    temperature_2m_anomaly,
    temperature_2m_efi,
    temperature_2m_sot10,
    temperature_2m_sot90,
    dew_point_2m_mean,
    precipitation_mean,
    precipitation_anomaly,
    precipitation_efi,
    snowfall_mean,
    pressure_msl_mean,
    wind_speed_10m_mean,
    cloud_cover_mean,
    sunshine_duration_mean,
    sea_surface_temperature_mean
}

/// <summary>
/// Monthly weather variables (means and anomalies) for the Seasonal Forecast API
/// </summary>
public enum SeasonalMonthlyOptionsParameter
{
    temperature_2m_mean,
    temperature_2m_anomaly,
    precipitation_mean,
    precipitation_anomaly,
    snowfall_mean,
    cloud_cover_mean,
    pressure_msl_mean,
    wind_speed_10m_mean,
    shortwave_radiation_mean,
    sea_surface_temperature_mean
}
