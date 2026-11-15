using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Ensemble.Units;

namespace OpenMeteo.Weather.Ensemble.ResponseModel.Conversion;

/// <summary>
/// Splits a JSON response for several ensemble models into one response per model.
/// </summary>
/// <remarks>
/// When more than one model is requested, the API adds the model name to every key,
/// for example <c>temperature_2m_member01_ncep_gefs025</c>. The suffix is the API's own name for the model,
/// which can differ from the requested name (<c>gfs025</c> becomes <c>ncep_gefs025</c>),
/// so the models are matched by the order in which they appear, which is the order they were requested in.
/// </remarks>
internal static class EnsembleModelSplitter
{
    private static readonly Regex _memberPattern = new(@"^member(\d+)_(.+)$", RegexOptions.Compiled);

    public static IReadOnlyList<WeatherEnsembleDto> Split(WeatherEnsembleDto dto, WeatherEnsembleOptions options)
    {
        var hourlyVariables = options.Hourly.Parameter.Select(p => p.ToString()).ToList();
        var dailyVariables = options.Daily.Parameter.Select(p => p.ToString()).ToList();

        var suffixes = new List<string>();
        var hourly = SplitData(dto.Hourly?.AdditionalData, hourlyVariables, suffixes);
        var daily = SplitData(dto.Daily?.AdditionalData, dailyVariables, suffixes);
        var hourlyUnits = SplitData(dto.HourlyUnits?.AdditionalData, hourlyVariables, suffixes);
        var dailyUnits = SplitData(dto.DailyUnits?.AdditionalData, dailyVariables, suffixes);

        return suffixes.Select(suffix => new WeatherEnsembleDto
        {
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Elevation = dto.Elevation,
            GenerationTime = dto.GenerationTime,
            UtcOffset = dto.UtcOffset,
            Timezone = dto.Timezone,
            TimezoneAbbreviation = dto.TimezoneAbbreviation,
            Hourly = dto.Hourly == null ? null : new WeatherEnsembleHourlyDto { Time = dto.Hourly.Time, AdditionalData = hourly.GetValueOrDefault(suffix) ?? [] },
            Daily = dto.Daily == null ? null : new WeatherEnsembleDailyDto { Time = dto.Daily.Time, AdditionalData = daily.GetValueOrDefault(suffix) ?? [] },
            HourlyUnits = dto.HourlyUnits == null ? null : new WeatherEnsembleHourlyUnits { Time = dto.HourlyUnits.Time, AdditionalData = hourlyUnits.GetValueOrDefault(suffix) ?? [] },
            DailyUnits = dto.DailyUnits == null ? null : new WeatherEnsembleDailyUnits { Time = dto.DailyUnits.Time, AdditionalData = dailyUnits.GetValueOrDefault(suffix) ?? [] },
        }).ToList();
    }

    /// <summary>
    /// Groups the keys of <paramref name="data"/> by model suffix and removes the suffix from each key.
    /// Suffixes not seen before are added to <paramref name="suffixes"/> in order of appearance.
    /// </summary>
    private static Dictionary<string, Dictionary<string, object>> SplitData(
        Dictionary<string, object>? data, List<string> variables, List<string> suffixes)
    {
        var result = new Dictionary<string, Dictionary<string, object>>();
        if (data == null)
            return result;

        foreach (var (key, value) in data)
        {
            if (!TrySplitKey(key, variables, out var suffix, out var newKey))
                continue;

            if (!suffixes.Contains(suffix))
                suffixes.Add(suffix);
            if (!result.TryGetValue(suffix, out var group))
                result[suffix] = group = [];
            group[newKey] = value;
        }

        return result;
    }

    internal static bool TrySplitKey(string key, IEnumerable<string> variables, out string suffix, out string newKey)
    {
        suffix = newKey = string.Empty;

        // Longest match first, so temperature_2m_max is not read as temperature_2m with suffix max_...
        var variable = variables
            .Where(v => key.Length > v.Length + 1 && key.StartsWith(v + "_", System.StringComparison.Ordinal))
            .OrderByDescending(v => v.Length)
            .FirstOrDefault();
        if (variable == null)
            return false;

        var rest = key[(variable.Length + 1)..];
        var memberMatch = _memberPattern.Match(rest);
        if (memberMatch.Success)
        {
            suffix = memberMatch.Groups[2].Value;
            newKey = $"{variable}_member{memberMatch.Groups[1].Value}";
        }
        else
        {
            suffix = rest;
            newKey = variable;
        }
        return true;
    }
}
