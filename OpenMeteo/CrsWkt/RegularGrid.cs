using System;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace OpenMeteo.CrsWkt;

/// <summary>
/// Regular latitude/longitude or projected grid.
/// Uses ProjNet for coordinate transformations between the grid's CRS and WGS 84.
/// </summary>
public class RegularGrid : IOmGrid
{
    private readonly MathTransform? _toProjection;
    private readonly MathTransform? _toWgs84;
    private readonly bool _isProjected;
    private readonly double _xMin;
    private readonly double _yMin;
    private readonly double _xMax;
    private readonly double _yMax;
    private readonly double _dx;
    private readonly double _dy;
    private readonly int _ny;
    private readonly int _nx;

    private double[,]? _latitude;
    private double[,]? _longitude;

    public RegularGrid(string crsWkt, (int Ny, int Nx) shape)
    {
        _ny = shape.Ny;
        _nx = shape.Nx;

        if (_nx <= 1 || _ny <= 1)
            throw new ArgumentException("Invalid grid shape");

        var bbox = CrsWktParser.ParseBoundingBox(crsWkt)
            ?? throw new ArgumentException("WKT does not contain BBOX");

        _isProjected = CrsWktParser.IsProjectedCrs(crsWkt);

        if (_isProjected)
        {
            var crs = CrsWktParser.ParseCoordinateSystem(crsWkt)
                ?? throw new ArgumentException("Unable to parse projected CRS from WKT");

            var wgs84 = GeographicCoordinateSystem.WGS84;
            var ctFactory = new CoordinateTransformationFactory();

            _toProjection = ctFactory.CreateFromCoordinateSystems(wgs84, crs).MathTransform;
            _toWgs84 = ctFactory.CreateFromCoordinateSystems(crs, wgs84).MathTransform;

            var (pxMin, pyMin) = _toProjection.Transform((double)bbox.West, (double)bbox.South);
            var (pxMax, pyMax) = _toProjection.Transform((double)bbox.East, (double)bbox.North);

            _xMin = pxMin;
            _yMin = pyMin;
            _xMax = pxMax;
            _yMax = pyMax;
        }
        else
        {
            // Geographic CRS – coordinates are in (lon, lat) = (x, y) convention
            _xMin = (double)bbox.West;
            _yMin = (double)bbox.South;
            _xMax = (double)bbox.East;
            _yMax = (double)bbox.North;
        }

        _dx = (_xMax - _xMin) / (_nx - 1);
        _dy = (_yMax - _yMin) / (_ny - 1);
    }

    /// <inheritdoc />
    public (int Ny, int Nx) Shape => (_ny, _nx);

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

    private void ComputeCoordinates()
    {
        var lats = new double[_ny, _nx];
        var lons = new double[_ny, _nx];

        for (int y = 0; y < _ny; y++)
        {
            for (int x = 0; x < _nx; x++)
            {
                double xProj = _xMin + x * _dx;
                double yProj = _yMin + y * _dy;

                if (_isProjected && _toWgs84 is not null)
                {
                    var (lon, lat) = _toWgs84.Transform(xProj, yProj);
                    lons[y, x] = lon;
                    lats[y, x] = lat;
                }
                else
                {
                    lons[y, x] = xProj;
                    lats[y, x] = yProj;
                }
            }
        }

        _latitude = lats;
        _longitude = lons;
    }

    /// <inheritdoc />
    public XYIndex? FindPointXY(double lat, double lon)
    {
        double xProj, yProj;

        if (_isProjected && _toProjection is not null)
        {
            (xProj, yProj) = _toProjection.Transform(lon, lat);
        }
        else
        {
            xProj = lon;
            yProj = lat;
        }

        int xIdx = (int)Math.Round((xProj - _xMin) / _dx);
        int yIdx = (int)Math.Round((yProj - _yMin) / _dy);

        if (xIdx < 0 || xIdx >= _nx || yIdx < 0 || yIdx >= _ny)
            return null;

        return new XYIndex(xIdx, yIdx);
    }

    /// <inheritdoc />
    public LatLon GetCoordinates(int x, int y)
    {
        double xProj = _xMin + x * _dx;
        double yProj = _yMin + y * _dy;

        if (_isProjected && _toWgs84 is not null)
        {
            var (lon, lat) = _toWgs84.Transform(xProj, yProj);
            return new LatLon(lat, lon);
        }

        return new LatLon(yProj, xProj);
    }
}
