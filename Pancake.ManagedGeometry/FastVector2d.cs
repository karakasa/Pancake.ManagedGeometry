using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry
{
    /// <summary>
    /// Supplemental structure that stores a degenerate 3D vector whose Z compound is unitized.
    /// It is used for faster ray intersection calculation with the XY plane, for faster shadow analysis.
    /// </summary>
    public readonly struct FastVector2d
    {
        public readonly int Direction;
        public readonly double X;
        public readonly double Y;

        public FastVector2d(Coord vector3d)
        {
            var v = vector3d;
            var z = v.Z;

            if (Math.Abs(z) < 1e-7)
            {
                X = v.X;
                Y = v.Y;
                Direction = 0;
            }
            else if (z > 0)
            {
                X = v.X / z;
                Y = v.Y / z;
                Direction = 1;
            }
            else
            {
                X = -v.X / z;
                Y = -v.Y / z;
                Direction = -1;
            }
        }
    }
}
