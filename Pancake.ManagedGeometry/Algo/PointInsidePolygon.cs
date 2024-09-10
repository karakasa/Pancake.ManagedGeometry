using Pancake.ManagedGeometry.DataModel;
using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pancake.ManagedGeometry.Algo;

public static partial class PointInsidePolygon
{
    public enum PointContainment
    {
        Unset = 0,
        Inside = 1,
        Outside = 2,
        Coincident = 3
    }

    public static double Tolerance = 1e-8;

    private static int LineSide(double x1, double x2, double y1, double y2, in Coord2d pt)
    {
        return Math.Sign((pt.X - x1) * (y2 - y1) - (pt.Y - y1) * (x2 - x1));
    }
    private static int LineSide(double x1, double x2, double y1, double y2, in Coord2d pt, double tolerance)
    {
        return ((pt.X - x1) * (y2 - y1) - (pt.Y - y1) * (x2 - x1)).SignWithTolerance(tolerance);
    }

    private static int LineSide(Coord2d p0, Coord2d p1, Coord2d ptTest, double tolerance)
        => LineSide(p0.X, p1.X, p0.Y, p1.Y, ptTest, tolerance);
    public static PointContainment Contains<TPolygon>(TPolygon polygon, in Coord2d ptr)
        where TPolygon : IPolygon
    {
        return ContainsRaycastingMethod(polygon, ptr, Tolerance);
    }
    public static PointContainment Contains(Coord2d[] polygon, in Coord2d ptr)
    {
        return ContainsRaycastingMethod(new Coord2dWrapper(polygon), ptr, Tolerance);
    }
    public static PointContainment Contains<TPolygon>(TPolygon polygon, in Coord2d ptr, double tolerance)
        where TPolygon : IPolygon
    {
        return ContainsRaycastingMethod(polygon, ptr, tolerance);
    }
    private readonly struct Coord2dWrapper(Coord2d[] array) : IPolygon
    {
        private readonly Coord2d[] _array = array;

        public Coord2d this[int index] => _array[index];

        public IEnumerable<Line2d> Edges => throw new NotImplementedException();

        public IEnumerable<Coord2d> Vertices => throw new NotImplementedException();

        public int VertexCount => _array.Length;

        public void CopyVerticesTo(Coord2d[] array, int startIndex)
        {
            throw new NotImplementedException();
        }

        public Line2d EdgeAt(int startPtId)
        {
            throw new NotImplementedException();
        }
    }
}
