using Pancake.ManagedGeometry;
using Pancake.ManagedGeometry.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pancake.ModernUtility
{
    internal class Test
    {
        public void A(bool a)
        {
            PolygonUtilities.EdgeIntersectWith<Polygon, Polygon>(default, default, a);
        }
    }
}
