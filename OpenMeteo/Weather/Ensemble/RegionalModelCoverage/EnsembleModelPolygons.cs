using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Forecast.RegionalModelCoverage;
using System.Collections.Generic;

namespace OpenMeteo.Weather.Ensemble.RegionalModelCoverage;

internal static class EnsembleModelPolygons
{
    // Regional ensembles that run on the same domain as a deterministic model reuse its boundary.
    // Global models have no entry.
    // The ICON-EU and ICON-D2 ensembles run on native grids whose exact shape is not published,
    // so the deterministic ICON boundaries are used as an approximation near the edges.
    private static readonly Dictionary<EnsembleModelOptionsParameter, (double Longitude, double Latitude)[]> _polygons = new()
    {
        [EnsembleModelOptionsParameter.icon_eu] = DwdIconEuCoverage.ExteriorRing,
        [EnsembleModelOptionsParameter.icon_d2] = DwdIconD2Coverage.ExteriorRing,
        [EnsembleModelOptionsParameter.ecmwf_ifs_europe_ensemble] = EcmwfEuropeEnsembleCoverage.ExteriorRing,
        [EnsembleModelOptionsParameter.ecmwf_aifs_europe_ensemble] = EcmwfEuropeEnsembleCoverage.ExteriorRing,
        [EnsembleModelOptionsParameter.ukmo_uk_ensemble_2km] = UkmoUkDeterministic2kmCoverage.ExteriorRing,
        [EnsembleModelOptionsParameter.meteoswiss_icon_ch1] = MeteoswissIconCh1Coverage.ExteriorRing,
        [EnsembleModelOptionsParameter.meteoswiss_icon_ch2] = MeteoswissIconCh2Coverage.ExteriorRing,
        [EnsembleModelOptionsParameter.ncep_rrfs_conus_ensemble] = NcepRrfsConusCoverage.ExteriorRing,
    };

    internal static (double Longitude, double Latitude)[]? GetExteriorRing(EnsembleModelOptionsParameter ensembleModel)
        => _polygons.TryGetValue(ensembleModel, out var ring) ? ring : null;
}
