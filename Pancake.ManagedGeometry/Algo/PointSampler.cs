using Pancake.ManagedGeometry;
using Pancake.ManagedGeometry.Algo;
using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Algo
{
    public struct PointSampleRegion
    {
        public Polygon Polygon;
        public double Height;

        public static PointSampleRegion Create(Polygon ply, double height = 0.9)
            => new() { Polygon = ply, Height = height };

        public static PointSampleRegion[] CreateSingle(Polygon ply, double height = 0.9)
            => new[] { new PointSampleRegion { Polygon = ply, Height = height } };
    }
    public static class PointSampler
    {
        public static Coord[] SamplePoints(PointSampleRegion[] regions, double distance)
        {
            var terminals = regions.Select(ply =>
            {
                ply.Polygon.InternalVerticeArray.Select(c => c.X).MinMax(out var minX, out var maxX);
                ply.Polygon.InternalVerticeArray.Select(c => c.Y).MinMax(out var minY, out var maxY);
                return new { MinX = minX, MaxX = maxX, MinY = minY, MaxY = maxY };
            }).ToArray();

            var rangeMinX = terminals.Min(t => t.MinX);
            var rangeMaxX = terminals.Max(t => t.MaxX);

            var rangeMinY = terminals.Min(t => t.MinY);
            var rangeMaxY = terminals.Max(t => t.MaxY);

            if (rangeMinX > rangeMaxX || rangeMinY > rangeMaxY)
                throw new InvalidOperationException();

            var list = new List<Coord>();

            rangeMinX = Math.Floor(rangeMinX);
            rangeMinY = Math.Floor(rangeMinY);

            rangeMaxX = Math.Ceiling(rangeMaxX);
            rangeMaxY = Math.Ceiling(rangeMaxY);

            for (var x = rangeMinX; x <= rangeMaxX; x += distance)
                for (var y = rangeMinY; y <= rangeMaxY; y += distance)
                {
                    var pt = new Coord2d(x, y);
                    foreach (var it in regions)
                    {
                        if (PointInsidePolygon.Contains(it.Polygon, pt)
                            != PointInsidePolygon.PointContainment.Outside)
                        {
                            list.Add(new Coord(x, y, it.Height));
                            break;
                        }
                    }
                }

            return list.ToArray();
        }
    }
}
