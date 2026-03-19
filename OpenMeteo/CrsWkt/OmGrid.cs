using System;

namespace OpenMeteo.CrsWkt;

/// <summary>
/// Factory wrapper for Open-Meteo grid implementations. Automatically detects
/// the grid type from the WKT string and delegates to either
/// <see cref="RegularGrid"/> or <see cref="GaussianGrid"/>.
/// </summary>
public class OmGrid : IOmGrid
{
    private readonly IOmGrid _grid;

    public OmGrid(ICrsWktParser parser, string crsWkt, (int Ny, int Nx) shape)
    {
        if (!IsValidShape(shape))
            throw new ArgumentException("Shape must be a tuple of two positive integers");

        _grid = parser.IsGaussianGrid(crsWkt)
            ? new GaussianGrid(crsWkt, shape)
            : new RegularGrid(parser, crsWkt, shape);
    }

    /// <inheritdoc />
    public (int Ny, int Nx) Shape => _grid.Shape;

    /// <inheritdoc />
    public double[,] Latitude => _grid.Latitude;

    /// <inheritdoc />
    public double[,] Longitude => _grid.Longitude;

    /// <inheritdoc />
    public XYIndex? FindPointXY(double lat, double lon) => _grid.FindPointXY(lat, lon);

    /// <inheritdoc />
    public LatLon GetCoordinates(int x, int y) => _grid.GetCoordinates(x, y);

    /// <summary>Whether this grid is a Gaussian grid.</summary>
    public bool IsGaussian => _grid is GaussianGrid;

    /// <summary>
    /// Get meshgrid of geographic coordinates. Useful for visualisation.
    /// Returns (longitude, latitude) arrays of shape (Ny, Nx).
    /// </summary>
    public (double[,] Longitude, double[,] Latitude) GetMeshgrid() =>
        (_grid.Longitude, _grid.Latitude);

    private static bool IsValidShape((int Ny, int Nx) shape) =>
        shape.Ny > 0 && shape.Nx > 0;
}
