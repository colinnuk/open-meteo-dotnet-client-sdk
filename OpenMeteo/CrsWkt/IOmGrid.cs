namespace OpenMeteo.CrsWkt;

/// <summary>
/// Interface for Open-Meteo grid implementations providing coordinate lookups
/// and conversions between grid indices and geographic coordinates.
/// </summary>
public interface IOmGrid
{
    /// <summary>Grid shape as (Ny, Nx).</summary>
    (int Ny, int Nx) Shape { get; }

    /// <summary>2D array of latitude coordinates for all grid points, shape (Ny, Nx).</summary>
    double[,] Latitude { get; }

    /// <summary>2D array of longitude coordinates for all grid points, shape (Ny, Nx).</summary>
    double[,] Longitude { get; }

    /// <summary>
    /// Find grid point indices for given lat/lon coordinates.
    /// </summary>
    /// <returns>Grid indices if the point is within bounds, null otherwise.</returns>
    XYIndex? FindPointXY(double lat, double lon);

    /// <summary>
    /// Get lat/lon coordinates for given grid point indices.
    /// </summary>
    LatLon GetCoordinates(int x, int y);
}
