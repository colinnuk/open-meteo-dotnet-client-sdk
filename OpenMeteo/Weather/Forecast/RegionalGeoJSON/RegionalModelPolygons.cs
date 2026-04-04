using System.Collections.Generic;

namespace OpenMeteo.Weather.Forecast.RegionalGeoJSON;

internal static class RegionalModelPolygons
{
    private static readonly Dictionary<string, (double Longitude, double Latitude)[]> _polygons = new()
    {
        ["cmc_gem_hrdps"] = CmcGemHrdpsCoverage.ExteriorRing,
        ["cmc_gem_hrdps_west"] = CmcGemHrdpsWestCoverage.ExteriorRing,
        ["cmc_gem_rdps"] = CmcGemRdpsCoverage.ExteriorRing,
        ["dwd_icon_d2"] = DwdIconD2Coverage.ExteriorRing,
        ["dwd_icon_eu"] = DwdIconEuCoverage.ExteriorRing,
        ["italia_meteo_arpae_icon_2i"] = ItaliaMeteoArpaeIcon2iCoverage.ExteriorRing,
        ["jma_msm"] = JmaMsmCoverage.ExteriorRing,
        ["meteofrance_arome_france_hd"] = MeteofranceAromeFranceHdCoverage.ExteriorRing,
        ["meteofrance_arpege_europe"] = MeteofranceArpegeEuropeCoverage.ExteriorRing,
        ["meteoswiss_icon_ch1"] = MeteoswissIconCh1Coverage.ExteriorRing,
        ["meteoswiss_icon_ch2"] = MeteoswissIconCh2Coverage.ExteriorRing,
        ["metno_nordic_pp"] = MetnoNordicPpCoverage.ExteriorRing,
        ["ncep_hrrr_conus"] = NcepHrrrConusCoverage.ExteriorRing,
        ["ncep_nam_conus"] = NcepNamConusCoverage.ExteriorRing,
        ["ncep_nbm_conus"] = NcepNbmConusCoverage.ExteriorRing,
        ["ukmo_uk_deterministic_2km"] = UkmoUkDeterministic2kmCoverage.ExteriorRing,
    };

    internal static (double Longitude, double Latitude)[]? GetExteriorRing(string metaName)
        => _polygons.TryGetValue(metaName, out var ring) ? ring : null;
}
