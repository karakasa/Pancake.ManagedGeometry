using NUnit.Framework;
using Pancake.ManagedGeometry.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Tests.AlgoTest
{
    public class ShortestPathOfTravel
    {
        [Test]
        public void TestBasic()
        {
            var ply = new Polygon(new Coord2d[] {
            (0,0),
            (2,0),
            (2,1),
            (1,1),
            (1,2),
            (0,2)
            });

            PathOfTravel solver = null;

            Assert.DoesNotThrow(() => solver = new PathOfTravel(ply));
        }

        [Test]
        public void TestWithHole()
        {
            var ply = new Polygon(new Coord2d[] {
            (0,0),
            (2,0),
            (2,1),
            (1,1),
            (1,2),
            (0,2)
            });

            var hole1 = new Polygon(new Coord2d[] {
            (1.25, 0.25),
            (1.75, 0.25),
            (1.75, 0.75),
            (1.25, 0.75)
            });

            var hole2 = new Polygon(new Coord2d[] {
            (0.25, 1.25),
            (0.75, 1.25),
            (0.75, 1.75),
            (0.25, 1.75)
            });

            PathOfTravel solver = null;

            Assert.DoesNotThrow(() => solver = new PathOfTravel(ply, new[] { hole1, hole2}));
        }
    }
}
