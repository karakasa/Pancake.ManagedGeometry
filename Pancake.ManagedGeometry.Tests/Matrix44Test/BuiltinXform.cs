using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pancake.ManagedGeometry.Utility;

namespace Pancake.ManagedGeometry.Tests.Matrix44Test
{
    public class BuiltinXform
    {
        [Test]
        public void Translation()
        {
            var matrix = Matrix44.Translation(new Coord(4, 5, 6));
            var vec = new Vector4(1, 2, 3);
            var vec2 = matrix * vec;

            Assert.IsTrue((vec2.X - 5).CloseToZero());
            Assert.IsTrue((vec2.Y - 7).CloseToZero());
            Assert.IsTrue((vec2.Z - 9).CloseToZero());
            Assert.IsTrue((vec2.W - vec.W).CloseToZero());
        }
    }
}
