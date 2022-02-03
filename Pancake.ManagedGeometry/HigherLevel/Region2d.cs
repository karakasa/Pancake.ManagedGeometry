using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.HigherLevel
{
    public class Region2d
    {
        public Curve ExteriorCurve { get; private set; }
        public Curve[] Holes { get; private set; } = Array.Empty<Curve>();
        private Region2d()
        {

        }
        public static Region2d CreateFromOneCurve(Curve crv)
        {
            return new()
            {
                ExteriorCurve = crv,
                Holes = null
            };
        }
        public bool IsValid => ExteriorCurve != null && Holes != null;
        public double CalculateArea()
        {
            var sum = ExteriorCurve.CalculateArea();
            foreach (var it in Holes) sum -= it.CalculateArea();
            return sum;
        }
    }
}
