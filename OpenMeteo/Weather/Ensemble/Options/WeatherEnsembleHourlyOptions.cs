using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenMeteo.Weather.Ensemble.Options;

/// <summary>
/// Hourly Weather Variables for Ensemble API (https://open-meteo.com/en/docs/ensemble-api)
/// </summary>
public class WeatherEnsembleHourlyOptions : IEnumerable<WeatherEnsembleHourlyOptionsParameter>, ICollection<WeatherEnsembleHourlyOptionsParameter>
{
    public static WeatherEnsembleHourlyOptions All { get { return new WeatherEnsembleHourlyOptions((WeatherEnsembleHourlyOptionsParameter[])Enum.GetValues(typeof(WeatherEnsembleHourlyOptionsParameter))); } }

    /// <summary>
    /// A copy of the current applied parameter. This is a COPY. Editing anything inside this copy won't be applied 
    /// </summary>
    public List<WeatherEnsembleHourlyOptionsParameter> Parameter { get { return new List<WeatherEnsembleHourlyOptionsParameter>(_parameter); } }

    public int Count => _parameter.Count;

    public bool IsReadOnly => false;

    private readonly List<WeatherEnsembleHourlyOptionsParameter> _parameter;

    public WeatherEnsembleHourlyOptions(WeatherEnsembleHourlyOptionsParameter parameter)
    {
        _parameter = [];
        Add(parameter);
    }

    public WeatherEnsembleHourlyOptions(WeatherEnsembleHourlyOptionsParameter[] parameter)
    {
        _parameter = [];
        Add(parameter);
    }

    public WeatherEnsembleHourlyOptions()
    {
        _parameter = [];
    }

    public WeatherEnsembleHourlyOptionsParameter this[int index]
    {
        get { return _parameter[index]; }
        set
        {
            _parameter[index] = value;
        }
    }

    public void Add(WeatherEnsembleHourlyOptionsParameter param)
    {
        if (_parameter.Contains(param)) return;
        _parameter.Add(param);
    }

    public void Add(WeatherEnsembleHourlyOptionsParameter[] param)
    {
        foreach (WeatherEnsembleHourlyOptionsParameter paramToAdd in param)
        {
            Add(paramToAdd);
        }
    }

