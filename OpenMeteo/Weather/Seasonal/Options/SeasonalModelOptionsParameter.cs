namespace OpenMeteo.Weather.Seasonal.Options;

/// <summary>
/// Models available in the Seasonal Forecast API (https://open-meteo.com/en/docs/seasonal-forecast-api)
/// </summary>
public enum SeasonalModelOptionsParameter
{
    /// <summary>ECMWF EC46 for the first 46 days, then SEAS5. All 51 members.</summary>
    ecmwf_seasonal_seamless,

    /// <summary>ECMWF SEAS5 (up to 7 months). All 51 members.</summary>
    ecmwf_seas5,

    /// <summary>ECMWF SEAS5 (up to 7 months). Ensemble mean instead of individual members.</summary>
    ecmwf_seas5_ensemble_mean,

    /// <summary>ECMWF EC46 (46 days). All members.</summary>
    ecmwf_ec46,

    /// <summary>ECMWF EC46 (46 days). Ensemble mean instead of individual members.</summary>
    ecmwf_ec46_ensemble_mean
}
