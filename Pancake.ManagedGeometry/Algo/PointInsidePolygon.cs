using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Algo
{
    public static class PointInsidePolygon
    {
        public enum PointContainment
        {
            Unset = 0,
            Inside = 1,
            Outside = 2,
            Coincident = 3
        }

        public static double Tolerance = 1e-8;
        public static PointContainment Contains(Coord[] polygon, Coord ptr)
        {
            var crossing = 0;
            var len = polygon.Length;

            for (var i = 0; i < len; i++)
            {
                var j = i + 1;
                if (j == len) j = 0;

                var p1 = polygon[i];
                var p2 = polygon[j];

                var y1 = p1.Y;
                var y2 = p2.Y;

                var x1 = p1.X;
                var x2 = p2.X;

                if (Math.Abs(x1 - x2) < Tolerance && Math.Abs(y1 - y2) < Tolerance)
                    continue;

                var minY = Math.Min(y1, y2);
                var maxY = Math.Max(y1, y2);

                if (ptr.Y < minY || ptr.Y > maxY)
                    continue;

                if (Math.Abs(minY - maxY) < Tolerance)
                {
                    var minX = Math.Min(x1, x2);
                    var maxX = Math.Max(x1, x2);

                    if (ptr.X >= minX && ptr.X <= maxX)
                    {
                        return PointContainment.Coincident;
                    }
                    else
                    {
                        if (ptr.X < minX)
                            ++crossing;
                    }
                }
                else
                {
                    var x = (x2 - x1) * (ptr.Y - y1) / (y2 - y1) + x1;
                    if (Math.Abs(x - ptr.X) <= Tolerance)
                        return PointContainment.Coincident;

                    if (ptr.X < x)
                    {
                        ++crossing;
                    }
                }
            }

            return ((crossing & 1) == 0) ? PointContainment.Outside : PointContainment.Inside;
        }

        public static PointContainment Contains(Coord2d[] polygon, Coord2d ptr)
        {
            var crossing = 0;
            var len = polygon.Length;

            for (var i = 0; i < len; i++)
            {
                var j = i + 1;
                if (j == len) j = 0;

                var p1 = polygon[i];
                var p2 = polygon[j];

                var y1 = p1.Y;
                var y2 = p2.Y;

                var x1 = p1.X;
                var x2 = p2.X;

                if (Math.Abs(x1 - x2) < Tolerance && Math.Abs(y1 - y2) < Tolerance)
                    continue;

                var minY = Math.Min(y1, y2);
                var maxY = Math.Max(y1, y2);

                if (ptr.Y < minY || ptr.Y > maxY)
                    continue;

                if (Math.Abs(minY - maxY) < Tolerance)
                {
                    var minX = Math.Min(x1, x2);
                    var maxX = Math.Max(x1, x2);

                    if (ptr.X >= minX && ptr.X <= maxX)
                    {
                        return PointContainment.Coincident;
                    }
                    else
                    {
                        if (ptr.X < minX)
                            ++crossing;
                    }
                }
                else
                {
                    var x = (x2 - x1) * (ptr.Y - y1) / (y2 - y1) + x1;
                    if (Math.Abs(x - ptr.X) <= Tolerance)
                        return PointContainment.Coincident;

                    if (ptr.X < x)
                    {
                        ++crossing;
                    }
                }
            }

            return ((crossing & 1) == 0) ? PointContainment.Outside : PointContainment.Inside;
        }
    }
}
