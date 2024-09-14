using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.DataModel
{
    public interface IPolygon
    {
        public Line2d EdgeAt(int startPtId);
        public int VertexCount { get; }
        public void CopyVerticesTo(Coord2d[] array, int startIndex);
        public Coord2d this[int index] { get; }
    }
}
