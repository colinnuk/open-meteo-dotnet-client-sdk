using System;
using System.Collections.Generic;

namespace OpenMeteo.Weather.Ensemble.ResponseModel;

/// <summary>
/// Hourly ensemble weather data with individual member forecasts
/// </summary>
public class EnsembleHourly
{
    /// <summary>
    /// Array of timestamps for hourly data
    /// </summary>
    public DateTimeOffset[]? Time { get; set; }

    // Basic Weather Variables
    public Dictionary<int, float?[]?>? Temperature_2m { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_2m { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_2m { get; set; }
    public Dictionary<int, float?[]?>? Apparent_temperature { get; set; }
    public Dictionary<int, float?[]?>? Precipitation { get; set; }
    public Dictionary<int, float?[]?>? Rain { get; set; }
    public Dictionary<int, float?[]?>? Snowfall { get; set; }
    public Dictionary<int, float?[]?>? Snow_depth { get; set; }
    public Dictionary<int, int?[]?>? Weathercode { get; set; }

    // Pressure Variables
    public Dictionary<int, float?[]?>? Pressure_msl { get; set; }
    public Dictionary<int, float?[]?>? Surface_pressure { get; set; }

    // Cloud Cover Variables
    public Dictionary<int, int?[]?>? Cloudcover { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_low { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_mid { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_high { get; set; }

    // Other Atmospheric Variables
    public Dictionary<int, float?[]?>? Visibility { get; set; }
    public Dictionary<int, float?[]?>? Et0_fao_evapotranspiration { get; set; }
    public Dictionary<int, float?[]?>? Vapor_pressure_deficit { get; set; }

    // Wind Variables (Surface and Altitude)
    public Dictionary<int, float?[]?>? Windspeed_10m { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_80m { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_100m { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_120m { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_10m { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_80m { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_100m { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_120m { get; set; }
    public Dictionary<int, float?[]?>? Windgusts_10m { get; set; }

    // Temperature at Altitude
    public Dictionary<int, float?[]?>? Temperature_80m { get; set; }
    public Dictionary<int, float?[]?>? Temperature_120m { get; set; }
    public Dictionary<int, float?[]?>? Surface_temperature { get; set; }

    // Soil Temperature (Different depth ranges)
    public Dictionary<int, float?[]?>? Soil_temperature_0_to_10cm { get; set; }
    public Dictionary<int, float?[]?>? Soil_temperature_10_to_40cm { get; set; }
    public Dictionary<int, float?[]?>? Soil_temperature_40_to_100cm { get; set; }
    public Dictionary<int, float?[]?>? Soil_temperature_100_to_200cm { get; set; }
    public Dictionary<int, float?[]?>? Soil_temperature_0_to_7cm { get; set; }
    public Dictionary<int, float?[]?>? Soil_temperature_7_to_28cm { get; set; }
    public Dictionary<int, float?[]?>? Soil_temperature_28_to_100cm { get; set; }
    public Dictionary<int, float?[]?>? Soil_temperature_100_to_255cm { get; set; }

    // Soil Moisture
    public Dictionary<int, float?[]?>? Soil_moisture_0_to_10cm { get; set; }
    public Dictionary<int, float?[]?>? Soil_moisture_10_to_40cm { get; set; }
    public Dictionary<int, float?[]?>? Soil_moisture_40_to_100cm { get; set; }
    public Dictionary<int, float?[]?>? Soil_moisture_100_to_255cm { get; set; }
    public Dictionary<int, float?[]?>? Soil_moisture_0_to_7cm { get; set; }
    public Dictionary<int, float?[]?>? Soil_moisture_7_to_28cm { get; set; }
    public Dictionary<int, float?[]?>? Soil_moisture_28_to_100cm { get; set; }
    public Dictionary<int, float?[]?>? Soil_moisture_100_to_400cm { get; set; }

    // UV and Temperature Extremes
    public Dictionary<int, float?[]?>? Uv_index { get; set; }
    public Dictionary<int, float?[]?>? Uv_index_clear_sky { get; set; }
    public Dictionary<int, float?[]?>? Temperature_2m_min { get; set; }
    public Dictionary<int, float?[]?>? Temperature_2m_max { get; set; }
    public Dictionary<int, float?[]?>? Wet_bulb_temperature_2m { get; set; }

    // Stability Indices
    public Dictionary<int, float?[]?>? Cape { get; set; }
    public Dictionary<int, float?[]?>? Convective_inhibition { get; set; }

    // Snow and Ice
    public Dictionary<int, float?[]?>? Freezing_level_height { get; set; }
    public Dictionary<int, float?[]?>? Snowfall_height { get; set; }
    public Dictionary<int, float?[]?>? Snowfall_water_equivalent { get; set; }
    public Dictionary<int, float?[]?>? Snow_depth_water_equivalent { get; set; }

    // Solar Radiation
    public Dictionary<int, float?[]?>? Sunshine_duration { get; set; }
    public Dictionary<int, float?[]?>? Shortwave_radiation { get; set; }
    public Dictionary<int, float?[]?>? Direct_radiation { get; set; }
    public Dictionary<int, float?[]?>? Diffuse_radiation { get; set; }
    public Dictionary<int, float?[]?>? Direct_normal_irradiance { get; set; }
    public Dictionary<int, float?[]?>? Global_tilted_irradiance { get; set; }
    public Dictionary<int, float?[]?>? Shortwave_radiation_instant { get; set; }
    public Dictionary<int, float?[]?>? Direct_radiation_instant { get; set; }
    public Dictionary<int, float?[]?>? Diffuse_radiation_instant { get; set; }
    public Dictionary<int, float?[]?>? Direct_normal_irradiance_instant { get; set; }
    public Dictionary<int, float?[]?>? Global_tilted_irradiance_instant { get; set; }

    // Pressure Level Variables - Temperature
    public Dictionary<int, float?[]?>? Temperature_1000hPa { get; set; }
    public Dictionary<int, float?[]?>? Temperature_925hPa { get; set; }
    public Dictionary<int, float?[]?>? Temperature_850hPa { get; set; }
    public Dictionary<int, float?[]?>? Temperature_700hPa { get; set; }
    public Dictionary<int, float?[]?>? Temperature_600hPa { get; set; }
    public Dictionary<int, float?[]?>? Temperature_500hPa { get; set; }
    public Dictionary<int, float?[]?>? Temperature_400hPa { get; set; }
    public Dictionary<int, float?[]?>? Temperature_300hPa { get; set; }
    public Dictionary<int, float?[]?>? Temperature_250hPa { get; set; }
    public Dictionary<int, float?[]?>? Temperature_200hPa { get; set; }
    public Dictionary<int, float?[]?>? Temperature_150hPa { get; set; }
    public Dictionary<int, float?[]?>? Temperature_100hPa { get; set; }
    public Dictionary<int, float?[]?>? Temperature_50hPa { get; set; }

    // Pressure Level Variables - Relative Humidity
    public Dictionary<int, int?[]?>? Relativehumidity_1000hPa { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_925hPa { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_850hPa { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_700hPa { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_600hPa { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_500hPa { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_400hPa { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_300hPa { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_250hPa { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_200hPa { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_150hPa { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_100hPa { get; set; }
    public Dictionary<int, int?[]?>? Relativehumidity_50hPa { get; set; }

    // Pressure Level Variables - Dewpoint
    public Dictionary<int, float?[]?>? Dewpoint_1000hPa { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_925hPa { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_850hPa { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_700hPa { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_600hPa { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_500hPa { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_400hPa { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_300hPa { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_250hPa { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_200hPa { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_150hPa { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_100hPa { get; set; }
    public Dictionary<int, float?[]?>? Dewpoint_50hPa { get; set; }

    // Pressure Level Variables - Cloud Cover
    public Dictionary<int, int?[]?>? Cloudcover_1000hPa { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_925hPa { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_850hPa { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_700hPa { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_600hPa { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_500hPa { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_400hPa { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_300hPa { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_250hPa { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_200hPa { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_150hPa { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_100hPa { get; set; }
    public Dictionary<int, int?[]?>? Cloudcover_50hPa { get; set; }

    // Pressure Level Variables - Wind Speed
    public Dictionary<int, float?[]?>? Windspeed_1000hPa { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_925hPa { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_850hPa { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_700hPa { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_600hPa { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_500hPa { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_400hPa { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_300hPa { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_250hPa { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_200hPa { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_150hPa { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_100hPa { get; set; }
    public Dictionary<int, float?[]?>? Windspeed_50hPa { get; set; }

    // Pressure Level Variables - Wind Direction
    public Dictionary<int, int?[]?>? Winddirection_1000hPa { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_925hPa { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_850hPa { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_700hPa { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_600hPa { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_500hPa { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_400hPa { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_300hPa { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_250hPa { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_200hPa { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_150hPa { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_100hPa { get; set; }
    public Dictionary<int, int?[]?>? Winddirection_50hPa { get; set; }

    // Pressure Level Variables - Geopotential Height
    public Dictionary<int, float?[]?>? Geopotential_height_1000hPa { get; set; }
    public Dictionary<int, float?[]?>? Geopotential_height_925hPa { get; set; }
    public Dictionary<int, float?[]?>? Geopotential_height_850hPa { get; set; }
    public Dictionary<int, float?[]?>? Geopotential_height_700hPa { get; set; }
    public Dictionary<int, float?[]?>? Geopotential_height_600hPa { get; set; }
    public Dictionary<int, float?[]?>? Geopotential_height_500hPa { get; set; }
    public Dictionary<int, float?[]?>? Geopotential_height_400hPa { get; set; }
    public Dictionary<int, float?[]?>? Geopotential_height_300hPa { get; set; }
    public Dictionary<int, float?[]?>? Geopotential_height_250hPa { get; set; }
    public Dictionary<int, float?[]?>? Geopotential_height_200hPa { get; set; }
    public Dictionary<int, float?[]?>? Geopotential_height_150hPa { get; set; }
    public Dictionary<int, float?[]?>? Geopotential_height_100hPa { get; set; }
    public Dictionary<int, float?[]?>? Geopotential_height_50hPa { get; set; }

    // Pressure Level Variables - Vertical Velocity
    public Dictionary<int, float?[]?>? Vertical_velocity_1000hPa { get; set; }
    public Dictionary<int, float?[]?>? Vertical_velocity_925hPa { get; set; }
    public Dictionary<int, float?[]?>? Vertical_velocity_850hPa { get; set; }
    public Dictionary<int, float?[]?>? Vertical_velocity_700hPa { get; set; }
    public Dictionary<int, float?[]?>? Vertical_velocity_600hPa { get; set; }
    public Dictionary<int, float?[]?>? Vertical_velocity_500hPa { get; set; }
    public Dictionary<int, float?[]?>? Vertical_velocity_400hPa { get; set; }
    public Dictionary<int, float?[]?>? Vertical_velocity_300hPa { get; set; }
    public Dictionary<int, float?[]?>? Vertical_velocity_250hPa { get; set; }
    public Dictionary<int, float?[]?>? Vertical_velocity_200hPa { get; set; }
    public Dictionary<int, float?[]?>? Vertical_velocity_150hPa { get; set; }
    public Dictionary<int, float?[]?>? Vertical_velocity_100hPa { get; set; }
    public Dictionary<int, float?[]?>? Vertical_velocity_50hPa { get; set; }
}
