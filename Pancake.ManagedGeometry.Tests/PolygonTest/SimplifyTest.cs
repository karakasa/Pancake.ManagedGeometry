using NUnit.Framework;
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

            Assert.IsTrue(ply.TrySimplify(out var newPly));
            Assert.AreEqual(newPly.VerticeCount, 4);
            Assert.AreEqual(newPly.CalculateArea(), 1, 1e-9);
            Assert.AreEqual(newPly.CalculatePerimeter(), 4, 1e-9);
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

            Assert.IsTrue(ply.TrySimplify(out newPly));
            Assert.AreEqual(newPly.VerticeCount, 4);
            Assert.AreEqual(newPly.CalculateArea(), 1, 1e-9);
            Assert.AreEqual(newPly.CalculatePerimeter(), 4, 1e-9);

            ply = Polygon.CreateByRef(new Coord2d[] {
                (0.2,1),
                (0.3,1),
                (1,1),
                (1,0),
                (0,0),
                (0,1),
                (0.1,1)
            });

            Assert.IsTrue(ply.TrySimplify(out newPly));
            Assert.AreEqual(newPly.VerticeCount, 4);
            Assert.AreEqual(newPly.CalculateArea(), 1, 1e-9);
            Assert.AreEqual(newPly.CalculatePerimeter(), 4, 1e-9);

            ply = Polygon.CreateByRef(new Coord2d[] {
                (0.3,1),
                (1,1),
                (1,0),
                (0,0),
                (0,1),
                (0.1,1),
                (0.2,1),
            });

            Assert.IsTrue(ply.TrySimplify(out newPly));
            Assert.AreEqual(newPly.VerticeCount, 4);
            Assert.AreEqual(newPly.CalculateArea(), 1, 1e-9);
            Assert.AreEqual(newPly.CalculatePerimeter(), 4, 1e-9);
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

            Assert.IsFalse(ply.TrySimplify(out var ply2));
            Assert.AreSame(ply, ply2);
        }
    }
}
