using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenMeteo.Weather.Ensemble.Options;

/// <summary>
/// Ensemble Model Options (https://open-meteo.com/en/docs/ensemble-api)
/// </summary>
public class EnsembleModelOptions : IEnumerable<EnsembleModelOptionsParameter>, ICollection<EnsembleModelOptionsParameter>
{
    /// <summary>
    /// Applying every parameter
    /// </summary>
    public static EnsembleModelOptions All { get { return new EnsembleModelOptions((EnsembleModelOptionsParameter[])Enum.GetValues(typeof(EnsembleModelOptionsParameter))); } }

    /// <summary>
    /// A copy of the current applied parameter. This is a COPY. Editing anything inside this copy won't be applied 
    /// </summary>
    public List<EnsembleModelOptionsParameter> Parameter { get { return new List<EnsembleModelOptionsParameter>(_parameter); } }

    public int Count => _parameter.Count;

    public bool IsReadOnly => false;

    private readonly List<EnsembleModelOptionsParameter> _parameter;

    public EnsembleModelOptions(EnsembleModelOptionsParameter parameter)
    {
        _parameter = [];
        Add(parameter);
    }

    public EnsembleModelOptions(EnsembleModelOptionsParameter[] parameter)
    {
        _parameter = [];
        Add(parameter);
    }

    public EnsembleModelOptions()
    {
        _parameter = [];
    }

    public EnsembleModelOptionsParameter this[int index]
    {
        get { return _parameter[index]; }
        set
        {
            _parameter[index] = value;
        }
    }

    public void Add(EnsembleModelOptionsParameter param)
    {
        if (_parameter.Contains(param)) return;
        _parameter.Add(param);
    }

    public void Add(EnsembleModelOptionsParameter[] parameters)
    {
        foreach (EnsembleModelOptionsParameter paramToAdd in parameters)
        {
            Add(paramToAdd);
        }
    }

    public IEnumerator<EnsembleModelOptionsParameter> GetEnumerator()
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

    public bool Contains(EnsembleModelOptionsParameter item)
    {
        return _parameter.Contains(item);
    }

    public void CopyTo(EnsembleModelOptionsParameter[] array, int arrayIndex)
    {
        _parameter.CopyTo(array, arrayIndex);
    }

    public bool Remove(EnsembleModelOptionsParameter item)
    {
        return _parameter.Remove(item);
    }
}

public enum EnsembleModelOptionsParameter
{
    icon_seamless,
    icon_global,
    icon_eu,
    icon_d2,
    gfs_seamless,
    gfs025,
    gfs05,
    ecmwf_ifs025,
    ecmwf_aifs025,
    gem_global,
    bom_access_global_ensemble,
    ukmo_global_ensemble_20km,
    ukmo_uk_ensemble_2km,
    meteoswiss_icon_ch1,
    meteoswiss_icon_ch2,
    ncep_aigefs025
}
