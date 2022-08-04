using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pancake.ManagedGeometry.Algo
{
    public static class Distincter
    {
        public static int DistinctSortedArrayInplace<TValue, TComparer>(TValue[] array, TComparer comparer)
            where TComparer : IComparer<TValue>
            => DistinctSortedArrayInplaceInternal(array, comparer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int DistinctSortedArrayInplaceInternal<TValue, TComparer>(TValue[] array, TComparer comparer)
            where TComparer : IComparer<TValue>
        {
            if (array.Length <= 1) 
                return array.Length;

            var result = 0;
            for (var first = 1; first < array.Length; ++first)
            {
                var vFirst = array[first];
                if (comparer.Compare(array[result], vFirst) != 0 && ++result != first)
                    array[result] = vFirst;
            }
            return ++result;
        }
        /// <summary>
        /// Sort and distinct array values in place by comparers.
        /// You should provide a same comparer in both struct and class form for best performance.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TComparer"></typeparam>
        /// <typeparam name="TComparerClass"></typeparam>
        /// <param name="array"></param>
        /// <param name="comparer"></param>
        /// <param name="comparer2"></param>
        /// <returns></returns>
        public static int SortAndDistinctArrayInplace<TValue, TComparer, TComparerClass>
            (TValue[] array, TComparer comparer, TComparerClass comparer2)
            where TComparer : IComparer<TValue>
            where TComparerClass : class, IComparer<TValue>
        {
            if (array is null) return 0;

            Array.Sort(array, comparer2);

            return DistinctSortedArrayInplaceInternal(array, comparer);
        }
    }
}