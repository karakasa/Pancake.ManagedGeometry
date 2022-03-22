using NUnit.Framework;
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

        }
    }
}
