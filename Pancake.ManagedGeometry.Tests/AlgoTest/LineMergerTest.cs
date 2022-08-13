using NUnit.Framework;
using NUnit.Framework.Constraints;
using Pancake.ManagedGeometry.Algo;
using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Tests.AlgoTest
{
    public class LineMergerTest
    {
        [Test]
        public void SimpleTest()
        {
            var merger = new LineMerger
            {
                Tolerance = 1e-9,
                SplitAtOriginalEndPoints = false
            };

            var lines = new Line2d[]
            {
                ((1,1), (3,3)),
                ((2,2), (4,4)),
                ((6,6), (7,7))
            };

            var calculated = merger.Calculate(lines).OrderBy(l2d => l2d.From.X).ToArray();

            Assert.AreEqual(2, calculated.Length);

            Assert.IsTrue(calculated[0].AlmostEqualTo(((1, 1), (4, 4))));
            Assert.IsTrue(calculated[1].AlmostEqualTo(((6, 6), (7, 7))));

            merger.SplitAtOriginalEndPoints = true;

            calculated = merger.Calculate(lines).OrderBy(l2d => l2d.From.X).ToArray();

            Assert.AreEqual(4, calculated.Length);

            Assert.IsTrue(calculated[0].AlmostEqualTo(((1, 1), (2, 2))));
            Assert.IsTrue(calculated[1].AlmostEqualTo(((2, 2), (3, 3))));
            Assert.IsTrue(calculated[2].AlmostEqualTo(((3, 3), (4, 4))));
            Assert.IsTrue(calculated[3].AlmostEqualTo(((6, 6), (7, 7))));
        }
        [Test]
        public void ToleranceTest()
        {
            var merger = new LineMerger
            {
                Tolerance = 0.01,
                SplitAtOriginalEndPoints = false
            };

            var lines = new Line2d[]
            {
                ((0,1), (0,4.99998)),
                ((0,5), (0,8))
            };

            var calculated = merger.Calculate(lines).OrderBy(l2d => l2d.From.Y).ToArray();

            Assert.AreEqual(1, calculated.Length);
            Assert.IsTrue(calculated[0].AlmostEqualTo(((0, 1), (0, 8))));
        }
        [Test]
        public void LargeCoordinateTest()
        {
            const double VERY_LARGE_COORDINATE = 1e8;

            var translation = new Coord2d(VERY_LARGE_COORDINATE, VERY_LARGE_COORDINATE);

            var merger = new LineMerger
            {
                Tolerance = 0.01,
                SplitAtOriginalEndPoints = false
            };

            var lines = new Line2d[]
            {
                ((0,1), (0,4.99998)),
                ((0,5), (0,8))
            };

            lines[0].Translate(translation);
            lines[1].Translate(translation);

            var calculated = merger.Calculate(lines).OrderBy(l2d => l2d.From.Y).ToArray();

            Assert.AreEqual(1, calculated.Length);
            Assert.AreEqual(7, calculated[0].Length, 0.01);
        }
        [Test]
        public void DuplicateRemoveTest()
        {
            var merger = new LineMerger
            {
                Tolerance = 0.01,
                SplitAtOriginalEndPoints = false
            };

            var lines = Enumerable.Repeat(new Line2d((0, 1), (0, 5)), 10);

            var calculated = merger.Calculate(lines).ToArray();

            Assert.AreEqual(1, calculated.Length);
            Assert.IsTrue(calculated[0].AlmostEqualTo(((0, 1), (0, 5))));

            merger.SplitAtOriginalEndPoints = true;

            calculated = merger.Calculate(lines).ToArray();

            Assert.AreEqual(1, calculated.Length);
            Assert.IsTrue(calculated[0].AlmostEqualTo(((0, 1), (0, 5))));
        }
        [Test]
        public void DuplicateRemoveWithToleranceTest()
        {
            const double TOLERANCE = 0.01;

            var merger = new LineMerger
            {
                Tolerance = TOLERANCE,
                SplitAtOriginalEndPoints = false
            };

            const double TINY_LENGTH = 1e-7;

            var lines = Enumerable.Range(0, 10)
                .Select(i => new Line2d((0, 1 + TINY_LENGTH * i), (0, 5 + TINY_LENGTH * i)));

            var calculated = merger.Calculate(lines).ToArray();

            Assert.AreEqual(1, calculated.Length);
            Assert.IsTrue(calculated[0].AlmostEqualTo(((0, 1), (0, 5)), TOLERANCE));

            merger.SplitAtOriginalEndPoints = true;

            calculated = merger.Calculate(lines).ToArray();

            Assert.AreEqual(1, calculated.Length);
            Assert.IsTrue(calculated[0].AlmostEqualTo(((0, 1), (0, 5)), TOLERANCE));
        }
        [Test]
        public void DuplicateRemoveWithToleranceAndLargeCoordTest()
        {
            const double VERY_LARGE_COORDINATE = 1e8;
            const double TOLERANCE = 0.01;

            var translation = new Coord2d(VERY_LARGE_COORDINATE, VERY_LARGE_COORDINATE);

            var merger = new LineMerger
            {
                Tolerance = TOLERANCE,
                SplitAtOriginalEndPoints = false
            };

            const double TINY_LENGTH = 1e-7;

            var lines = Enumerable.Range(0, 10)
                .Select(i => new Line2d(
                    (0, 1 + TINY_LENGTH * i + VERY_LARGE_COORDINATE),
                    (0, 5 + TINY_LENGTH * i + VERY_LARGE_COORDINATE)
                    ));

            var calculated = merger.Calculate(lines).ToArray();

            Assert.AreEqual(1, calculated.Length);
            Assert.IsTrue(calculated[0].AlmostEqualTo(((0, 1 + VERY_LARGE_COORDINATE), (0, 5 + VERY_LARGE_COORDINATE)), TOLERANCE));

            merger.SplitAtOriginalEndPoints = true;

            calculated = merger.Calculate(lines).ToArray();

            Assert.AreEqual(1, calculated.Length);
            Assert.IsTrue(calculated[0].AlmostEqualTo(((0, 1 + VERY_LARGE_COORDINATE), (0, 5 + VERY_LARGE_COORDINATE)), TOLERANCE));
        }
    }
}
