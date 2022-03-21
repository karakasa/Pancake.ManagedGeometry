using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pancake.ManagedGeometry.Utility;

namespace Pancake.ManagedGeometry.Tests.BasicTest
{
    public class DoubleExtension
    {
        [Test]
        public void DoubleValidity()
        {
            Assert.IsFalse(double.PositiveInfinity.IsFinite());
            Assert.IsFalse(double.NegativeInfinity.IsFinite());
            Assert.IsFalse(double.NaN.IsFinite());

            Assert.IsTrue(double.MaxValue.IsFinite());
            Assert.IsTrue(double.MinValue.IsFinite());
            Assert.IsTrue(double.Epsilon.IsFinite());
            Assert.IsTrue(0.0.IsFinite());
            Assert.IsTrue(123456.456.IsFinite());
            Assert.IsTrue((-123456.456).IsFinite());
        }
    }
}
