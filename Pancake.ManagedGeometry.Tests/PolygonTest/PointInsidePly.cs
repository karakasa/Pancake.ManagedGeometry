using NUnit.Framework;
using Pancake.ManagedGeometry.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Tests.PolygonTest
{
    public class PointInsidePly
    {
        [Test]
        public void BugFoundOn20220108()
        {

            var hole1 = new Polygon(new Coord2d[] {
            (1.25, 0.25),
            (1.75, 0.25),
            (1.75, 0.75),
            (1.25, 0.75)
            });

            var ply = new Polygon(new Coord2d[] {
            (0,0),
            (2,0),
            (2,1),
            (1,1),
            (1,2),
            (0,2)
            });

            Assert.AreEqual(PointInsidePolygon.Contains(hole1, (0.75, 0.75)), PointInsidePolygon.PointContainment.Outside);
            Assert.AreEqual(PointInsidePolygon.Contains(hole1, (1.3, 0.75)), PointInsidePolygon.PointContainment.Coincident);
            Assert.AreEqual(PointInsidePolygon.Contains(hole1, (3, 0.75)), PointInsidePolygon.PointContainment.Outside);

            Assert.AreEqual(PointInsidePolygon.Contains(ply, (-1, 1)), PointInsidePolygon.PointContainment.Outside);
            Assert.AreEqual(PointInsidePolygon.Contains(ply, (0.5, 1)), PointInsidePolygon.PointContainment.Inside);
            Assert.AreEqual(PointInsidePolygon.Contains(ply, (1.5, 1)), PointInsidePolygon.PointContainment.Coincident);
            Assert.AreEqual(PointInsidePolygon.Contains(ply, (3, 1)), PointInsidePolygon.PointContainment.Outside);
        }
    }
}
