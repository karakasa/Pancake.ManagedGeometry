using NUnit.Framework;
using Pancake.ManagedGeometry.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Tests.AlgoTest
{
    public class MajorRectangleTest
    {
        [Test]
        public void SimpleCaseTest()
        {
            var ply = Polygon.CreateByCoords(
                (0,0),
                (3,0),
                (5,0),
                (5,7),
                (4,7),
                (4,5),
                (0,5)
                );

            var solver = new MajorRectangleSolver();

            Assert.That(solver.TryGreedyLookup(ply, out var rect));
            Utility.AssertEquals(rect.MinX, 0);
            Utility.AssertEquals(rect.MinY, 0);
            Utility.AssertEquals(rect.MaxX, 5);
            Utility.AssertEquals(rect.MaxY, 5);
        }

        [Test]
        public void WithMaximumMinimumTest()
        {
            var ply = Polygon.CreateByCoords(
                (0, 0),
                (3, 0),
                (5, 0),
                (5, 7),
                (4, 7),
                (4, 5),
                (0, 5)
                );

            var solver = new MajorRectangleSolver();

            solver.MaximalAcceptableLength = 3;

            Assert.That(solver.TryGreedyLookup(ply, out var rect));
            Utility.AssertEquals(rect.MinX, 4);
            Utility.AssertEquals(rect.MinY, 5);
            Utility.AssertEquals(rect.MaxX, 5);
            Utility.AssertEquals(rect.MaxY, 7);

            solver.MinimalAcceptableLength = 1.5;

            Assert.That(!solver.TryGreedyLookup(ply, out _));
        }
        [Test]
        public void Bug20220708()
        {
            var tolerance = 1.0 / 1000 * 3.280839895;

            var ply = Polygon.CreateByCoords(
                (-42.486876640512754, 1.6404199350379609),
                (-42.486876640511753, -6.5616798024764664),
                (-50.1968503936883, -6.5616798024750222),
                (-50.196850393688713, -19.356955393031729),
                (-37.729658808666684, -19.356955393031768),
                (-37.729658808683794, -16.404205259639),
                (-37.729658808743693, -6.0695538182211255),
                (-37.729658808750528, -5.5675538182192277),
                (-37.729658808750521, 1.6404199350376896));

            var solver = new MajorRectangleSolver { OrthoTolerance = tolerance };

            Assert.That(solver.TryGreedyLookup(ply, out var rect));
            Utility.AssertEquals(12.4672, rect.SpanX, 0.0001);
            Utility.AssertEquals(12.7953, rect.SpanY, 0.0001);
        }
    }
}
