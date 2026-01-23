using OpenMeteo.Weather.Forecast.Options;
using System;

namespace OpenMeteo.Weather.Forecast.Metadata;
internal static class MetadataNameHelper
{
    public static string GetMetadataUrlName(WeatherModelOptionsParameter weatherModel) => weatherModel switch
    {
        WeatherModelOptionsParameter.ecmwf_ifs => "ecmwf_ifs",
        WeatherModelOptionsParameter.ecmwf_ifs025 => "ecmwf_ifs025",
        WeatherModelOptionsParameter.ecmwf_aifs025_single => "ecmwf_aifs025_single",
        WeatherModelOptionsParameter.icon_global => "dwd_icon",
        WeatherModelOptionsParameter.icon_eu => "dwd_icon_eu",
        WeatherModelOptionsParameter.icon_d2 => "dwd_icon_d2",
        WeatherModelOptionsParameter.meteofrance_arpege_world => "meteofrance_arpege_world025",
        WeatherModelOptionsParameter.meteofrance_arpege_europe => "meteofrance_arpege_europe",
        WeatherModelOptionsParameter.meteofrance_arome_france => "meteofrance_arome_france0025",
        WeatherModelOptionsParameter.meteofrance_arome_france_hd => "meteofrance_arome_france_hd",
        WeatherModelOptionsParameter.ukmo_uk_deterministic_2km => "ukmo_uk_deterministic_2km",
        WeatherModelOptionsParameter.ukmo_global_deterministic_10km => "ukmo_global_deterministic_10km",
        WeatherModelOptionsParameter.gfs_global => "ncep_gfs013",
        WeatherModelOptionsParameter.gfs_graphcast025 => "ncep_gfs_graphcast025",
        WeatherModelOptionsParameter.gfs_hrrr => "ncep_hrrr_conus",
        WeatherModelOptionsParameter.ncep_aigfs025 => "ncep_aigfs025",
        WeatherModelOptionsParameter.ncep_hgefs025_stats => "ncep_hgefs025_stats",
        WeatherModelOptionsParameter.ncep_nbm_conus => "ncep_nbm_conus",
        WeatherModelOptionsParameter.ncep_nam_conus => "ncep_nam_conus",
        WeatherModelOptionsParameter.gem_global => "cmc_gem_gdps",
        WeatherModelOptionsParameter.gem_hrdps_continental => "cmc_gem_hrdps",
        WeatherModelOptionsParameter.gem_hrdps_west => "cmc_gem_hrdps_west",
        WeatherModelOptionsParameter.gem_regional => "cmc_gem_rdps",
        WeatherModelOptionsParameter.jma_gsm => "jma_gsm",
        WeatherModelOptionsParameter.jma_msm => "jma_msm",
        WeatherModelOptionsParameter.metno_nordic => "metno_nordic_pp",
        WeatherModelOptionsParameter.bom_access_global => "bom_access_global",
        WeatherModelOptionsParameter.italia_meteo_arpae_icon_2i => "italia_meteo_arpae_icon_2i",
        WeatherModelOptionsParameter.meteoswiss_icon_ch1 => "meteoswiss_icon_ch1",
        WeatherModelOptionsParameter.meteoswiss_icon_ch2 => "meteoswiss_icon_ch2",
        _ => throw new ArgumentOutOfRangeException(nameof(weatherModel), weatherModel, "No mapping specified for weather model name to the metadata URL operation. Unable to get metadata for this model.")
    };
}
