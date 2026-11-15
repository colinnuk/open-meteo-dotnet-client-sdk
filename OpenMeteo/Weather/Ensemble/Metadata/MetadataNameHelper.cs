using OpenMeteo.Weather.Ensemble.Options;
using System;

namespace OpenMeteo.Weather.Ensemble.Metadata;
internal static class MetadataNameHelper
{
    public static string GetMetadataUrlName(EnsembleModelOptionsParameter weatherModel) => weatherModel switch
    {
        // DWD ICON Ensemble Models
        // ICON ensembles have run on their native grids since October 2026
        EnsembleModelOptionsParameter.icon_seamless => "dwd_icon_global_eps_native",
        EnsembleModelOptionsParameter.icon_global => "dwd_icon_global_eps_native",
        EnsembleModelOptionsParameter.icon_eu => "dwd_icon_eu_eps_native",
        EnsembleModelOptionsParameter.icon_d2 => "dwd_icon_d2_eps_native",
        
        // NOAA GFS Ensemble Models
        EnsembleModelOptionsParameter.gfs_seamless => "ncep_gefs05",
        EnsembleModelOptionsParameter.gfs025 => "ncep_gefs025",
        EnsembleModelOptionsParameter.gfs05 => "ncep_gefs05",
        
        // NOAA AI-based Ensemble Model
        EnsembleModelOptionsParameter.ncep_aigefs025 => "ncep_aigefs025",

        // Google WeatherNext2 Ensemble Model
        EnsembleModelOptionsParameter.google_weathernext2_ensemble => "google_weathernext2_ensemble",

        // ECMWF Ensemble Models
        EnsembleModelOptionsParameter.ecmwf_ifs025 => "ecmwf_ifs025_ensemble",
        EnsembleModelOptionsParameter.ecmwf_aifs025 => "ecmwf_aifs025_ensemble",

        // Canadian GEM Ensemble
        EnsembleModelOptionsParameter.gem_global => "cmc_gem_geps",
        
        // UK Met Office Ensemble Models
        EnsembleModelOptionsParameter.ukmo_global_ensemble_20km => "ukmo_global_ensemble_20km",
        EnsembleModelOptionsParameter.ukmo_uk_ensemble_2km => "ukmo_uk_ensemble_2km",
        
        // MeteoSwiss ICON Ensemble Models
        EnsembleModelOptionsParameter.meteoswiss_icon_ch1 => "meteoswiss_icon_ch1_ensemble",
        EnsembleModelOptionsParameter.meteoswiss_icon_ch2 => "meteoswiss_icon_ch2_ensemble",

        // ncep_rrfs_conus_ensemble, ecmwf_ifs_europe_ensemble and ecmwf_aifs_europe_ensemble publish no metadata
        // (see https://open-meteo.com/en/docs/model-updates)
        _ => throw new ArgumentOutOfRangeException(nameof(weatherModel), weatherModel, "No mapping specified for weather model name to the metadata URL operation. Unable to get metadata for this model.")
    };
}
