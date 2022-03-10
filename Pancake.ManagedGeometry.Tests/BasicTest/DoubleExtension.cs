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
            Assert.IsFalse(double.PositiveInfinity.IsValid());
            Assert.IsFalse(double.NegativeInfinity.IsValid());
            Assert.IsFalse(double.NaN.IsValid());

            Assert.IsTrue(double.MaxValue.IsValid());
            Assert.IsTrue(double.MinValue.IsValid());
            Assert.IsTrue(double.Epsilon.IsValid());
            Assert.IsTrue(0.0.IsValid());
            Assert.IsTrue(123456.456.IsValid());
            Assert.IsTrue((-123456.456).IsValid());
        }
    }
}
