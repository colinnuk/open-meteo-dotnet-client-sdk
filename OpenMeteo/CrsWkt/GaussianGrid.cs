using System;

namespace OpenMeteo.CrsWkt;

/// <summary>
/// Implementation of reduced Gaussian grids (O1280, O320, N320, N160) as used by
/// ECMWF IFS and other global models. These grids have varying numbers of longitude
/// points at each latitude line, with more points near the equator and fewer near
/// the poles.
/// </summary>
public class GaussianGrid : IOmGrid
{
    private readonly int _ny;
    private readonly int _nx;
    private readonly string _gridType;
    private readonly int _latitudeLines;
    private readonly int[] _integralTable;

    private double[,]? _latitude;
    private double[,]? _longitude;

    public GaussianGrid(string crsWkt, (int Ny, int Nx) shape)
    {
        _ny = shape.Ny;
        _nx = shape.Nx;

        if (_ny != 1)
            throw new ArgumentException($"Gaussian grid must have Ny=1, got {_ny}");

        _gridType = ParseGridType(crsWkt);
        _latitudeLines = GetLatitudeLines();

        int expectedCount = CalculateTotalPoints();
        if (_nx != expectedCount)
            throw new ArgumentException(
                $"Grid point count mismatch: expected {expectedCount}, got {_nx}");

        _integralTable = BuildIntegralTable();
    }

    /// <inheritdoc />
    public (int Ny, int Nx) Shape => (_ny, _nx);

    /// <summary>The detected Gaussian grid type (e.g. "O1280", "N320").</summary>
    public string GridType => _gridType;

    /// <inheritdoc />
    public double[,] Latitude
    {
        get
        {
            if (_latitude is null)
                ComputeCoordinates();
            return _latitude!;
        }
    }

    /// <inheritdoc />
    public double[,] Longitude
    {
        get
        {
            if (_longitude is null)
                ComputeCoordinates();
            return _longitude!;
        }
    }

    /// <inheritdoc />
    public XYIndex? FindPointXY(double lat, double lon)
    {
        var (xIdx, yIdx) = FindPointXYInternal(lat, lon);
        int gridpoint = _integralTable[yIdx] + xIdx;
        return new XYIndex(gridpoint, 0);
    }

    /// <inheritdoc />
    public LatLon GetCoordinates(int x, int y)
    {
        if (y != 0)
            throw new ArgumentException($"Gaussian grid only has y=0, got y={y}");

        return GetCoordinatesFromGridpoint(x);
    }

    // ── Grid type detection ────────────────────────────────────────────

    private string ParseGridType(string crsWkt)
    {
        if (crsWkt.Contains("O1280", StringComparison.OrdinalIgnoreCase) || _nx == 6599680)
            return "O1280";
        if (crsWkt.Contains("O320", StringComparison.OrdinalIgnoreCase) || _nx == 421120)
            return "O320";
        if (crsWkt.Contains("N320", StringComparison.OrdinalIgnoreCase) || _nx == 542080)
            return "N320";
        if (crsWkt.Contains("N160", StringComparison.OrdinalIgnoreCase) || _nx == 138346)
            return "N160";

        throw new ArgumentException($"Unknown Gaussian grid type with {_nx} points");
    }

    private int GetLatitudeLines() => _gridType switch
    {
        "O1280" => 1280,
        "O320" => 320,
        "N320" => 320,
        "N160" => 160,
        _ => throw new ArgumentException($"Unknown grid type: {_gridType}")
    };

    private int CalculateTotalPoints() => _gridType switch
    {
        "O1280" or "O320" => 4 * _latitudeLines * (_latitudeLines + 9),
        "N320" => 542080,
        "N160" => 138346,
        _ => throw new ArgumentException($"Unknown grid type: {_gridType}")
    };

    // ── Grid point helpers ─────────────────────────────────────────────

    private int NxOfY(int y)
    {
        if (_gridType is "O1280" or "O320")
        {
            return y < _latitudeLines
                ? 20 + y * 4
                : (2 * _latitudeLines - y - 1) * 4 + 20;
        }

        int[] countPerLine = _gridType == "N320"
            ? GaussianGridLookupTables.N320CountPerLine
            : GaussianGridLookupTables.N160CountPerLine;

        return y < _latitudeLines
            ? countPerLine[y]
            : countPerLine[2 * countPerLine.Length - y - 1];
    }

