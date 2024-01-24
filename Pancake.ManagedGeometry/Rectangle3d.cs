using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry
{
    public sealed class Rectangle3d
    {
        public Plane BasePlane { get; set; }
        public Interval1d RangeX { get; set; }
        public Interval1d RangeY { get; set; }
    }
}
