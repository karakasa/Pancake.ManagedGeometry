using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Algo.DataStructure
{
    public class OrderedListBasicImpl<T> : IOrderedList<T>
    {
        private List<T> _underlying;
        private IComparer<T> _comparer;
        internal OrderedListBasicImpl(IEnumerable<T> dataSrc, IComparer<T> comparer)
        {
            _comparer = comparer ?? Comparer<T>.Default;

            if (dataSrc is null)
            {
                _underlying = new();
            }
            else
            {
                _underlying = new(dataSrc);
                _underlying.Sort(_comparer);
            }
        }

        public int Count => _underlying.Count;

        public bool IsReadOnly => false;

        public void Add(T item)
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

        public bool Contains(T item) => _underlying.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _underlying.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => _underlying.GetEnumerator();

        public bool Remove(T item) => _underlying.Remove(item);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int LowerBoundIndex(T value)
        {
            var lo = 0;
            var hi = Count - 1;

            while (lo < hi)
            {
                var pivot = (hi + lo) >> 1;
                if (_comparer.Compare(_underlying[pivot], value) < 0)
                {
                    lo = pivot + 1;
                }
                else
                {
                    hi = pivot - 1;
                }
            }

            if (_comparer.Compare(_underlying[lo], value) < 0) 
                lo++;

            return lo;
        }
    }
}
