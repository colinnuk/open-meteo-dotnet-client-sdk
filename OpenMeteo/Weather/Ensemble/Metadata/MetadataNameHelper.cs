using OpenMeteo.Weather.Ensemble.Options;
using System;

namespace OpenMeteo.Weather.Ensemble.Metadata;
internal static class MetadataNameHelper
{
    public static string GetMetadataUrlName(EnsembleModelOptionsParameter weatherModel) => weatherModel switch
    {
        // DWD ICON Ensemble Models
        EnsembleModelOptionsParameter.icon_seamless => "dwd_icon_eps",
        EnsembleModelOptionsParameter.icon_global => "dwd_icon_eps",
        EnsembleModelOptionsParameter.icon_eu => "dwd_icon_eu_eps",
        EnsembleModelOptionsParameter.icon_d2 => "dwd_icon_d2_eps",
        
        // NOAA GFS Ensemble Models
        EnsembleModelOptionsParameter.gfs_seamless => "ncep_gefs05",
        EnsembleModelOptionsParameter.gfs025 => "ncep_gefs025",
        EnsembleModelOptionsParameter.gfs05 => "ncep_gefs05",
        
        // ECMWF Ensemble Models
        EnsembleModelOptionsParameter.ecmwf_ifs025 => "ecmwf_ifs025_ensemble",
        EnsembleModelOptionsParameter.ecmwf_aifs025 => "ecmwf_aifs025_ensemble",
        
        // Canadian GEM Ensemble
        EnsembleModelOptionsParameter.gem_global => "cmc_gem_geps",
        
        // BOM ACCESS Ensemble
        EnsembleModelOptionsParameter.bom_access_global_ensemble => "bom_access_global_ensemble",
        
        // UK Met Office Ensemble Models
        EnsembleModelOptionsParameter.ukmo_global_ensemble_20km => "ukmo_global_ensemble_20km",
        EnsembleModelOptionsParameter.ukmo_uk_ensemble_2km => "ukmo_uk_ensemble_2km",
        
        // MeteoSwiss ICON Ensemble Models
        EnsembleModelOptionsParameter.meteoswiss_icon_ch1 => "meteoswiss_icon_ch1_ensemble",
        EnsembleModelOptionsParameter.meteoswiss_icon_ch2 => "meteoswiss_icon_ch2_ensemble",

        _ => throw new ArgumentOutOfRangeException(nameof(weatherModel), weatherModel, "No mapping specified for weather model name to the metadata URL operation. Unable to get metadata for this model.")
    };
}
