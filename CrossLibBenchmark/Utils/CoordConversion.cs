using Pancake.ManagedGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossLibBenchmark.Utils
{
    public static class CoordConversion
    {
        public static Elements.Geometry.Vector3 ToElement(this Coord coord)
        {
            return new(coord.X, coord.Y, coord.Z);
        }
        public static GShark.Geometry.Point3 ToGShark(this Coord coord)
        {
            return new(coord.X, coord.Y, coord.Z);
        }
    }
}
