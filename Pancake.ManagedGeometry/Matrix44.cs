using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Pancake.ManagedGeometry
{
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay(
        "({A11}, {A12}, {A13}, {A14}), " +
        "({A21}, {A22}, {A23}, {A24}), " +
        "({A31}, {A32}, {A33}, {A34}), " +
        "({A41}, {A42}, {A43}, {A44})"
        )]
    public struct Matrix44
    {
        public double A11;
        public double A12;
        public double A13;
        public double A14;

        public double A21;
        public double A22;
        public double A23;
        public double A24;

        public double A31;
        public double A32;
        public double A33;
        public double A34;

        public double A41;
        public double A42;
        public double A43;
        public double A44;

        public bool SimilarTo(Matrix44 another)
        {
            return (A11 - another.A11).CloseToZero()
                && (A12 - another.A12).CloseToZero()
                && (A13 - another.A13).CloseToZero()
                && (A14 - another.A14).CloseToZero()
                && (A21 - another.A21).CloseToZero()
                && (A22 - another.A22).CloseToZero()
                && (A23 - another.A23).CloseToZero()
                && (A24 - another.A24).CloseToZero()
                && (A31 - another.A31).CloseToZero()
                && (A32 - another.A32).CloseToZero()
                && (A33 - another.A33).CloseToZero()
                && (A34 - another.A34).CloseToZero()
                && (A41 - another.A41).CloseToZero()
                && (A42 - another.A42).CloseToZero()
                && (A43 - another.A43).CloseToZero()
                && (A44 - another.A44).CloseToZero();
        }
        public static Matrix44 CreateByRowArray(double[] elems)
        {
            if (elems.Length != 16) throw new NotSupportedException();

            return new()
            {
                A11 = elems[0],
                A12 = elems[1],
                A13 = elems[2],
                A14 = elems[3],

                A21 = elems[4],
                A22 = elems[5],
                A23 = elems[6],
                A24 = elems[7],

                A31 = elems[8],
                A32 = elems[9],
                A33 = elems[10],
                A34 = elems[11],

                A41 = elems[12],
                A42 = elems[13],
                A43 = elems[14],
                A44 = elems[15],
            };
        }
        public static Matrix44 CreateByColumn(
            double a11, double a21, double a31, double a41,
            double a12, double a22, double a32, double a42,
            double a13, double a23, double a33, double a43,
            double a14, double a24, double a34, double a44
            )
        {
            return new()
            {
                A11 = a11,
                A21 = a21,
                A31 = a31,
                A41 = a41,
                A12 = a12,
                A22 = a22,
                A32 = a32,
                A42 = a42,
                A13 = a13,
                A23 = a23,
                A33 = a33,
                A43 = a43,
                A14 = a14,
                A24 = a24,
                A34 = a34,
                A44 = a44
            };
        }

        public static Matrix44 operator *(Matrix44 a, Matrix44 b)
        {
            return new()
            {
                A11 = a.A11 * b.A11 + a.A12 * b.A21 + a.A13 * b.A31 + a.A14 * b.A41,
                A12 = a.A11 * b.A12 + a.A12 * b.A22 + a.A13 * b.A32 + a.A14 * b.A42,
                A13 = a.A11 * b.A13 + a.A12 * b.A23 + a.A13 * b.A33 + a.A14 * b.A43,
                A14 = a.A11 * b.A14 + a.A12 * b.A24 + a.A13 * b.A34 + a.A14 * b.A44,

                A21 = a.A21 * b.A11 + a.A22 * b.A21 + a.A23 * b.A31 + a.A24 * b.A41,
                A22 = a.A21 * b.A12 + a.A22 * b.A22 + a.A23 * b.A32 + a.A24 * b.A42,
                A23 = a.A21 * b.A13 + a.A22 * b.A23 + a.A23 * b.A33 + a.A24 * b.A43,
                A24 = a.A21 * b.A14 + a.A22 * b.A24 + a.A23 * b.A34 + a.A24 * b.A44,

                A31 = a.A31 * b.A11 + a.A32 * b.A21 + a.A33 * b.A31 + a.A34 * b.A41,
                A32 = a.A31 * b.A12 + a.A32 * b.A22 + a.A33 * b.A32 + a.A34 * b.A42,
                A33 = a.A31 * b.A13 + a.A32 * b.A23 + a.A33 * b.A33 + a.A34 * b.A43,
                A34 = a.A31 * b.A14 + a.A32 * b.A24 + a.A33 * b.A34 + a.A34 * b.A44,

                A41 = a.A41 * b.A11 + a.A42 * b.A21 + a.A43 * b.A31 + a.A44 * b.A41,
                A42 = a.A41 * b.A12 + a.A42 * b.A22 + a.A43 * b.A32 + a.A44 * b.A42,
                A43 = a.A41 * b.A13 + a.A42 * b.A23 + a.A43 * b.A33 + a.A44 * b.A43,
                A44 = a.A41 * b.A14 + a.A42 * b.A24 + a.A43 * b.A34 + a.A44 * b.A44
            };
        }

        public static implicit operator Matrix44((
            (double, double, double, double),
            (double, double, double, double),
            (double, double, double, double),
            (double, double, double, double)) x)
        {
            return new()
            {
                A11 = x.Item1.Item1,
                A12 = x.Item1.Item2,
                A13 = x.Item1.Item3,
                A14 = x.Item1.Item4,

                A21 = x.Item2.Item1,
                A22 = x.Item2.Item2,
                A23 = x.Item2.Item3,
                A24 = x.Item2.Item4,

                A31 = x.Item3.Item1,
                A32 = x.Item3.Item2,
                A33 = x.Item3.Item3,
                A34 = x.Item3.Item4,

                A41 = x.Item4.Item1,
                A42 = x.Item4.Item2,
                A43 = x.Item4.Item3,
                A44 = x.Item4.Item4,
            };
        }

        public static implicit operator Matrix44((Vector4, Vector4, Vector4, Vector4) x)
        {
            return new()
            {
                A11 = x.Item1.X,
                A12 = x.Item1.Y,
                A13 = x.Item1.Z,
                A14 = x.Item1.W,

                A21 = x.Item2.X,
                A22 = x.Item2.Y,
                A23 = x.Item2.Z,
                A24 = x.Item2.W,

                A31 = x.Item3.X,
                A32 = x.Item3.Y,
                A33 = x.Item3.Z,
                A34 = x.Item3.W,

                A41 = x.Item4.X,
                A42 = x.Item4.Y,
                A43 = x.Item4.Z,
                A44 = x.Item4.W,
            };
        }

        public static Vector4 operator *(Matrix44 a, Vector4 pt)
        {
            return new
            (
                a.A14 * pt.W + a.A11 * pt.X + a.A12 * pt.Y + a.A13 * pt.Z,
                a.A24 * pt.W + a.A21 * pt.X + a.A22 * pt.Y + a.A23 * pt.Z,
                a.A34 * pt.W + a.A31 * pt.X + a.A32 * pt.Y + a.A33 * pt.Z,
                a.A44 * pt.W + a.A41 * pt.X + a.A42 * pt.Y + a.A43 * pt.Z
            );
        }

        public double Determinant()
        {
            return A14 * A23 * A32 * A41 - A13 * A24 * A32 * A41 - A14 * A22 * A33 * A41 + A12 * A24 * A33 * A41 
                + A13 * A22 * A34 * A41 - A12 * A23 * A34 * A41 - A14 * A23 * A31 * A42 + A13 * A24 * A31 * A42 
                + A14 * A21 * A33 * A42 - A11 * A24 * A33 * A42 - A13 * A21 * A34 * A42 + A11 * A23 * A34 * A42 
                + A14 * A22 * A31 * A43 - A12 * A24 * A31 * A43 - A14 * A21 * A32 * A43 + A11 * A24 * A32 * A43 
                + A12 * A21 * A34 * A43 - A11 * A22 * A34 * A43 - A13 * A22 * A31 * A44 + A12 * A23 * A31 * A44 
                + A13 * A21 * A32 * A44 - A11 * A23 * A32 * A44 - A12 * A21 * A33 * A44 + A11 * A22 * A33 * A44;
        }

        public bool TryGetInverse2(out Matrix44 inversed)
        {
            // 其实就是行列式的值
            var denom = Determinant();

            if (denom.CloseToZero())
            {
                inversed = default;
                return false;
            }

            denom = 1 / denom;

            inversed = (
                ((-(A24 * A33 * A42) + A23 * A34 * A42 + A24 * A32 * A43 - A22 * A34 * A43 - A23 * A32 * A44 + A22 * A33 * A44) * denom,
                (A14 * A33 * A42 - A13 * A34 * A42 - A14 * A32 * A43 + A12 * A34 * A43 + A13 * A32 * A44 - A12 * A33 * A44) * denom,
                (-(A14 * A23 * A42) + A13 * A24 * A42 + A14 * A22 * A43 - A12 * A24 * A43 - A13 * A22 * A44 + A12 * A23 * A44) * denom,
                (A14 * A23 * A32 - A13 * A24 * A32 - A14 * A22 * A33 + A12 * A24 * A33 + A13 * A22 * A34 - A12 * A23 * A34) * denom),
                ((A24 * A33 * A41 - A23 * A34 * A41 - A24 * A31 * A43 + A21 * A34 * A43 + A23 * A31 * A44 - A21 * A33 * A44) * denom,
                (-(A14 * A33 * A41) + A13 * A34 * A41 + A14 * A31 * A43 - A11 * A34 * A43 - A13 * A31 * A44 + A11 * A33 * A44) * denom,
                (A14 * A23 * A41 - A13 * A24 * A41 - A14 * A21 * A43 + A11 * A24 * A43 + A13 * A21 * A44 - A11 * A23 * A44) * denom,
                (-(A14 * A23 * A31) + A13 * A24 * A31 + A14 * A21 * A33 - A11 * A24 * A33 - A13 * A21 * A34 + A11 * A23 * A34) * denom),
                ((-(A24 * A32 * A41) + A22 * A34 * A41 + A24 * A31 * A42 - A21 * A34 * A42 - A22 * A31 * A44 + A21 * A32 * A44) * denom,
                (A14 * A32 * A41 - A12 * A34 * A41 - A14 * A31 * A42 + A11 * A34 * A42 + A12 * A31 * A44 - A11 * A32 * A44) * denom,
                (-(A14 * A22 * A41) + A12 * A24 * A41 + A14 * A21 * A42 - A11 * A24 * A42 - A12 * A21 * A44 + A11 * A22 * A44) * denom,
                (A14 * A22 * A31 - A12 * A24 * A31 - A14 * A21 * A32 + A11 * A24 * A32 + A12 * A21 * A34 - A11 * A22 * A34) * denom),
                ((A23 * A32 * A41 - A22 * A33 * A41 - A23 * A31 * A42 + A21 * A33 * A42 + A22 * A31 * A43 - A21 * A32 * A43) * denom,
                (-(A13 * A32 * A41) + A12 * A33 * A41 + A13 * A31 * A42 - A11 * A33 * A42 - A12 * A31 * A43 + A11 * A32 * A43) * denom,
                (A13 * A22 * A41 - A12 * A23 * A41 - A13 * A21 * A42 + A11 * A23 * A42 + A12 * A21 * A43 - A11 * A22 * A43) * denom,
                (-(A13 * A22 * A31) + A12 * A23 * A31 + A13 * A21 * A32 - A11 * A23 * A32 - A12 * A21 * A33 + A11 * A22 * A33) * denom));

            return true;
        }
        public bool TryGetInverse(out Matrix44 inversed)
        {
            // 其实就是行列式的值
            var denom =
                A14 * A23 * A32 * A41 - A13 * A24 * A32 * A41 - A14 * A22 * A33 * A41 + A12 * A24 * A33 * A41
                + A13 * A22 * A34 * A41 - A12 * A23 * A34 * A41 - A14 * A23 * A31 * A42 + A13 * A24 * A31 * A42
                + A14 * A21 * A33 * A42 - A11 * A24 * A33 * A42 - A13 * A21 * A34 * A42 + A11 * A23 * A34 * A42
                + A14 * A22 * A31 * A43 - A12 * A24 * A31 * A43 - A14 * A21 * A32 * A43 + A11 * A24 * A32 * A43
                + A12 * A21 * A34 * A43 - A11 * A22 * A34 * A43 - A13 * A22 * A31 * A44 + A12 * A23 * A31 * A44
                + A13 * A21 * A32 * A44 - A11 * A23 * A32 * A44 - A12 * A21 * A33 * A44 + A11 * A22 * A33 * A44;

            if(denom.CloseToZero())
            {
                inversed = default;
                return false;
            }

            denom = 1 / denom;

            inversed = (
                ((-(A24 * A33 * A42) + A23 * A34 * A42 + A24 * A32 * A43 - A22 * A34 * A43 - A23 * A32 * A44 + A22 * A33 * A44) * denom,
                (A14 * A33 * A42 - A13 * A34 * A42 - A14 * A32 * A43 + A12 * A34 * A43 + A13 * A32 * A44 - A12 * A33 * A44) * denom,
                (-(A14 * A23 * A42) + A13 * A24 * A42 + A14 * A22 * A43 - A12 * A24 * A43 - A13 * A22 * A44 + A12 * A23 * A44) * denom,
                (A14 * A23 * A32 - A13 * A24 * A32 - A14 * A22 * A33 + A12 * A24 * A33 + A13 * A22 * A34 - A12 * A23 * A34) * denom),
                ((A24 * A33 * A41 - A23 * A34 * A41 - A24 * A31 * A43 + A21 * A34 * A43 + A23 * A31 * A44 - A21 * A33 * A44) * denom,
                (-(A14 * A33 * A41) + A13 * A34 * A41 + A14 * A31 * A43 - A11 * A34 * A43 - A13 * A31 * A44 + A11 * A33 * A44) * denom,
                (A14 * A23 * A41 - A13 * A24 * A41 - A14 * A21 * A43 + A11 * A24 * A43 + A13 * A21 * A44 - A11 * A23 * A44) * denom,
                (-(A14 * A23 * A31) + A13 * A24 * A31 + A14 * A21 * A33 - A11 * A24 * A33 - A13 * A21 * A34 + A11 * A23 * A34) * denom),
                ((-(A24 * A32 * A41) + A22 * A34 * A41 + A24 * A31 * A42 - A21 * A34 * A42 - A22 * A31 * A44 + A21 * A32 * A44) * denom,
                (A14 * A32 * A41 - A12 * A34 * A41 - A14 * A31 * A42 + A11 * A34 * A42 + A12 * A31 * A44 - A11 * A32 * A44) * denom,
                (-(A14 * A22 * A41) + A12 * A24 * A41 + A14 * A21 * A42 - A11 * A24 * A42 - A12 * A21 * A44 + A11 * A22 * A44) * denom,
                (A14 * A22 * A31 - A12 * A24 * A31 - A14 * A21 * A32 + A11 * A24 * A32 + A12 * A21 * A34 - A11 * A22 * A34) * denom),
                ((A23 * A32 * A41 - A22 * A33 * A41 - A23 * A31 * A42 + A21 * A33 * A42 + A22 * A31 * A43 - A21 * A32 * A43) * denom,
                (-(A13 * A32 * A41) + A12 * A33 * A41 + A13 * A31 * A42 - A11 * A33 * A42 - A12 * A31 * A43 + A11 * A32 * A43) * denom,
                (A13 * A22 * A41 - A12 * A23 * A41 - A13 * A21 * A42 + A11 * A23 * A42 + A12 * A21 * A43 - A11 * A22 * A43) * denom,
                (-(A13 * A22 * A31) + A12 * A23 * A31 + A13 * A21 * A32 - A11 * A23 * A32 - A12 * A21 * A33 + A11 * A22 * A33) * denom));

            return true;
        }

        public Matrix44 Inverse()
        {
            if (!TryGetInverse(out var matrix))
                throw new InvalidOperationException("Matrix has no inverse.");

            return matrix;
        }

        public static readonly Matrix44 Identity = ((1, 0, 0, 0), (0, 1, 0, 0), (0, 0, 1, 0), (0, 0, 0, 1));

        public static Matrix44 Translation(Coord vec)
        {
            return ((1, 0, 0, vec.X), (0, 1, 0, vec.Y), (0, 0, 1, vec.Z), (0, 0, 0, 1));
        }
    }
}
