using OpenMeteo.Weather.Forecast.Options;
using System.Collections.Generic;

namespace OpenMeteo.Weather.Forecast.RegionalModelCoverage;

internal static class RegionalModelPolygons
{
    private static readonly Dictionary<WeatherModelOptionsParameter, (double Longitude, double Latitude)[]> _polygons = new()
    {
        [WeatherModelOptionsParameter.gem_hrdps_continental] = CmcGemHrdpsCoverage.ExteriorRing,
        [WeatherModelOptionsParameter.gem_hrdps_west] = CmcGemHrdpsWestCoverage.ExteriorRing,
        [WeatherModelOptionsParameter.gem_regional] = CmcGemRdpsCoverage.ExteriorRing,
        [WeatherModelOptionsParameter.icon_d2] = DwdIconD2Coverage.ExteriorRing,
        [WeatherModelOptionsParameter.icon_eu] = DwdIconEuCoverage.ExteriorRing,
        [WeatherModelOptionsParameter.italia_meteo_arpae_icon_2i] = ItaliaMeteoArpaeIcon2iCoverage.ExteriorRing,
        [WeatherModelOptionsParameter.jma_msm] = JmaMsmCoverage.ExteriorRing,
        [WeatherModelOptionsParameter.meteofrance_arome_france] = MeteofranceAromeFranceHdCoverage.ExteriorRing,
        [WeatherModelOptionsParameter.meteofrance_arome_france_hd] = MeteofranceAromeFranceHdCoverage.ExteriorRing,
        [WeatherModelOptionsParameter.meteofrance_arpege_europe] = MeteofranceArpegeEuropeCoverage.ExteriorRing,
        [WeatherModelOptionsParameter.meteoswiss_icon_ch1] = MeteoswissIconCh1Coverage.ExteriorRing,
        [WeatherModelOptionsParameter.meteoswiss_icon_ch2] = MeteoswissIconCh2Coverage.ExteriorRing,
        [WeatherModelOptionsParameter.metno_nordic] = MetnoNordicPpCoverage.ExteriorRing,
        [WeatherModelOptionsParameter.gfs_hrrr] = NcepHrrrConusCoverage.ExteriorRing,
        [WeatherModelOptionsParameter.ncep_nam_conus] = NcepNamConusCoverage.ExteriorRing,
        [WeatherModelOptionsParameter.ncep_nbm_conus] = NcepNbmConusCoverage.ExteriorRing,
        [WeatherModelOptionsParameter.ukmo_uk_deterministic_2km] = UkmoUkDeterministic2kmCoverage.ExteriorRing,
        [WeatherModelOptionsParameter.geosphere_arome_austria] = GeosphereAromeAustriaCoverage.ExteriorRing
    };

    internal static (double Longitude, double Latitude)[]? GetExteriorRing(WeatherModelOptionsParameter weatherModel)
        => _polygons.TryGetValue(weatherModel, out var ring) ? ring : null;
}
