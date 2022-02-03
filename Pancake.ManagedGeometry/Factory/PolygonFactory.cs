using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pancake.ManagedGeometry.Factory
{
    public static class PolygonFactory
    {
        public static Polygon RegularCircumscribed(int side, double circumscribedRadius,
            Coord2d basePt = default, double baseAngle = 0.0)
        {
            var ptArray = new Coord2d[side];
            for(var i = 0; i < ptArray.Length; i++)
            {
                var ang = Math.PI * 2 / side * i + baseAngle;
                ptArray[i] = 
                new Coord2d(circumscribedRadius * Math.Cos(ang)
                + basePt.X, circumscribedRadius * Math.Sin(ang) + basePt.Y);
            }

            return Polygon.CreateByRef(ptArray);
        }

        public static Polygon RegularInscribed(int side, double inscribedRadius,
            Coord2d basePt = default, double baseAngle = 0.0)
        {
            var radius = inscribedRadius / Math.Cos(Math.PI / side);
            return RegularCircumscribed(side, radius, basePt, baseAngle);
        }

        public const int DEFAULT_CIRCLE_DIVISION = 64;
        public const int MINIMAL_CIRCLE_DIVISION = 6;
        public static Polygon TessellatedCircle(double radius, int division = DEFAULT_CIRCLE_DIVISION,
            Coord2d basePt = default)
        {
            return RegularCircumscribed(division, radius, basePt);
        }
        public static Polygon TessellatedCircleWithinAbsoluteError(double radius, double error,
           Coord2d basePt = default)
        {
            var delta = Math.Acos(1 - error / radius);
            if (delta <= 0) 
                // Error is larger compared to the radius.
                // Happens with fixed error and ultra-small radius.
                // Use the default tesslation to avoid underflow.
                return TessellatedCircle(radius, MINIMAL_CIRCLE_DIVISION, basePt);

            delta = Math.Ceiling(Math.PI / delta);

            var division = (delta < MINIMAL_CIRCLE_DIVISION)
                ? MINIMAL_CIRCLE_DIVISION
                : (int)delta;

            return RegularCircumscribed(division, radius, basePt);
        }
        public static Polygon Rectangle(Coord2d pt1, Coord2d pt2)
        {
            var bbox = new BoundingBox2d(pt1, pt2);
            return bbox.ToPolygon();
        }
    }
}
