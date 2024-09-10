using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pancake.ManagedGeometry;
using Pancake.ManagedGeometry.Utility;

namespace Pancake.ManagedGeometry.Tests.Line2dTest
{
    public class NearestPointPair
    {
        [Test]
        public void Basic()
        {
            var l1 = new Line2d((0, 0), (0, 2));
            var l2 = new Line2d((2, 1), (3, 1));

            var dist = l1.NearestPtToAnotherLine(l2, out var pt1, out var pt2);

            Assert.That((dist - 2).CloseToZero());
            Assert.That((pt1 - (0, 1)).Length.CloseToZero());
            Assert.That((pt2 - (2, 1)).Length.CloseToZero());
            Assert.That(l1.IsOnLine(pt1));
            Assert.That(l2.IsOnLine(pt2));
        }

        [Test]
        public void SkewedLine()
        {
            var l1 = new Line2d((4, 4), (0, 1));
            var l2 = new Line2d((-2, 0), (4, 7));

            var dist = l1.NearestPtToAnotherLine(l2, out var pt1, out var pt2);
            Assert.That((dist - 8 / Math.Sqrt(85)).CloseToZero());
            Assert.That((pt1 - (0, 1)).Length.CloseToZero());
            Assert.That((pt2 - (-56.0 / 85, 133.0 / 85)).Length.CloseToZero());
            Assert.That(l1.IsOnLine(pt1));
            Assert.That(l2.IsOnLine(pt2));
        }

        [Test]
        public void Intersection()
        {
            var l1 = new Line2d((-2, -2), (2, 2));
            var l2 = new Line2d((5, 1), (-3, 1));

            var dist = l1.NearestPtToAnotherLine(l2, out var pt1, out var pt2);

            Assert.That((dist).CloseToZero());
            Assert.That((pt1 - (1, 1)).Length.CloseToZero());
            Assert.That((pt2 - (1, 1)).Length.CloseToZero());
            Assert.That(l1.IsOnLine(pt1));
            Assert.That(l2.IsOnLine(pt2));
        }
        [Test]
        public void Parallel()
        {
            var l1 = new Line2d((1, 1), (5, 5));
            var l2 = new Line2d((3, 2), (7, 6));

            var dist = l1.NearestPtToAnotherLine(l2, out var pt1, out var pt2);

            double ExpectedLength = 1 / Math.Sqrt(2);

            Assert.That((dist - ExpectedLength).CloseToZero());
            Assert.That(((pt1 - pt2).Length - ExpectedLength).CloseToZero());
            Assert.That(l1.IsOnLine(pt1));
            Assert.That(l2.IsOnLine(pt2));
        }
        [Test]
        public void Colinear()
        {
            var l1 = new Line2d((1, 1), (5, 5));
            var l2 = new Line2d((2, 2), (6, 6));

            var dist = l1.NearestPtToAnotherLine(l2, out var pt1, out var pt2);

            double ExpectedLength = 0;

            Assert.That((dist - ExpectedLength).CloseToZero());
            Assert.That(((pt1 - pt2).Length - ExpectedLength).CloseToZero());
            Assert.That(l1.IsOnLine(pt1));
            Assert.That(l2.IsOnLine(pt2));
        }
    }
}
