using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Tests.PolygonTest
{
    public class DirectionTest
    {
        [Test]
        public void ClassifyTest()
        {
            Polygon ply;

            ply = Polygon.CreateByCoords(
                (0, 0),
                (1, 0),
                (1, 1),
                (0, 1)
                );

            Assert.AreEqual(ply.CalculateDirection(), ClockwiseDirection.CounterClockwise);

            ply = Polygon.CreateByCoords(
                (0, 0),
                (0, 1),
                (1, 1),
                (1, 0)
                );

            Assert.AreEqual(ply.CalculateDirection(), ClockwiseDirection.Clockwise);
        }
    }
}
