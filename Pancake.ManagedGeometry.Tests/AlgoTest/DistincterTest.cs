using NUnit.Framework;
using Pancake.ManagedGeometry.Algo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ManagedGeometry.Tests.AlgoTest
{
    public class DistincterTest
    {
        [Test]
        public void IntArrayInplaceTest()
        {
            var intArray = new int[] { 2, 4, 6, 23, 7, 4, 1, 7, 2 };

            var sorted = intArray.OrderBy(x => x).Distinct().ToArray();
            var newIndex = Distincter.SortAndDistinctArrayInplace(intArray, Comparer<int>.Default, Comparer<int>.Default);

            var sorted2 = new ArraySegment<int>(intArray, 0, newIndex).ToArray();

            Assert.AreEqual(sorted, sorted2);
        }
    }
}
