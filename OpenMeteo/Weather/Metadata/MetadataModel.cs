using System;

namespace OpenMeteo.Weather.Metadata;
public record MetadataModel(
    DateTime DataEndTime,
    DateTime LastRunAvailabilityTime,
    DateTime LastRunInitialisationTime,
    DateTime LastRunModificationTime,
    int TemporalResolutionSeconds,
    int UpdateIntervalSeconds
);
