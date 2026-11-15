namespace OpenMeteo.Weather.Ensemble.RegionalModelCoverage;

/// <summary>
/// Domain of the ECMWF IFS and AIFS Europe ensembles.
/// </summary>
internal static class EcmwfEuropeEnsembleCoverage
{
    internal static readonly (double Longitude, double Latitude)[] ExteriorRing =
    [
        (-11.0, 33.005),
        (37.0, 33.005),
        (37.0, 70.967),
        (-11.0, 70.967),
        (-11.0, 33.005),
    ];
}
