using NUnit.Framework;
using Pancake.ManagedGeometry.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Tests.AlgoTest
{
    public class ComplexInterval1dSetTest
    {
        private static Interval1dSet GetBaseSet()
        {
            var baseSet = new Interval1dSet();


            for (var i = 0; i < 10; i++)
            {
                baseSet.UnionWith((1 + i * 0.1, 1 + i * 0.1 + 0.1));
            }

            return baseSet;
        }
        [Test]
        public void BaseSetConformityTest()
        {
            var set = GetBaseSet();
            set.Compact();
            Utility.AssertEquals(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
        }
        [Test]
        public void SubtractTest()
        {
            var baseSet = GetBaseSet();
            
            Interval1dSet set;

            {
                set = baseSet.Clone();

                set.SubtractBy((1, 2));
                set.Compact();

                Utility.AssertEquals(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((0, 2));
                set.Compact();

                Utility.AssertEquals(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((1, 3));
                set.Compact();

                Utility.AssertEquals(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((0, 3));
                set.Compact();

                Utility.AssertEquals(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((3, 4));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((2, 3));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((0, 1));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((-1, 0));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((1.5, 1.6));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 1.5), (1.6, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((1, 1.4));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1.4, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((0, 1.4));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1.4, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((1.6, 2));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 1.6) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.SubtractBy((1.6, 3));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 1.6) }, set.Intervals.ToArray());
            }
        }

        [Test]
        public void IntersectTest()
        {
            var baseSet = GetBaseSet();
            Interval1dSet set;

            {
                set = baseSet.Clone();

                set.IntersectWith((1, 2));
                set.Compact();

                // BUG

                Utility.AssertEquals(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((0, 2));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((1, 3));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((0, 3));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((3, 4));
                set.Compact();

                Utility.AssertEquals(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((2, 3));
                set.Compact();

                Utility.AssertEquals(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((0, 1));
                set.Compact();

                Utility.AssertEquals(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((-1, 0));
                set.Compact();

                Utility.AssertEquals(0, set.Count);
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((1.5, 1.6));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1.5, 1.6) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((1, 1.4));
                set.Compact();

                Utility.AssertEquals(1, set.Count);

                var iv = set.Intervals.First();

                Utility.AssertEquals(1, iv.From, 1e-7);
                Utility.AssertEquals(1.4, iv.To, 1e-7);
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((0, 1.4));
                set.Compact();

                Utility.AssertEquals(1, set.Count);

                var iv = set.Intervals.First();

                Utility.AssertEquals(1, iv.From, 1e-7);
                Utility.AssertEquals(1.4, iv.To, 1e-7);
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((1.6, 2));
                set.Compact();

                Utility.AssertEquals(1, set.Count);

                var iv = set.Intervals.First();

                Utility.AssertEquals(1.6, iv.From, 1e-7);
                Utility.AssertEquals(2, iv.To, 1e-7);
            }

            {
                set = baseSet.Clone();

                set.IntersectWith((1.6, 3));
                set.Compact();

                Utility.AssertEquals(1, set.Count);

                var iv = set.Intervals.First();

                Utility.AssertEquals(1.6, iv.From, 1e-7);
                Utility.AssertEquals(2, iv.To, 1e-7);
            }
        }
        [Test]
        public void UnionTest()
        {
            var baseSet = GetBaseSet();
            Interval1dSet set;

            {
                set = baseSet.Clone();

                set.UnionWith((1, 2));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.UnionWith((0, 2));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (0, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.UnionWith((1, 3));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 3) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.UnionWith((0, 3));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (0, 3) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.UnionWith((3, 4));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 2), (3, 4) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.UnionWith((2, 3));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 3) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.UnionWith((0, 1));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (0, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.UnionWith((-1, 0));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (-1, 0), (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.UnionWith((1.5, 1.6));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.UnionWith((1, 1.4));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.UnionWith((0, 1.4));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (0, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.UnionWith((1.6, 2));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 2) }, set.Intervals.ToArray());
            }

            {
                set = baseSet.Clone();

                set.UnionWith((1.6, 3));
                set.Compact();

                Utility.AssertEquals(new Interval1d[] { (1, 3) }, set.Intervals.ToArray());
            }
        }
    }
}
