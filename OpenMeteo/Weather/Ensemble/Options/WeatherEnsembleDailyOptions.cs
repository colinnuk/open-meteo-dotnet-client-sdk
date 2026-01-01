using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenMeteo.Weather.Ensemble.Options;

/// <summary>
/// Daily Weather Variables for Ensemble API (https://open-meteo.com/en/docs/ensemble-api)
/// </summary>
public class WeatherEnsembleDailyOptions : IEnumerable, ICollection<WeatherEnsembleDailyOptionsParameter>
{
    /// <summary>
    /// Gets a new object containing every parameter
    /// </summary>
    /// <returns></returns>
    public static WeatherEnsembleDailyOptions All { get { return new WeatherEnsembleDailyOptions((WeatherEnsembleDailyOptionsParameter[])Enum.GetValues(typeof(WeatherEnsembleDailyOptionsParameter))); } }
    
    /// <summary>
    /// Gets a copy of elements contained in the List.
    /// </summary>
    /// <typeparam name="WeatherEnsembleDailyOptionsParameter"></typeparam>
    /// <returns>A copy of elements contained in the List</returns>
    public List<WeatherEnsembleDailyOptionsParameter> Parameter { get { return new List<WeatherEnsembleDailyOptionsParameter>(_parameter); } }

    public int Count => _parameter.Count;

    public bool IsReadOnly => false;

    private readonly List<WeatherEnsembleDailyOptionsParameter> _parameter = new List<WeatherEnsembleDailyOptionsParameter>();

    public WeatherEnsembleDailyOptions()
    {
    }

    public WeatherEnsembleDailyOptions(WeatherEnsembleDailyOptionsParameter parameter)
    {
        Add(parameter);
    }

    public WeatherEnsembleDailyOptions(WeatherEnsembleDailyOptionsParameter[] parameter)
    {
        Add(parameter);
    }

    /// <summary>
    /// Index the collection
    /// </summary>
    /// <param name="index"></param>
    /// <returns><see cref="string"/> WeatherEnsembleDailyOptionsParameter as string representation at index</returns>
    public WeatherEnsembleDailyOptionsParameter this[int index]
    {
        get { return _parameter[index]; }
        set
        {
            _parameter[index] = value;
        }
    }

    public void Add(WeatherEnsembleDailyOptionsParameter param)
    {
        if (_parameter.Contains(param)) return;
        _parameter.Add(param);
    }

    public void Add(WeatherEnsembleDailyOptionsParameter[] param)
    {
        foreach (WeatherEnsembleDailyOptionsParameter paramToAdd in param)
        {
            Add(paramToAdd);
        }
    }

    public void Clear()
    {
        _parameter.Clear();
    }

    public bool Contains(WeatherEnsembleDailyOptionsParameter item)
    {
        return _parameter.Contains(item);
    }

    public bool Remove(WeatherEnsembleDailyOptionsParameter item)
    {
        return _parameter.Remove(item);
    }

    public void CopyTo(WeatherEnsembleDailyOptionsParameter[] array, int arrayIndex)
    {
        _parameter.CopyTo(array, arrayIndex);
    }

    public IEnumerator<WeatherEnsembleDailyOptionsParameter> GetEnumerator()
    {
        return _parameter.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public enum WeatherEnsembleDailyOptionsParameter
{
    temperature_2m_mean,
    temperature_2m_min,
    temperature_2m_max,
    apparent_temperature_mean,
    apparent_temperature_min,
    apparent_temperature_max,
    windspeed_10m_mean,
    windspeed_10m_min,
    windspeed_10m_max,
    winddirection_10m_dominant,
    windgusts_10m_mean,
    windgusts_10m_min,
    windgusts_10m_max,
    windspeed_100m_mean,
    windspeed_100m_min,
    windspeed_100m_max,
    winddirection_100m_dominant,
    cloudcover_mean,
    cloudcover_min,
    cloudcover_max,
    precipitation_sum,
    precipitation_hours,
    rain_sum,
    snowfall_sum,
    pressure_msl_mean,
    pressure_msl_min,
    pressure_msl_max,
    surface_pressure_mean,
    surface_pressure_min,
    surface_pressure_max,
    relativehumidity_2m_mean,
    relativehumidity_2m_min,
    relativehumidity_2m_max,
    cape_mean,
    cape_min,
    cape_max,
    dewpoint_2m_mean,
    dewpoint_2m_min,
    dewpoint_2m_max,
    et0_fao_evapotranspiration,
    shortwave_radiation_sum
}
