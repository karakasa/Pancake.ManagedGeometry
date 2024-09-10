using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pancake.ManagedGeometry
{
    public readonly struct PolygonEdgeEnumerable(Polygon ply) : IEnumerable<Line2d>
    {
        private readonly Polygon _ply = ply;
        public struct PolygonEdgeEnumerator(Coord2d[] c2d) : IEnumerator<Line2d>
        {
            private readonly Coord2d[] _coords = c2d;
            private int _index = -1;

            public readonly Line2d Current 
                => new(_coords[_index], _coords[(_index + 1) % _coords.Length]);

            readonly object IEnumerator.Current => Current;

            public readonly void Dispose()
            {
            }

            public bool MoveNext()
            {
                ++_index;
                return _index < _coords.Length;
            }

            public void Reset()
            {
                _index = -1;
            }
        }
        IEnumerator<Line2d> IEnumerable<Line2d>.GetEnumerator() => new Polygon.PolygonEdgeEnumeratorBoxed(_ply);
        IEnumerator IEnumerable.GetEnumerator() => new Polygon.PolygonEdgeEnumeratorBoxed(_ply);
        public PolygonEdgeEnumerator GetEnumerator() => new(_ply.InternalVerticeArray);
    }
}
