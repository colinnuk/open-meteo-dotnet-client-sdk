namespace OpenMeteo.Weather.Metadata;

public record BoundingBox(
    double South,
    double West,
    double North,
    double East
);
