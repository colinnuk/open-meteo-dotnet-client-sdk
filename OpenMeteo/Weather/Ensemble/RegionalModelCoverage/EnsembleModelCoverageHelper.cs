using OpenMeteo.Weather.Ensemble.Options;
using OpenMeteo.Weather.Forecast.RegionalModelCoverage;

namespace OpenMeteo.Weather.Ensemble.RegionalModelCoverage;

public class EnsembleModelCoverageHelper : IEnsembleModelCoverageHelper
{
    /// <inheritdoc/>
    public bool? IsLocationInModelCoverage(EnsembleModelOptionsParameter ensembleModel, float latitude, float longitude)
    {
        var ring = EnsembleModelPolygons.GetExteriorRing(ensembleModel);
        if (ring == null)
            return null;

        return PolygonContainment.IsPointInRing(ring, latitude, longitude);
    }
}
