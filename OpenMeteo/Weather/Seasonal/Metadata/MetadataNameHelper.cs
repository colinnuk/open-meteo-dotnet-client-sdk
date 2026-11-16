using OpenMeteo.Weather.Seasonal.Options;
using System;

namespace OpenMeteo.Weather.Seasonal.Metadata;
internal static class MetadataNameHelper
{
    public static string GetMetadataUrlName(SeasonalModelOptionsParameter weatherModel) => weatherModel switch
    {
        // ECMWF SEAS5 (up to 7 months)
        SeasonalModelOptionsParameter.ecmwf_seas5 => "ecmwf_seas5",
        SeasonalModelOptionsParameter.ecmwf_seas5_ensemble_mean => "ecmwf_seas5_ensemble_mean",

        // ECMWF EC46 (46 days)
        SeasonalModelOptionsParameter.ecmwf_ec46 => "ecmwf_ec46",
        SeasonalModelOptionsParameter.ecmwf_ec46_ensemble_mean => "ecmwf_ec46_ensemble_mean",

        // ecmwf_seasonal_seamless is a combination of EC46 and SEAS5 and publishes no metadata of its own
        // (see https://open-meteo.com/en/docs/model-updates)
        _ => throw new ArgumentOutOfRangeException(nameof(weatherModel), weatherModel, "No mapping specified for weather model name to the metadata URL operation. Unable to get metadata for this model.")
    };
}
