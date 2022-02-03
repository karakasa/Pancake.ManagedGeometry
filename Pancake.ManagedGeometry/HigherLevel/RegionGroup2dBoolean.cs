using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pancake.ManagedGeometry.HigherLevel
{
    public static class RegionGroup2dBoolean
    {
        public static RegionGroup2d Intersect(RegionGroup2d crv1, RegionGroup2d crv2)
        {
            throw new NotImplementedException();
        }
        public static RegionGroup2d Union(RegionGroup2d crv1, RegionGroup2d crv2)
        {
            throw new NotImplementedException();
        }
        public static RegionGroup2d Subtract(RegionGroup2d crvToBeSubtractedFrom, RegionGroup2d crvToBeSubtracted)
        {
            throw new NotImplementedException();
        }
        public static bool Contains(RegionGroup2d largerCrv, RegionGroup2d smallerCrv)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsXYOverlapped(RegionGroup2d a, RegionGroup2d b)
        {
            if (a.Regions.Count == 1 && b.Regions.Count == 1)
            {
                // In many cases there's only one region in each group.
                return a.Regions[0].ExteriorCurve.GetBoundingBox2d()
                    .IntersectsWith(b.Regions[0].ExteriorCurve.GetBoundingBox2d());
            }

            // Due to this function may be a heat path, pure array access is used against LINQ.
            // Array access is about 7x faster than LINQ.

            var AhasMoreRegions = a.Regions.Count > b.Regions.Count;
            var grpMoreRegions = AhasMoreRegions ? a : b;
            var grpFewerRegions = AhasMoreRegions ? b : a;

            // Array-ize the smaller region group to reduce memory footprint and is ~10% faster
            var bboxes = new BoundingBox2d[grpFewerRegions.Regions.Count];
            for (var i = 0; i < bboxes.Length; i++)
                bboxes[i] = grpFewerRegions.Regions[i].ExteriorCurve.GetBoundingBox2d();

            var regions = grpMoreRegions.Regions;
            var cnt = regions.Count;

            for (var i = 0; i < cnt; i++)
            {
                var it = regions[i].ExteriorCurve.GetBoundingBox2d();
                foreach(var it2 in bboxes)
                    if (it.IntersectsWith(it2)) return true;
            }

            return false;
        }
    }
}

