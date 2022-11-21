using Pancake.ManagedGeometry.HigherLevel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Algo.ComplicatedRegion
{
    public class SimplyConnectedRegion<TShape>
    {
        public TShape ExteriorCurve { get; internal set; }
        public TShape[] Holes { get; internal set; } = Array.Empty<TShape>();
    }
}
