using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Algo.ComplicatedRegion
{
    public class Region<TShape, TCalculator>
        where TCalculator : ISimpleBooleanCalculator<TShape>
    {
        private static class SimplyConnectedRegionCalculator
        {

        }
        public TCalculator ShapeCalculator { get; set; } = default;

        private List<SimplyConnectedRegion<TShape>> _subRegions = new();
    }
}
