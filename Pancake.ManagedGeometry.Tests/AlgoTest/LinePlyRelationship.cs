using NUnit.Framework;
using Pancake.ManagedGeometry.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Tests.AlgoTest
{
    public class LinePlyRelationship
    {
        [Test]
        public void InsidePolygon()
        {
            var ply = new Polygon(new Coord2d[] {
            (0,0),
            (2,0),
            (2,1),
            (1,1),
            (1,2),
            (0,2)
            });

            Assert.AreEqual(ply.CalculateArea(), 3.0, 1e-6);

            var solver = new LineInsidePolygon();

            Assert.IsTrue(solver.IsInside(ply, ((0, 0), (1, 1))));
            Assert.IsTrue(solver.IsInside(ply, ((0.3, 0.3), (0.5, 0.5))));
            Assert.IsTrue(solver.IsInside(ply, ((0, 2), (2, 0))));

            Assert.IsTrue(solver.IsInside(ply, ((1, 1), (2, 1))));
            Assert.IsTrue(solver.IsInside(ply, ((1, 1), (1.9, 1))));
            Assert.IsTrue(solver.IsInside(ply, ((0.9, 1), (1.9, 1))));
            Assert.IsTrue(solver.IsInside(ply, ((0.9, 1), (2, 1))));

            Assert.IsTrue(solver.IsInside(ply, ((0.5, 1.5), (1.5, 0.5))));
            Assert.IsTrue(solver.IsInside(ply, ((0.5, 1.5), (1, 1.5))));

            Assert.IsFalse(solver.IsInside(ply, ((0.5, 0.5), (1.5, 1.5))));
            Assert.IsFalse(solver.IsInside(ply, ((0, 0), (3, 1))));
            Assert.IsFalse(solver.IsInside(ply, ((6, 6), (5, 5))));

            Assert.IsFalse(solver.IsInside(ply, ((0.9, 1), (2.1, 1))));
            Assert.IsFalse(solver.IsInside(ply, ((1.1, 1), (2.1, 1))));

            Assert.IsFalse(solver.IsInside(ply, ((1,2), (2,1))));
            Assert.IsFalse(solver.IsInside(ply, ((1, 2), (3,0))));

            Assert.IsFalse(solver.IsInside(ply, ((0.6, 1.5), (1.6, 0.5))));
            Assert.IsFalse(solver.IsInside(ply, ((2, 1.5), (1, 1.5))));
        }
        [Test]
        public void OutsidePolygon()
        {
            var ply = new Polygon(new Coord2d[] {
            (0,0),
            (2,0),
            (2,1),
            (1,1),
            (1,2),
            (0,2)
            });

            Assert.AreEqual(ply.CalculateArea(), 3.0, 1e-6);

            var solver = new LineInsidePolygon();

            Assert.IsFalse(solver.IsOutside(ply, ((0, 0), (1, 1))));
            Assert.IsFalse(solver.IsOutside(ply, ((0.3, 0.3), (0.5, 0.5))));
            Assert.IsFalse(solver.IsOutside(ply, ((0, 2), (2, 0))));

            Assert.IsTrue(solver.IsOutside(ply, ((1, 1), (2, 1))));
            Assert.IsTrue(solver.IsOutside(ply, ((1, 1), (1.9, 1))));
            Assert.IsFalse(solver.IsOutside(ply, ((0.9, 1), (1.9, 1))));
            Assert.IsFalse(solver.IsOutside(ply, ((0.9, 1), (2, 1))));

            Assert.IsFalse(solver.IsOutside(ply, ((0.5, 1.5), (1.5, 0.5))));
            Assert.IsFalse(solver.IsOutside(ply, ((0.5, 1.5), (1, 1.5))));

            Assert.IsFalse(solver.IsOutside(ply, ((0.5, 0.5), (1.5, 1.5))));
            Assert.IsFalse(solver.IsOutside(ply, ((0, 0), (3, 1))));
            Assert.IsTrue(solver.IsOutside(ply, ((6, 6), (5, 5))));

            Assert.IsFalse(solver.IsOutside(ply, ((0.9, 1), (2.1, 1))));
            Assert.IsTrue(solver.IsOutside(ply, ((1.1, 1), (2.1, 1))));

            Assert.IsTrue(solver.IsOutside(ply, ((1, 2), (2, 1))));
            Assert.IsTrue(solver.IsOutside(ply, ((1, 2), (3, 0))));

            Assert.IsFalse(solver.IsOutside(ply, ((0.6, 1.5), (1.6, 0.5))));
            Assert.IsTrue(solver.IsOutside(ply, ((2, 1.5), (1, 1.5))));
        }
    }
}
