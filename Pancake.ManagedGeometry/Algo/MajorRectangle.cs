using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Algo
{
    /// <summary>
    /// Get the "major" (usually the largest) rectangle inside an orthogonal simple polygon, convex or concave.
    /// </summary>
    public class MajorRectangle
    {
        private enum PolygonEdgeCategory
        {
            ParallelToX,
            ParallelToY,
            NotOrthogonal
        }
        public double OrthoTolerance = MathUtils.ZeroTolerance;
        public double MinimumAcceptableLength = 0.0;
        public double MaximumAcceptableLength = double.MaxValue;
        public BoundingBox2d Calculate(Polygon polygon)
        {
            polygon.TrySimplify(out var ply);


            throw new NotImplementedException();
            ClassifyPolygonEdges(polygon, out var xlines, out var ylines);
        }

        private void ClassifyPolygonEdges(Polygon polygon, out List<Line2d> xLines, out List<Line2d> yLines)
        {
            var listParallelToX = new List<Line2d>();
            var listParallelToY = new List<Line2d>();

            for (var i = 0; i < polygon.VerticeCount; i++)
            {
                var line = polygon.LineAt(i);
                switch (ClassifyEdge(line, out var xformed))
                {
                    case PolygonEdgeCategory.ParallelToX:
                        listParallelToX.Add(xformed);
                        break;
                    case PolygonEdgeCategory.ParallelToY:
                        listParallelToY.Add(xformed);
                        break;
                }
            }

            if (listParallelToX.Count <= 2 || listParallelToY.Count <= 2)
            {
                throw new ArgumentException("Insufficient ortho edges in the polygon.");
            }

            xLines = listParallelToX;
            yLines = listParallelToY;
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
    }
}
