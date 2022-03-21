using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Algo.DataStructure
{
    public static class OrderedList
    {
        public static IOrderedList<T> Create<T>(IEnumerable<T> data = null, IComparer<T> comparer = null)
        {
            return new OrderedListBasicImpl<T>(data, comparer);
        }
    }
}
