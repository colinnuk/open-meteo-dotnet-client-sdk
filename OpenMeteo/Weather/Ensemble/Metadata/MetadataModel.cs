using System;

namespace OpenMeteo.Weather.Ensemble.Metadata;
public record MetadataModel(
    DateTime DataEndTime,
    DateTime LastRunAvailabilityTime,
    DateTime LastRunInitialisationTime,
    DateTime LastRunModificationTime,
    int TemporalResolutionSeconds,
    int UpdateIntervalSeconds
);
