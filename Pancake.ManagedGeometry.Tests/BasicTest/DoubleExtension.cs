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
            Assert.That(!double.PositiveInfinity.IsFinite());
            Assert.That(!double.NegativeInfinity.IsFinite());
            Assert.That(!double.NaN.IsFinite());

            Assert.That(double.MaxValue.IsFinite());
            Assert.That(double.MinValue.IsFinite());
            Assert.That(double.Epsilon.IsFinite());
            Assert.That(0.0.IsFinite());
            Assert.That(123456.456.IsFinite());
            Assert.That((-123456.456).IsFinite());
        }
    }
}
