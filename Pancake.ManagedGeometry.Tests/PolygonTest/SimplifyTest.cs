using NUnit.Framework;
using Pancake.ManagedGeometry.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Tests.PolygonTest
{
    public class SimplifyTest
    {
        [Test]
        public void SimpleSimplify()
        {
            var ply = Polygon.CreateByRef(new Coord2d[] {
                (0,0),
                (0,1),
                (0.1,1),
                (0.2,1),
                (0.3,1),
                (1,1),
                (1,0)
            });

            Assert.That(ply.TrySimplify(out var newPly));
            Utility.AssertEquals(newPly.VertexCount, 4);
            Utility.AssertEquals(newPly.CalculateArea(), 1, 1e-9);
            Utility.AssertEquals(newPly.CalculatePerimeter(), 4, 1e-9);
        }

        [Test]
        public void SimplifyInvolvingSeamPoint()
        {
            Polygon newPly, ply;

            ply = Polygon.CreateByRef(new Coord2d[] {
                (0.1,1),
                (0.2,1),
                (0.3,1),
                (1,1),
                (1,0),
                (0,0),
                (0,1)
            });

            Assert.That(ply.TrySimplify(out newPly));
            Utility.AssertEquals(newPly.VertexCount, 4);
            Utility.AssertEquals(newPly.CalculateArea(), 1, 1e-9);
            Utility.AssertEquals(newPly.CalculatePerimeter(), 4, 1e-9);

            ply = Polygon.CreateByRef(new Coord2d[] {
                (0.2,1),
                (0.3,1),
                (1,1),
                (1,0),
                (0,0),
                (0,1),
                (0.1,1)
            });

            Assert.That(ply.TrySimplify(out newPly));
            Utility.AssertEquals(newPly.VertexCount, 4);
            Utility.AssertEquals(newPly.CalculateArea(), 1, 1e-9);
            Utility.AssertEquals(newPly.CalculatePerimeter(), 4, 1e-9);

            ply = Polygon.CreateByRef(new Coord2d[] {
                (0.3,1),
                (1,1),
                (1,0),
                (0,0),
                (0,1),
                (0.1,1),
                (0.2,1),
            });

            Assert.That(ply.TrySimplify(out newPly));
            Utility.AssertEquals(newPly.VertexCount, 4);
            Utility.AssertEquals(newPly.CalculateArea(), 1, 1e-9);
            Utility.AssertEquals(newPly.CalculatePerimeter(), 4, 1e-9);
        }

        [Test]
        public void ThrowOnDegeneratePolygonTest()
        {
            var ply = Polygon.CreateByCoords(
                (0, 0),
                (1.5, 0),
                (2, 0),
                (1.3, 0),
                (1, 0)
                );

            Assert.Throws<InvalidOperationException>(() => ply.TrySimplify(out _));
        }
        [Test]
        public void NoSimplificationTest()
        {
            var ply = Polygon.CreateByCoords(
                (0, 0),
                (1, 0),
                (1, 1),
                (0, 1)
                );

            Assert.That(!ply.TrySimplify(out var ply2));
            Assert.That(ply.InternalVerticeArray, Is.SameAs(ply2.InternalVerticeArray));
        }

        [Test]
        public void MergePointsTest()
        {
            var ply = Polygon.CreateByCoords(
                (0, 0),
                (0, 0),
                (0, 0),
                (1, 1),
                (1, 1),
                (1, 1),
                (1, 1),
                (1, 1),
                (1, 1),
                (1, 0),
                (0, 0)
                );

            Assert.That(ply.TrySimplify(out var ply2));
            Utility.AssertEquals(3, ply2.VertexCount);
            Utility.AssertEquals(0.5, ply2.CalculateArea(), 0.0001);
        }
        /// <summary>
        /// Previously simplification tolerance is not supported, which causes issue in MajorRectangleSolver
        /// </summary>
        [Test]
        public void Bug20220728()
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

            Assert.That(ply.TrySimplify(out var ply2, tolerance));
            Utility.AssertEquals(ply2.VertexCount, 6);
            Utility.AssertEquals(ply2.CalculateArea(), 198.5403, 0.0001);
        }
    }
}
