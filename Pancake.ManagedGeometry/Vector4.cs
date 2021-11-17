using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Pancake.ManagedGeometry
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4
    {
        public double X;
        public double Y;
        public double Z;
        public double W;

        public Vector4(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
            W = 1;
        }

        public Vector4(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Coord ThreeDPart => new(X, Y, Z);
        public Coord2d TwoDPart => new(X, Y);

        public static implicit operator Vector4(Coord coord)
        {
            return new Vector4(coord.X, coord.Y, coord.Z);
        }
        public static implicit operator Vector4(Coord2d coord)
        {
            return new Vector4(coord.X, coord.Y, 0);
        }
    }
}
