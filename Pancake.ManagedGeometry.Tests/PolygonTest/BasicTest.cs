using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Tests.PolygonTest
{
    public class BasicTest
    {
        [Test]
        public void EdgeTest()
        {
            Polygon ply;

            ply = Polygon.CreateByCoords(
                (0, 0),
                (1, 0),
                (1, 1),
                (0, 1)
                );

            foreach (var it in ply.Edges)
            {
                ;
            }
        }
    }
}
