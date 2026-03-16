namespace OpenMeteo.Weather.Metadata;

public record MetadataApiModel
{
    public long data_end_time { get; init; }
    public long last_run_availability_time { get; init; }
    public long last_run_initialisation_time { get; init; }
    public long last_run_modification_time { get; init; }
    public int temporal_resolution_seconds { get; init; }
    public int update_interval_seconds { get; init; }
    public string crs_wkt { get; init; } = string.Empty;
}

