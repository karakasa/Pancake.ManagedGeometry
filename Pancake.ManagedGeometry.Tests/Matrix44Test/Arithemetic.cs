using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pancake.ManagedGeometry.Utility;

namespace Pancake.ManagedGeometry.Tests.Matrix44Test
{
    public class Arithemetic
    {
        [Test]
        public void Basic()
        {
            var matrix = Matrix44.Identity;
            var vec = new Vector4(1, 2, 3);
            var vec2 = matrix * vec;

            Assert.IsTrue((matrix * matrix * matrix).SimilarTo(matrix));
            Assert.IsTrue((vec2.X - vec.X).CloseToZero());
            Assert.IsTrue((vec2.Y - vec.Y).CloseToZero());
            Assert.IsTrue((vec2.Z - vec.Z).CloseToZero());
            Assert.IsTrue((vec2.W - vec.W).CloseToZero());
        }
        [Test]
        public void Inverse()
        {
            var array = Enumerable.Range(1, 16).Select(i => (i % 5 + i % 7) + 0.0).ToArray();
            var matrix = Matrix44.CreateByRowArray(array);

            Matrix44 matrix2 = ((-(1.0 / 2), 5.0 / 26, 15.0 / 26, -(7.0 / 26)),
                (-(1.0 / 2), 1.0 / 2, 1.0 / 2, -(1.0 / 2)),
                (-4.0, 59.0 /  26, 125.0 / 26, -(93.0 / 26)), 
                (7.0 / 2, -2.0, -4.0, 3.0));

            Assert.IsTrue(matrix.Inverse().SimilarTo(matrix2));
        }
        [Test]
        public void InverseUnavailable()
        {
            var array = Enumerable.Range(1, 16).Select(i => 1.0 * i).ToArray();
            var matrix = Matrix44.CreateByRowArray(array);

            Assert.Throws<InvalidOperationException>(() => matrix.Inverse());
            Assert.IsFalse(matrix.TryGetInverse(out _));
        }
        [Test]
        public void Determinant()
        {
            var array = Enumerable.Range(1, 16).Select(i => (i % 3 + i % 5) + 0.0).ToArray();
            var matrix = Matrix44.CreateByRowArray(array);

            Assert.IsTrue((matrix.Determinant() - 144.0).CloseToZero());
        }
    }
}
