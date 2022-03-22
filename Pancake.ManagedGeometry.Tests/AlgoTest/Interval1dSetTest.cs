using NUnit.Framework;
using Pancake.ManagedGeometry.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Tests.AlgoTest
{
    public class Interval1dSetTest
    {
        [Test]
        public void SimpleCase()
        {
            var set = new Interval1dSet();

            Assert.AreEqual(set.Count, 0);

            set.UnionWith((1, 2));

            Assert.AreEqual(set.Count, 1);

            set.UnionWith((3, 4));

            Assert.AreEqual(set.Count, 2);

            set.UnionWith((1.5, 3.5));
            set.Compact();

            Assert.AreEqual(set.Count, 1);
            Assert.AreEqual(set.Intervals.ToArray(), new Interval1d[] { (1, 4) });

            set.UnionWith((-8, 10));
            set.Compact();

            Assert.AreEqual(set.Count, 1);
            Assert.AreEqual(set.Intervals.ToArray(), new Interval1d[] { (-8, 10) });
        }
        [Test]
        public void CaseWithTolerance()
        {
            var set = new Interval1dSet(0.01);

            set.UnionWith((1, 2));
            set.UnionWith((3, 4));

            Assert.AreEqual(set.Count, 2);

            set.UnionWith((2.00001, 3.5));
            set.Compact();

            Assert.AreEqual(set.Count, 1);
            Assert.AreEqual(set.Intervals.ToArray(), new Interval1d[] { (1, 4) });
        }
        [Test]
        public void InfiniteTest()
        {
            var set = new Interval1dSet();
            set.UnionWith((1, double.PositiveInfinity));
            set.UnionWith((-1, 2));

            set.Compact();

            Assert.AreEqual(set.Count, 1);
            Assert.AreEqual(set.Intervals.ToArray(), new Interval1d[] { (-1, double.PositiveInfinity) });
        }
        [Test]
        public void RemoveDuplicateTest()
        {
            var iv = new Interval1d(1, 2);

            var set = new Interval1dSet();
            set.UnionWith(iv);
            set.UnionWith(iv);

            Assert.AreEqual(set.Count, 1);
            Assert.AreEqual(set.Intervals.ToArray(), new[] { iv });
        }
    }
}
