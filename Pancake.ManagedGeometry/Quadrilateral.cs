using Pancake.ManagedGeometry.Algo;
using Pancake.ManagedGeometry.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry;

public readonly struct Quadrilateral(Coord2d a, Coord2d b, Coord2d c, Coord2d d) : IPolygon, IEnumerable<Coord2d>
{
    public readonly Coord2d Vertex0 = a;
    public readonly Coord2d Vertex1 = b;
    public readonly Coord2d Vertex2 = c;
    public readonly Coord2d Vertex3 = d;
    public Coord2d this[int index] => index switch
    {
        0 => Vertex0,
        1 => Vertex1,
        2 => Vertex2,
        3 => Vertex3,
        _ => ThrowOutOfRange<Coord2d>()
    };

    IEnumerable<Line2d> IPolygon.Edges => [EdgeAt(0), EdgeAt(1), EdgeAt(2), EdgeAt(3)];

    IEnumerable<Coord2d> IPolygon.Vertices => [Vertex0, Vertex1, Vertex2, Vertex3];

    public struct VerticesEnumerator(Quadrilateral qd) : IEnumerator<Coord2d>
    {
        private readonly Quadrilateral _qd = qd;
        private int index = -1;
        public readonly Coord2d Current => _qd[index];

        readonly object IEnumerator.Current => Current;

        public readonly void Dispose() { }

        public bool MoveNext() { ++index; return index < 4; }

        public void Reset() { index = -1; }
    }

    public struct EdgeEnumerator(Quadrilateral qd) : IEnumerator<Line2d>, IEnumerable<Line2d>
    {
        private readonly Quadrilateral _qd = qd;
        private int index = -1;
        public readonly Line2d Current => _qd.EdgeAt(index);

        readonly object IEnumerator.Current => Current;

        public readonly void Dispose() { }

        public bool MoveNext() { ++index; return index < 4; }

        public void Reset() { index = -1; }
        readonly IEnumerator<Line2d> IEnumerable<Line2d>.GetEnumerator() => this;
        readonly IEnumerator IEnumerable.GetEnumerator() => this;
        public readonly EdgeEnumerator GetEnumerator() => this;
    }

    public int VertexCount => 4;

    public void CopyVerticesTo(Coord2d[] array, int startIndex)
    {
        array[startIndex] = Vertex0;
        array[startIndex + 1] = Vertex1;
        array[startIndex + 2] = Vertex2;
        array[startIndex + 3] = Vertex3;
    }

    public Line2d EdgeAt(int startPtId)
    {
        return startPtId switch
        {
            0 => new Line2d(Vertex0, Vertex1),
            1 => new Line2d(Vertex1, Vertex2),
            2 => new Line2d(Vertex2, Vertex3),
            3 => new Line2d(Vertex3, Vertex0),
            _ => ThrowOutOfRange<Line2d>()
        };
    }

    private static T ThrowOutOfRange<T>() => throw new IndexOutOfRangeException();
    IEnumerator<Coord2d> IEnumerable<Coord2d>.GetEnumerator() => new VerticesEnumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => new VerticesEnumerator(this);
    public VerticesEnumerator GetEnumerator() => new(this);

    public ClockwiseDirection CalculateDirection()
    {
        return (this[1].X - this[3].X) * (this[0].Y - this[2].Y) - (this[0].X - this[2].X) * (this[1].Y - this[3].Y) < 0 
            ? ClockwiseDirection.CounterClockwise : ClockwiseDirection.Clockwise;
    }
    public double CalculateDirectionalArea()
    {
        var area1 = 0.0;
        var area2 = 0.0;

        area1 += this[0].X * this[1].Y;
        area1 += this[1].X * this[2].Y;
        area1 += this[2].X * this[3].Y;
        area1 += this[3].X * this[0].Y;

        area2 += this[0].Y * this[1].X;
        area2 += this[1].Y * this[2].X;
        area2 += this[2].Y * this[3].X;
        area2 += this[3].Y * this[0].X;

        var area = (area1 - area2) / 2;
        return area;
    }
    public double CalculateArea()
    {
        return Math.Abs(this.CalculateDirectionalArea());
    }
}