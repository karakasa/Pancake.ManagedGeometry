using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.HigherLevel
{
    public abstract class Curve
    {
        public abstract Coord Start { get; }
        public abstract Coord End { get; }
    }
}
