using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pancake.ManagedGeometry.Utility;

namespace Pancake.ManagedGeometry.Tests.BasicTest
{
    public class Interval1dTest
    {
        [Test]
        public void Contains()
        {
            var interval = new Interval1d(2.0, 3.0);

            Assert.That(interval.Contains(2.5));
            Assert.That(!interval.Contains(1.9));
            Assert.That(!interval.Contains(3.1));

            Assert.That(interval.Contains(1.99999, 0.01));
            Assert.That(interval.Contains(3.00001, 0.01));

            Assert.That(interval.ContainsOpen(2.5, 0.01));
            Assert.That(!interval.ContainsOpen(1.99999, 0.01));
            Assert.That(!interval.ContainsOpen(2.00001, 0.01));
            Assert.That(!interval.ContainsOpen(2.99999, 0.01));
            Assert.That(!interval.ContainsOpen(3.00001, 0.01));

            var iv1 = new Interval1d(2.00001, 2.99999);
            var iv2 = new Interval1d(1.99999, 3.00001);

            Assert.That(interval.Contains(iv1, 1e-9));
            Assert.That(interval.Contains(iv1, 0.01));
            Assert.That(!interval.Contains(iv2, 1e-9));
            Assert.That(interval.Contains(iv2, 0.01));
        }

        [Test]
        public void IntervalSplitter()
        {
            var intervals = new Interval1d[] { (2, 3), (4, 5), (6, 7) };

            var splitted = intervals.SplitAt(new[] { 2.5, 3.5, 6.5 }).ToArray();
            Utility.AssertEquals(splitted, new Interval1d[] {
                (2,2.5),
                (2.5,3),
                (4,5),
                (6,6.5),
                (6.5,7)
            });

            intervals = new Interval1d[] { (2, 3), (4, 5),(5,6), (6, 7) };

            splitted = intervals.SplitAt(new[] { 2.00001, 2.5, 3.5, 6.5, 3.00001 }, 0.01).ToArray();
            Utility.AssertEquals(splitted, new Interval1d[] {
                (2,2.5),
                (2.5,3),
                (4,5),
                (5,6),
                (6,6.5),
                (6.5,7)
            });
        }
    }
}
