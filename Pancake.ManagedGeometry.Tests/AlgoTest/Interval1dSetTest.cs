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
        public void BasicTest()
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
        [Test]
        public void SubtractTest()
        {
            var baseSet = new Interval1dSet();
            baseSet.UnionWith((1, 2));
            Interval1dSet set;

            {
                set = baseSet.Clone();

                set.SubtractBy((1, 2));
                set.Compact();

                Assert.AreEqual(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((0, 2));
                set.Compact();

                Assert.AreEqual(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((1, 3));
                set.Compact();

                Assert.AreEqual(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((0, 3));
                set.Compact();

                Assert.AreEqual(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((3, 4));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((2, 3));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((0, 1));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((-1, 0));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((1.5, 1.6));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1, 1.5), (1.6, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((1, 1.4));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1.4, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((0, 1.4));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1.4, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((1.6, 2));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1, 1.6) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((1.6, 3));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1, 1.6) }, set.Intervals.ToArray());
            }
        }

        [Test]
        public void IntersectTest()
        {
            var baseSet = new Interval1dSet();
            baseSet.UnionWith((1, 2));
            Interval1dSet set;

            {
                set = baseSet.Clone();

                set.IntersectWith((1, 2));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((0, 2));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((1, 3));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((0, 3));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((3, 4));
                set.Compact();

                Assert.AreEqual(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((2, 3));
                set.Compact();

                Assert.AreEqual(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((0, 1));
                set.Compact();

                Assert.AreEqual(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((-1, 0));
                set.Compact();

                Assert.AreEqual(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((1.5, 1.6));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1.5, 1.6) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((1, 1.4));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1, 1.4) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((0, 1.4));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1, 1.4) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((1.6, 2));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1.6, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((1.6, 3));
                set.Compact();

                Assert.AreEqual(new Interval1d[] { (1.6, 2) }, set.Intervals.ToArray());
            }
        }
    }
}
