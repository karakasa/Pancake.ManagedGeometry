﻿using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pancake.ManagedGeometry.Algo
{
    /// <summary>
    /// Get the "major" ortho rectangle inside a simple polygon.
    /// </summary>
    public class MajorRectangleSolver
    {
        /// <summary>
        /// The minimal acceptable length of the output rectangle.
        /// Rectangles with a shorter edge whose length is small than this value will be skipped.
        /// By default <see langword="0"/>.
        /// </summary>
        public double MinimalAcceptableLength { get; set; } = 0.0;
        /// <summary>
        /// The maximal acceptable length of the output rectangle.
        /// Rectangles with a lengthier edge whose length is larger than this value will be skipped.
        /// By default <see cref="double.MaxValue"/>.
        /// </summary>
        public double MaximalAcceptableLength { get; set; } = double.MaxValue;

        /// <summary>
        /// An extra filter of what rectangles are accepted.
        /// If not <see langword="null"/>, this function will be called to determine whether the rectangle will be kept.
        /// This function should return <see langword="true"/> to keep the rectangle.
        /// </summary>
        public Func<BoundingBox2d, bool> ExtraFilter { get; set; } = null;

        private struct Coord2dWithMetadata<T>
        {
            public Coord2d Coords;
            public T Metadata;
        }
        #region Greedy Algorithm
        private struct PolygonEdge
        {
            public PolygonEdgeCategory Category;
            public Line2d Edge;
        }
        private enum PolygonEdgeCategory
        {
            ParallelToX,
            ParallelToY,
            NotOrthogonal
        }
        private Coord2d[] _bboxPts = new Coord2d[4];
        private double _orthoTolerance = MathUtils.ZeroTolerance;
        /// <summary>
        /// Extra tolerance option, specially for <see cref="TryGreedyLookup(Polygon, out BoundingBox2d)"/>,
        /// because some inputs are not strictly ortho.
        /// </summary>
        public double OrthoTolerance
        {
            get => _orthoTolerance;
            set
            {
                _orthoTolerance = value;
                EnsureEpsComparer(value);
            }
        }

        private EpsilonComparerStruct _comparerStruct = new();
        private EpsilonComparer _comparerClass;
        private void EnsureEpsComparer(double eps)
        {
            _comparerClass ??= new();

            _comparerClass.Tolerance = eps;
            _comparerStruct.Tolerance = eps;
        }
        /// <summary>
        /// This implementation is a fast greedy algorithm.
        /// Output rectangle will use at least 2 pendicular edges from the input polygon, which must be ortho.
        /// It doesn't guarantee for the best result and cannot handle inordinary cases.
        /// </summary>
        /// <param name="polygon">An ortho polygon</param>
        /// <param name="rectangle">The output</param>
        /// <returns>Whether a rectangle can be found</returns>
        public bool TryGreedyLookup(Polygon polygon, out BoundingBox2d rectangle)
        {
            polygon.TrySimplify(out var ply, OrthoTolerance);

            var edges = new PolygonEdge[ply.VertexCount];
            for (var i = 0; i < ply.VertexCount; i++)
                edges[i] = ClassifyLine(ply.EdgeAt(i));

            BoundingBox2d maximumBBox = default;
            double areaBBox = 0;

            for (var i = 0; i < edges.Length; i++)
            {
                var edge1 = edges[i];
                var edge2 = edges[(i + 1) % edges.Length];

                if (!IsDifferentOrthoPair(edge1, edge2))
                    continue;

                _bboxPts[0] = edge1.Edge.From;
                _bboxPts[1] = edge1.Edge.To;
                _bboxPts[2] = edge2.Edge.From;
                _bboxPts[3] = edge2.Edge.To;

                var bbox = new BoundingBox2d(_bboxPts);
                var spanX = bbox.SpanX;
                var spanY = bbox.SpanY;

                GeometricExtensions.MinMax(spanX, spanY, out var shorterEdge, out var lengthierEdge);

                if (shorterEdge < MinimalAcceptableLength || lengthierEdge > MaximalAcceptableLength)
                    continue;

                if (spanX * spanY < areaBBox)
                    continue;

                var badPolygon = false;

                foreach(var it in bbox.Vertices)
                {
                    var found = false;

                    foreach (var it2 in _bboxPts)
                    {
                        if (it2.AlmostEqualTo(it))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (found)
                        continue;

                    if (!ply.Contains(it))
                    {
                        badPolygon = true;
                        break;
                    }
                }

                if (badPolygon)
                    continue;

                if (ExtraFilter != null && !ExtraFilter(bbox))
                    continue;

                areaBBox = spanX * spanY;
                maximumBBox = bbox;
            }

            rectangle = maximumBBox;
            return areaBBox > 0;
        }

        private bool IsDifferentOrthoPair(PolygonEdge edge1, PolygonEdge edge2)
        {
            if (edge1.Category == PolygonEdgeCategory.ParallelToX && edge2.Category == PolygonEdgeCategory.ParallelToY)
            {
                return true;
            }

            if (edge2.Category == PolygonEdgeCategory.ParallelToX && edge1.Category == PolygonEdgeCategory.ParallelToY)
            {
                return true;
            }

            return false;
        }
        private PolygonEdge ClassifyLine(Line2d line)
        {
            var result = ClassifyEdge(line, out var xformed);
            return new PolygonEdge
            {
                Category = result,
                Edge = xformed
            };
        }

        private PolygonEdgeCategory ClassifyEdge(Line2d line, out Line2d xformedLine)
        {
            var zeroX = line.Direction.X.CloseToZero(OrthoTolerance);
            var zeroY = line.Direction.Y.CloseToZero(OrthoTolerance);

            if (!(zeroX ^ zeroY))
            {
                xformedLine = line;
                return PolygonEdgeCategory.NotOrthogonal;
            }
            else
            {
                if (zeroX)
                {
                    // Parallel to Y
                    xformedLine = new Line2d(line.From, line.From + (0, line.Direction.Y));
                    return PolygonEdgeCategory.ParallelToY;
                }
                else
                {
                    // Parallel to X
                    xformedLine = new Line2d(line.From, line.From + (line.Direction.X, 0));
                    return PolygonEdgeCategory.ParallelToX;
                }
            }
        }

        #endregion
        #region Better Algorithm for Arbitary Polygon
        /// <summary>
        /// Solve the problem using the discrete way, possibly slower.
        /// But the polygon doesn't need to be ortho.
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="rectangle"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool TryDiscreteLookup(Polygon polygon, out BoundingBox2d rectangle)
        {
            var vertices = polygon.InternalVerticeArray;
            var count = vertices.Length;
            var coordinatesX = new double[count];
            var coordinatesY = new double[count];

            for (var i = 0; i < count; i++)
            {
                coordinatesX[i] = vertices[i].X;
                coordinatesY[i] = vertices[i].Y;
            }

            var lenCoordX = Distincter.SortAndDistinctArrayInplace(coordinatesX,
                _comparerStruct, _comparerClass);
            var lenCoordY = Distincter.SortAndDistinctArrayInplace(coordinatesY,
                _comparerStruct, _comparerClass);

            var pts = new Coord2dWithMetadata<bool>[lenCoordX, lenCoordY];
            for (var i = 0; i < lenCoordX; i++)
                for (var j = 0; j < lenCoordY; j++)
                {
                    var pt = new Coord2d(coordinatesX[i], coordinatesX[j]);
                    pts[i, j] = new Coord2dWithMetadata<bool>
                    {
                        Coords = pt,
                        Metadata = polygon.Contains(pt)
                    };
                }

            throw new NotImplementedException();
        }
        #endregion
    }
}
