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
        [Test]
        public void Bug20220728()
        {
            var ply = Polygon.CreateByCoords((-42.486876640512754, 1.6404199350379609),
                                            (-42.486876640511753, -6.5616798024764664),
                                            (-50.1968503936883, -6.5616798024750222),
                                            (-50.196850393688713, -19.356955393031729),
                                            (-37.729658808666684, -19.356955393031768),
                                            (-37.729658808750521, 1.6404199350376896));

            var testPt = new Coord2d(-50.1968503936883, 1.6404199350379609);

            Assert.IsFalse(ply.Contains(testPt));
        }

        [Test]
        public void WindingNumberMethod()
        {
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

                Assert.AreEqual(PointInsidePolygon.ContainsWindingNumberMethod(hole1, (0.75, 0.75)), false);
                Assert.AreEqual(PointInsidePolygon.ContainsWindingNumberMethod(hole1, (3, 0.75)), false);

                Assert.AreEqual(PointInsidePolygon.ContainsWindingNumberMethod(ply, (-1, 1)), false);
                Assert.AreEqual(PointInsidePolygon.ContainsWindingNumberMethod(ply, (0.5, 1)), true);
                Assert.AreEqual(PointInsidePolygon.ContainsWindingNumberMethod(ply, (3, 1)), false);

            }

            {
                var ply = Polygon.CreateByCoords((-42.486876640512754, 1.6404199350379609),
                                           (-42.486876640511753, -6.5616798024764664),
                                           (-50.1968503936883, -6.5616798024750222),
                                           (-50.196850393688713, -19.356955393031729),
                                           (-37.729658808666684, -19.356955393031768),
                                           (-37.729658808750521, 1.6404199350376896));

                var testPt = new Coord2d(-50.1968503936883, 1.6404199350379609);

                Assert.IsFalse(PointInsidePolygon.ContainsWindingNumberMethod(ply, testPt));
            }
        }
    }
}
