using Pancake.ManagedGeometry;
using Pancake.ManagedGeometry.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ModernUtility
{
    public readonly ref struct OnStackPolygon(ReadOnlySpan<Coord2d> pts) : IPolygon
    {
        public readonly ReadOnlySpan<Coord2d> Vertices = pts;
        public Coord2d this[int index] => Vertices[index];

        public int VertexCount => Vertices.Length;

        public void CopyVerticesTo(Coord2d[] array, int startIndex)
        {
            Vertices.CopyTo(array.AsSpan()[startIndex..]);
        }

        public Line2d EdgeAt(int startPtId)
        {
            return new Line2d(this[startPtId], this[startPtId % VertexCount]);
        }
    }
}
