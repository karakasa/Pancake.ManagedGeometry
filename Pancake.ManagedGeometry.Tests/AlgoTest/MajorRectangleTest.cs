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

            Assert.IsTrue(solver.TryGreedyLookup(ply, out var rect));
            Assert.AreEqual(rect.MinX, 0);
            Assert.AreEqual(rect.MinY, 0);
            Assert.AreEqual(rect.MaxX, 5);
            Assert.AreEqual(rect.MaxY, 5);
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

            Assert.IsTrue(solver.TryGreedyLookup(ply, out var rect));
            Assert.AreEqual(rect.MinX, 4);
            Assert.AreEqual(rect.MinY, 5);
            Assert.AreEqual(rect.MaxX, 5);
            Assert.AreEqual(rect.MaxY, 7);

            solver.MinimalAcceptableLength = 1.5;

            Assert.IsFalse(solver.TryGreedyLookup(ply, out _));
        }
    }
}
