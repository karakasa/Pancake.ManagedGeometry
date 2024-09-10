using NUnit.Framework;
using Pancake.ManagedGeometry.Algo.DataStructure;
using Pancake.ManagedGeometry.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Tests.AlgoTest
{
    public class OrderedListTest
    {
        [Test]
        public void Basic()
        {
            var list = OrderedList.Create<int>();

            Assert.DoesNotThrow(() => list.Add(5));

            Utility.AssertEquals(list.Count, 1);

            Assert.DoesNotThrow(() => list.Add(1));
            Assert.DoesNotThrow(() => list.Add(7));
            Assert.DoesNotThrow(() => list.Add(3));
            Assert.DoesNotThrow(() => list.Add(9));

            Utility.AssertEquals(list.Count, 5);
            Utility.AssertEquals(list.ToArray(), new double[] { 1, 3, 5, 7, 9 });

            Utility.AssertEquals(list.LowerBoundIndex(4), 2);

            Assert.DoesNotThrow(() => list.Add(4));

            Utility.AssertEquals(list.Count, 6);
            Utility.AssertEquals(list.ToArray(), new double[] { 1, 3, 4, 5, 7, 9 });
        }
    }
}
