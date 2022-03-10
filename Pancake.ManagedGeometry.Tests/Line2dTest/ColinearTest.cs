using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Tests.Line2dTest
{
    public class ColinearTest
    {
        [Test]
        public void Bug20220309()
        {
            var a = new Line2d((11538.83910841442, 6687.1407661178728), (11538.83910841442, 6687.1407661410976));
            var b = new Line2d((10638.83910840534, 2687.1407661692119), (13738.83910840534, 2687.1407661692119));

            Assert.IsFalse(a.IsValid());
            Assert.IsTrue(b.IsValid());
        }
    }
}
