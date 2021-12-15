using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry
{
    /// <summary>
    /// 辅助结构，标准化 Z 轴为单位长度后用于存储退化的三维向量，可以更快的计算射线与平面是否相交。
    /// </summary>
    public struct FastVector2d
    {
        public int Direction;
        public double X;
        public double Y;

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
