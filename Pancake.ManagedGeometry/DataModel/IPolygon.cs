﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.DataModel
{
    public interface IPolygon
    {
        public Coord2d VertexAt(int verticeId);
        public Line2d EdgeAt(int startPtId);
        public IEnumerable<Line2d> Edges { get; }
        public IEnumerable<Coord2d> Vertices { get; }
        public int VertexCount { get; }
        public void CopyVerticesTo(Coord2d[] array, int startIndex);
        public Coord2d this[int index] { get; }
    }
}
