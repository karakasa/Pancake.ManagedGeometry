﻿using Pancake.ManagedGeometry.Factory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Algo.DataStructure
{
    internal sealed class OrderedListBasicImpl<TValue, TComparer> : IOrderedList<TValue>
        where TComparer : IComparer<TValue>
    {
        private List<TValue> _underlying;
        internal List<TValue> UnderlyingList => _underlying;
        public TComparer Comparer { get; set; }
        public OrderedListBasicImpl(IEnumerable<TValue> dataSrc = null, TComparer comparer = default)
        {
            Comparer = comparer;

            if (dataSrc is null)
            {
                _underlying = new();
            }
            else
            {
                _underlying = new(dataSrc);
                _underlying.Sort(Comparer);
            }
        }

        public static OrderedListBasicImpl<TValue, TComparer> CreateWithSortedData(IEnumerable<TValue> dataSrc, TComparer comparer = default)
        {
            var list = new OrderedListBasicImpl<TValue, TComparer>(null, comparer);
            list._underlying.AddRange(dataSrc);
            return list;
        }

        public int Count => _underlying.Count;

        public bool IsReadOnly => false;

        public void Add(TValue item)
        {
            if (Count == 0)
            {
                _underlying.Add(item);
                return;
            }

            var index = LowerBoundIndex(item);
            _underlying.Insert(index, item);
        }

        public void Clear() => _underlying.Clear();

        public bool Contains(TValue item) => _underlying.Contains(item);

        public void CopyTo(TValue[] array, int arrayIndex) => _underlying.CopyTo(array, arrayIndex);

        public IEnumerator<TValue> GetEnumerator() => _underlying.GetEnumerator();

        public bool Remove(TValue item) => _underlying.Remove(item);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Find the index where <paramref name="value"/> should be inserted at.
        /// </summary>
        /// <param name="value">Value to insert</param>
        /// <returns>Index</returns>
        public int LowerBoundIndex(TValue value)
        {
            var lo = 0;
            var hi = Count - 1;

            while (lo < hi)
            {
                var pivot = (hi + lo) >> 1;
                if (Comparer.Compare(_underlying[pivot], value) < 0)
                {
                    lo = pivot + 1;
                }
                else
                {
                    hi = pivot - 1;
                }
            }

            if (Comparer.Compare(_underlying[lo], value) < 0) 
                lo++;

            return lo;
        }
        public int LowerBoundIndex<TConverted, TConvertedComparer>(
            TConverted value,
            Func<TValue, TConverted> selector,
            TConvertedComparer comparer
            )
            where TConvertedComparer : IComparer<TConverted>
        {
            var lo = 0;
            var hi = Count - 1;

            while (lo < hi)
            {
                var pivot = (hi + lo) >> 1;
                if (comparer.Compare(selector(_underlying[pivot]), value) < 0)
                {
                    lo = pivot + 1;
                }
                else
                {
                    hi = pivot - 1;
                }
            }

            if (comparer.Compare(selector(_underlying[lo]), value) < 0)
                lo++;

            return lo;
        }

        internal void ReplaceUnderlyingList(List<TValue> list)
        {
            _underlying = list;
        }
    }
}
