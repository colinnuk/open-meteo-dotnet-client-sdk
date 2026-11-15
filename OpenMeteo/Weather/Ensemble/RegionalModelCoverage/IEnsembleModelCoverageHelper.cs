using OpenMeteo.Weather.Ensemble.Options;

namespace OpenMeteo.Weather.Ensemble.RegionalModelCoverage;

public interface IEnsembleModelCoverageHelper
{
    /// <summary>
    /// Determines whether a given latitude/longitude point falls within the coverage area
    /// of the specified ensemble model's regional boundary.
    /// Returns <see langword="null"/> when the model has no regional boundary
    /// (e.g. global or seamless models).
    /// </summary>
    bool? IsLocationInModelCoverage(EnsembleModelOptionsParameter ensembleModel, float latitude, float longitude);
}
