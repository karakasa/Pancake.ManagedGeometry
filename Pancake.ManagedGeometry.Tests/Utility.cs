using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Tests
{
    internal static class Utility
    {
        public static void AssertEquals(object a, object b)
        {
            Assert.That(a, Is.EqualTo(b));
        }
        public static void AssertEquals(double a, double b, double tol)
        {
            Assert.That(a, Is.EqualTo(b).Within(tol));
        }
    }
}
