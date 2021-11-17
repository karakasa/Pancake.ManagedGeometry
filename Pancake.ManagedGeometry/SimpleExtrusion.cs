using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pancake.ManagedGeometry
{
    public static class SimpleExtrusion
    {
        public static IEnumerable<IList<Coord>> To3dFaces(Coord[] polygons, double height)
        {
            for (var i = 0; i < polygons.Length; i++)
            {
                var j = i + 1;
                if (j == polygons.Length) j = 0;

                var pt1 = polygons[i];
                var pt2 = polygons[j];

                if (pt1.IdenticalTo(pt2))
                    continue;

                var rect = new Coord[4];

                rect[0] = pt1;
                rect[1] = pt2;
                rect[2] = new Coord(pt2.X, pt2.Y, pt2.Z + height);
                rect[3] = new Coord(pt1.X, pt1.Y, pt1.Z + height);

                yield return rect;
            }

            yield return polygons.Select(x => new Coord(x.X, x.Y, x.Z + height)).ToArray();
        }
    }
}