    private int[] BuildIntegralTable()
    {
        int totalLines = 2 * _latitudeLines;
        var table = new int[totalLines + 1];
        for (int y = 0; y < totalLines; y++)
            table[y + 1] = table[y] + NxOfY(y);
        return table;
    }

    private (int y, int x, int nx) GetPos(int gridpoint)
    {
        if (gridpoint < 0 || gridpoint >= _nx)
            throw new ArgumentOutOfRangeException(nameof(gridpoint),
                $"Grid point {gridpoint} out of range [0, {_nx})");

        if (_gridType is "O1280" or "O320")
        {
            int halfCount = _nx / 2;
            int y;

            if (gridpoint < halfCount)
            {
                y = (int)((Math.Sqrt(2.0 * gridpoint + 81) - 9) / 2);
            }
            else
            {
                int gridpointFromEnd = _nx - gridpoint - 1;
                int yFromEnd = (int)((Math.Sqrt(2.0 * gridpointFromEnd + 81) - 9) / 2);
                y = 2 * _latitudeLines - 1 - yFromEnd;
            }

            int x = gridpoint - _integralTable[y];
            int nx = NxOfY(y);
            return (y, x, nx);
        }
        else
        {
            int[] countPerLine = _gridType == "N320"
                ? GaussianGridLookupTables.N320CountPerLine
                : GaussianGridLookupTables.N160CountPerLine;

            int cumsum = 0;
            // Northern hemisphere
            for (int y = 0; y < countPerLine.Length; y++)
            {
                cumsum += countPerLine[y];
                if (gridpoint < cumsum)
                    return (y, gridpoint - (cumsum - countPerLine[y]), countPerLine[y]);
            }

            // Southern hemisphere
            for (int i = 0; i < countPerLine.Length; i++)
            {
                int n = countPerLine[countPerLine.Length - 1 - i];
                cumsum += n;
                if (gridpoint < cumsum)
                {
                    int actualY = i + countPerLine.Length;
                    return (actualY, gridpoint - (cumsum - n), n);
                }
            }

            throw new ArgumentException($"Grid point {gridpoint} not found");
        }
    }

    // ── Coordinate calculations ────────────────────────────────────────

    private LatLon GetCoordinatesFromGridpoint(int gridpoint)
    {
        var (y, x, nx) = GetPos(gridpoint);

        double dy = 180.0 / (2 * _latitudeLines + 0.5);
        double lat = (_latitudeLines - y - 1) * dy + dy / 2;

        double dx = 360.0 / nx;
        double lon = x * dx;

        if (lon >= 180)
            lon -= 360;

        return new LatLon(lat, lon);
    }

    private (int x, int y) FindPointXYInternal(double lat, double lon)
    {
        double dy = 180.0 / (2.0 * _latitudeLines + 0.5);

        double yFloat = _latitudeLines - 1.0 - ((lat - dy / 2.0) / dy);
        int y = Math.Max(0, Math.Min(2 * _latitudeLines - 2, (int)yFloat));
        int yUpper = y + 1;

        int nx = NxOfY(y);
        int nxUpper = NxOfY(yUpper);

        double dx = 360.0 / nx;
        double dxUpper = 360.0 / nxUpper;

        int x = (int)Math.Round(lon / dx);
        int xUpper = (int)Math.Round(lon / dxUpper);

        double pointLat = (_latitudeLines - y - 1) * dy + dy / 2.0;
        double pointLon = x * dx;
        double pointLatUpper = (_latitudeLines - yUpper - 1) * dy + dy / 2.0;
        double pointLonUpper = xUpper * dxUpper;

        double distance = (pointLat - lat) * (pointLat - lat) + (pointLon - lon) * (pointLon - lon);
        double distanceUpper = (pointLatUpper - lat) * (pointLatUpper - lat) + (pointLonUpper - lon) * (pointLonUpper - lon);

        return distance < distanceUpper
            ? (((x % nx) + nx) % nx, y)
            : (((xUpper % nxUpper) + nxUpper) % nxUpper, yUpper);
    }

    private void ComputeCoordinates()
    {
        var lats = new double[_ny, _nx];
        var lons = new double[_ny, _nx];

        for (int gridpoint = 0; gridpoint < _nx; gridpoint++)
        {
            var (lat, lon) = GetCoordinatesFromGridpoint(gridpoint);
            lats[0, gridpoint] = lat;
            lons[0, gridpoint] = lon;
        }

        _latitude = lats;
        _longitude = lons;
    }
}
