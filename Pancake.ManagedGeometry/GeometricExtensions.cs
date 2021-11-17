using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry
{
    public static class GeometricExtensions
    {
        public static Coord Average(this IEnumerable<Coord> pts)
        {
            var cnt = 0;
            var x = 0.0;
            var y = 0.0;
            var z = 0.0;
            foreach (var it in pts)
            {
                x += it.X;
                y += it.Y;
                z += it.Z;
                ++cnt;
            }

            if (cnt == 0)
                return Coord.Unset;

            return new Coord(x / cnt, y / cnt, z / cnt);
        }
    }
}
