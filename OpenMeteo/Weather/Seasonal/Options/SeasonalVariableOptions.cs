using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenMeteo.Weather.Seasonal.Options;

/// <summary>
/// A de-duplicated collection of Seasonal API variables (or models) of one kind.
/// </summary>
/// <typeparam name="T">One of the Seasonal parameter enums</typeparam>
public class SeasonalVariableOptions<T> : ICollection<T> where T : struct, Enum
{
    private readonly List<T> _parameter = [];

    /// <summary>
    /// Gets a new object containing every parameter
    /// </summary>
    public static SeasonalVariableOptions<T> All => new(Enum.GetValues<T>());

    /// <summary>
    /// Gets a copy of the elements contained in the collection
    /// </summary>
    public List<T> Parameter => [.. _parameter];

    public int Count => _parameter.Count;

    public bool IsReadOnly => false;

    public SeasonalVariableOptions()
    {
    }

    public SeasonalVariableOptions(T parameter)
    {
        Add(parameter);
    }

    public SeasonalVariableOptions(IEnumerable<T> parameter)
    {
        foreach (T item in parameter)
            Add(item);
    }

    public T this[int index]
    {
        get => _parameter[index];
        set => _parameter[index] = value;
    }

    public void Add(T item)
    {
        if (_parameter.Contains(item)) return;
        _parameter.Add(item);
    }

    public void Clear() => _parameter.Clear();

    public bool Contains(T item) => _parameter.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => _parameter.CopyTo(array, arrayIndex);

    public bool Remove(T item) => _parameter.Remove(item);

    public IEnumerator<T> GetEnumerator() => _parameter.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
