namespace OpenMeteo.Weather.Metadata;

public record BoundingBox(
    decimal South,
    decimal West,
    decimal North,
    decimal East
);