    public IEnumerator<WeatherEnsembleHourlyOptionsParameter> GetEnumerator()
    {
        return _parameter.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Clear()
    {
        _parameter.Clear();
    }

    public bool Contains(WeatherEnsembleHourlyOptionsParameter item)
    {
        return _parameter.Contains(item);
    }

    public void CopyTo(WeatherEnsembleHourlyOptionsParameter[] array, int arrayIndex)
    {
        _parameter.CopyTo(array, arrayIndex);
    }

    public bool Remove(WeatherEnsembleHourlyOptionsParameter item)
    {
        return _parameter.Remove(item);
    }
}

public enum WeatherEnsembleHourlyOptionsParameter
{
    temperature_2m,
    relativehumidity_2m,
    dewpoint_2m,
    apparent_temperature,
    precipitation,
    rain,
    snowfall,
    snow_depth,
    weathercode,
    pressure_msl,
    surface_pressure,
    cloudcover,
    cloudcover_low,
    cloudcover_mid,
    cloudcover_high,
    visibility,
    et0_fao_evapotranspiration,
    vapor_pressure_deficit,
    windspeed_10m,
    windspeed_80m,
    windspeed_100m,
    windspeed_120m,
    winddirection_10m,
    winddirection_80m,
    winddirection_100m,
    winddirection_120m,
    windgusts_10m,
    temperature_80m,
    temperature_120m,
    surface_temperature,
    soil_temperature_0_to_10cm,
    soil_temperature_10_to_40cm,
    soil_temperature_40_to_100cm,
    soil_temperature_100_to_200cm,
    soil_moisture_0_to_10cm,
    soil_moisture_10_to_40cm,
    soil_moisture_40_to_100cm,
    soil_moisture_100_to_255cm,
    soil_temperature_0_to_7cm,
    soil_temperature_7_to_28cm,
    soil_temperature_28_to_100cm,
    soil_temperature_100_to_255cm,
    soil_moisture_0_to_7cm,
    soil_moisture_7_to_28cm,
    soil_moisture_28_to_100cm,
    soil_moisture_100_to_400cm,
    uv_index,
    uv_index_clear_sky,
    temperature_2m_min,
    temperature_2m_max,
    wet_bulb_temperature_2m,
    cape,
    convective_inhibition,
    freezing_level_height,
    snowfall_height,
    sunshine_duration,
    snowfall_water_equivalent,
    snow_depth_water_equivalent,
    shortwave_radiation,
    direct_radiation,
    diffuse_radiation,
    direct_normal_irradiance,
    global_tilted_irradiance,
    shortwave_radiation_instant,
    direct_radiation_instant,
    diffuse_radiation_instant,
    direct_normal_irradiance_instant,
    global_tilted_irradiance_instant,
    temperature_1000hPa,
    temperature_925hPa,
    temperature_850hPa,
    temperature_700hPa,
    temperature_600hPa,
    temperature_500hPa,
    temperature_400hPa,
    temperature_300hPa,
    temperature_250hPa,
    temperature_200hPa,
    temperature_150hPa,
    temperature_100hPa,
    temperature_50hPa,
    relativehumidity_1000hPa,
    relativehumidity_925hPa,
    relativehumidity_850hPa,
    relativehumidity_700hPa,
    relativehumidity_600hPa,
    relativehumidity_500hPa,
    relativehumidity_400hPa,
    relativehumidity_300hPa,
    relativehumidity_250hPa,
    relativehumidity_200hPa,
    relativehumidity_150hPa,
    relativehumidity_100hPa,
    relativehumidity_50hPa,
    dewpoint_1000hPa,
    dewpoint_925hPa,
    dewpoint_850hPa,
    dewpoint_700hPa,
    dewpoint_600hPa,
    dewpoint_500hPa,
    dewpoint_400hPa,
    dewpoint_300hPa,
    dewpoint_250hPa,
    dewpoint_200hPa,
    dewpoint_150hPa,
    dewpoint_100hPa,
    dewpoint_50hPa,
    cloudcover_1000hPa,
    cloudcover_925hPa,
    cloudcover_850hPa,
    cloudcover_700hPa,
    cloudcover_600hPa,
    cloudcover_500hPa,
    cloudcover_400hPa,
    cloudcover_300hPa,
    cloudcover_250hPa,
    cloudcover_200hPa,
    cloudcover_150hPa,
    cloudcover_100hPa,
    cloudcover_50hPa,
    windspeed_1000hPa,
    windspeed_925hPa,
    windspeed_850hPa,
    windspeed_700hPa,
    windspeed_600hPa,
    windspeed_500hPa,
    windspeed_400hPa,
    windspeed_300hPa,
    windspeed_250hPa,
    windspeed_200hPa,
    windspeed_150hPa,
    windspeed_100hPa,
    windspeed_50hPa,
    winddirection_1000hPa,
    winddirection_925hPa,
    winddirection_850hPa,
    winddirection_700hPa,
    winddirection_600hPa,
    winddirection_500hPa,
    winddirection_400hPa,
    winddirection_300hPa,
    winddirection_250hPa,
    winddirection_200hPa,
    winddirection_150hPa,
    winddirection_100hPa,
    winddirection_50hPa,
    geopotential_height_1000hPa,
    geopotential_height_925hPa,
    geopotential_height_850hPa,
    geopotential_height_700hPa,
    geopotential_height_600hPa,
    geopotential_height_500hPa,
    geopotential_height_400hPa,
    geopotential_height_300hPa,
    geopotential_height_250hPa,
    geopotential_height_200hPa,
    geopotential_height_150hPa,
    geopotential_height_100hPa,
    geopotential_height_50hPa,
    vertical_velocity_1000hPa,
    vertical_velocity_925hPa,
    vertical_velocity_850hPa,
    vertical_velocity_700hPa,
    vertical_velocity_600hPa,
    vertical_velocity_500hPa,
    vertical_velocity_400hPa,
    vertical_velocity_300hPa,
    vertical_velocity_250hPa,
    vertical_velocity_200hPa,
    vertical_velocity_150hPa,
    vertical_velocity_100hPa,
    vertical_velocity_50hPa
}
