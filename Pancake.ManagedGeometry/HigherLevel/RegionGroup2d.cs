using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pancake.ManagedGeometry.HigherLevel
{
    public class RegionGroup2d
    {
        public List<Region2d> Regions { get; set; } = new();
        public double CalculateArea() => Regions.Sum(static r => r.CalculateArea());
    }
}
