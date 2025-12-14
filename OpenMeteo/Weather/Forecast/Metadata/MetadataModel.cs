using System;

namespace OpenMeteo.Weather.Forecast.Metadata;
public record MetadataModel(
    DateTime DataEndTime,
    DateTime LastRunAvailabilityTime,
    DateTime LastRunInitialisationTime,
    DateTime LastRunModificationTime,
    int TemporalResolutionSeconds,
    int UpdateIntervalSeconds
);
