using Pancake.ManagedGeometry.Algo.DataStructure;
using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Pancake.ManagedGeometry.Factory;
using System.Diagnostics;

namespace Pancake.ManagedGeometry.Algo
{
    public sealed class Interval1dSet : IInterval1dSet, ICloneable
    {
        private readonly EpsilonComparerStruct _dblComparer;

        private readonly OrderedListBasicImpl
            <Interval1d, Interval1dComparer> _orderedList;
        private readonly double _tolerance;
        /// <summary>
        /// Get the tolerance of the current set. You can operate on two sets of different tolerances.
        /// The new set will follow the setting of the first operand.
        /// </summary>
        public double Tolerance => _tolerance;

        public Interval1dSet() : this(MathUtils.ZeroTolerance)
        {

        }

        public Interval1dSet(double tolerance)
        {
            _tolerance = tolerance;
            _dblComparer = new(_tolerance);
            _orderedList = new(null, new(_tolerance));
        }
        public Interval1dSet(IEnumerable<Interval1d> set) : this()
        {
            AddEnumerable(set);
        }
        public Interval1dSet(double tolerance, IEnumerable<Interval1d> set) : this(tolerance)
        {
            AddEnumerable(set);
        }

        public Interval1dSet(Interval1d iv) : this()
        {
            _orderedList.Add(iv);
        }
        public Interval1dSet(double tolerance, Interval1d iv) : this(tolerance)
        {
            _orderedList.Add(iv);
        }
        private void AddEnumerable(IEnumerable<Interval1d> set)
        {
            foreach (var it in set)
                UnionWith(it);
        }
        private Interval1dSet(Interval1dSet another)
        {
            _tolerance = another._tolerance;
            _dblComparer = new(_tolerance);
            _orderedList = OrderedListBasicImpl<Interval1d,
                Interval1dComparer>.CreateWithSortedData(
                another._orderedList.UnderlyingList, new(_tolerance));
        }
        /// <summary>
        /// Get a deep copy of the set.
        /// </summary>
        /// <returns></returns>
        public Interval1dSet Clone()
        {
            return new Interval1dSet(this);
        }
        /// <summary>
        /// Clear the set.
        /// </summary>
        public void Clear()
        {
            _orderedList.Clear();
        }

        /// <summary>
        /// Add <paramref name="intervals"/> to the underlying data structure without any check.
        /// <paramref name="intervals"/> must be non-overlapping, ordered from small to large and the minimal one must follow the largest existing interval.
        /// Otherwise the set's behavior will become unpredictable.
        /// This method is designed for fast creation directly after the set is created.
        /// </summary>
        /// <param name="intervals">Intervals to add</param>
        public Interval1dSet AddUnchecked(IEnumerable<Interval1d> intervals)
        {
            _orderedList.UnderlyingList.AddRange(intervals);
            return this;
        }
        /// <summary>
        /// Create the complement set against (-∞, ∞).
        /// </summary>
        /// <returns></returns>
        public Interval1dSet CreateComplement()
            => CreateComplement((double.NegativeInfinity, double.PositiveInfinity));
        /// <summary>
        /// Create the complement set against a designated range.
        /// When the universe is another set, use <see cref="SubtractBy(IInterval1dSet)"/>.
        /// </summary>
        /// <param name="totalRange"></param>
        /// <returns></returns>
        public Interval1dSet CreateComplement(Interval1d totalRange)
        {
            var set = new Interval1dSet(totalRange);
            set.SubtractBy(this);
            set.Compact();

            return set;
        }
        /// <summary>
        /// Get the count of intervals in this set.
        /// </summary>
        public int Count => _orderedList.Count;
        /// <summary>
        /// Get or set the capacity of the underlying data structure.
        /// </summary>
        public int Capacity
        {
            get => _orderedList.UnderlyingList.Capacity;
            set => _orderedList.UnderlyingList.Capacity = value;
        }
        private void CheckIntervalArgument(Interval1d interval)
        {
            if (interval.Length < _tolerance)
                throw new ArgumentException("Interval smaller than tolerance.");

            if (interval.From > interval.To)
                throw new ArgumentException("Flipped interval.");
        }
        //public void UnionWithOld(Interval1d interval)
        //{
        //    if (interval.Length < _tolerance)
        //        ThrowTooSmallInterval();

        //    if (_orderedList.Count == 0)
        //    {
        //        _orderedList.Add(interval);
        //        return;
        //    }

        //    FindCharacteristicPoint(interval);

        //    var list = _orderedList.UnderlyingList;

        //    if (_tempListForSplitPoints.Count == 0)
        //    {
        //        if (list.All(iv => !iv.Contains(interval, _tolerance)))
        //            _orderedList.Add(interval);

        //        return;
        //    }

        //    var splitPoints = _tempListForSplitPoints.DistinctByComparer(_dblComparer);

        //    _tempListForSegmentToAdd.Clear();

        //    _tempArrayInterval[0] = interval;
        //    foreach (var segment in _tempArrayInterval.SplitAtSorted(splitPoints))
        //    {
        //        if (list.All(iv => !iv.Contains(segment, _tolerance)))
        //            _tempListForSegmentToAdd.Add(segment);
        //    }

        //    foreach (var it in _tempListForSegmentToAdd)
        //        _orderedList.Add(it);

        //    _tempListForSegmentToAdd.Clear();
        //    _tempListForSplitPoints.Clear();
        //}
        // public void Add(Interval1d interval) => UnionWith(interval);
        public Interval1dSet UnionWith(Interval1d interval)
        {
            CheckIntervalArgument(interval);

            if (_orderedList.Count == 0
                || !DetermineEndInformation(interval, out var operationAtStart, out var operationAtEnd,
                out var startIndex, out var endIndex))
            {
                _orderedList.Add(interval);
                return this;
            }

            var list = _orderedList.UnderlyingList;

            if (operationAtStart == EndOperation.OutsideEnd
                    && operationAtEnd == EndOperation.OutsideEnd)
            {
                list[startIndex] = interval;
                if (endIndex > startIndex)
                    list.RemoveRange(startIndex + 1, endIndex - startIndex);
                return this;
            }

            if (operationAtStart == EndOperation.InsideInterval
                    && operationAtEnd == EndOperation.OutsideEnd)
            {
                list[startIndex] = (list[startIndex].From, interval.To);
                if (endIndex > startIndex)
                    list.RemoveRange(startIndex + 1, endIndex - startIndex);
                return this;
            }

            if (operationAtStart == EndOperation.OutsideEnd
                && operationAtEnd == EndOperation.InsideInterval)
            {
                list[startIndex] = (interval.From, list[endIndex].To);
                if (endIndex > startIndex)
                    list.RemoveRange(startIndex + 1, endIndex - startIndex);
                return this;
            }

            if (endIndex > startIndex)
            {
                list[startIndex] = (list[startIndex].From, list[endIndex].To);
                list.RemoveRange(startIndex + 1, endIndex - startIndex);
            }

            return this;
        }
        //private void SubtractByCopied(Interval1d interval)
        //{
        //    if (interval.Length < _tolerance)
        //        ThrowTooSmallInterval();

        //    if (_orderedList.Count == 0)
        //    {
        //        return;
        //    }

        //    var list = _orderedList.UnderlyingList;
        //    var list2 = new List<Interval1d>();

        //    var from = interval.From;
        //    var to = interval.To;

        //    foreach (var it in list)
        //    {
        //        var containsFrom = it.Contains(from, _tolerance);
        //        var containsTo = it.Contains(to, _tolerance);

        //        if (!containsFrom && !containsTo)
        //        {
        //            if (from < it.From && to > it.To)
        //            {

        //            }
        //            else
        //            {
        //                list2.Add(it);
        //            }

        //            continue;
        //        }

        //        if (containsFrom && !containsTo)
        //        {
        //            AddToList(list2, it.From, from);
        //            continue;
        //        }

        //        if (!containsFrom && containsTo)
        //        {
        //            AddToList(list2, to, it.To);
        //            continue;
        //        }

        //        if (containsFrom && containsTo)
        //        {
        //            AddToList(list2, it.From, from);
        //            AddToList(list2, to, it.To);
        //        }
        //    }

        //    _orderedList.ReplaceUnderlyingList(list2);
        //}
        //private void AddToList(List<Interval1d> list, double from, double to)
        //{
        //    if (to.CloseToExtended(from, _tolerance)) return;
        //    list.Add(new Interval1d(from, to));
        //}
        private enum EndOperation
        {
            Undefined,
            OutsideEnd,
            InsideInterval
        }

        private bool DetermineEndInformation(Interval1d interval,
            out EndOperation operationAtStart, out EndOperation operationAtEnd,
            out int startIndex, out int endIndex)
        {
            const int LIST_LENGTH_HEURISTIC_FOR_BINARY_SEARCH = 16;

            var list = _orderedList.UnderlyingList;
            var listCnt = list.Count;
            var useBinary = listCnt > LIST_LENGTH_HEURISTIC_FOR_BINARY_SEARCH;

            operationAtStart = EndOperation.Undefined;
            operationAtEnd = EndOperation.Undefined;

            startIndex = -1;
            endIndex = -1;

            int searchStart = 0;

            if (useBinary)
            {
                searchStart = _orderedList
                    .LowerBoundIndex(interval.From, static iv => iv.From, _dblComparer) - 1;

                if (searchStart < 0)
                    searchStart = 0;
            }
            else
            {
                searchStart = 0;
            }

            for (var i = searchStart; i < listCnt; i++)
            {
                if (list[i].From.CloseTo(interval.From, _tolerance)
                    || list[i].From >= interval.From)
                {
                    startIndex = i;
                    operationAtStart = EndOperation.OutsideEnd;
                    break;
                }

                if (list[i].ContainsOpen(interval.From, _tolerance))
                {
                    startIndex = i;
                    operationAtStart = EndOperation.InsideInterval;
                    break;
                }
            }

            if (startIndex == -1)
            {
                // The interval is larger than the largest in the current set
                return false;
            }

            if (useBinary)
            {
                searchStart = _orderedList
                    .LowerBoundIndex(interval.To, static iv => iv.To, _dblComparer) - 1;

                if (searchStart < startIndex)
                    searchStart = startIndex;
            }
            else
            {
                searchStart = startIndex;
            }

            for (var i = searchStart; i < listCnt; i++)
            {
                if (list[i].To.CloseTo(interval.To, _tolerance)
                    || (list[i].To <= interval.To
                    && (i == listCnt - 1 || list[i + 1].From >= interval.To - _tolerance)))
                {
                    endIndex = i;
                    operationAtEnd = EndOperation.OutsideEnd;
                    break;
                }

                if (list[i].ContainsOpen(interval.To, _tolerance))
                {
                    endIndex = i;
                    operationAtEnd = EndOperation.InsideInterval;
                    break;
                }
            }

            if (endIndex == -1)
            {
                // The interval is smaller than the smallest in the current set
                return false;
            }

            return true;
        }
        public Interval1dSet SubtractBy(Interval1d interval)
        {
            CheckIntervalArgument(interval);

            if (_orderedList.Count == 0)
            {
                return this;
            }

            if (!DetermineEndInformation(interval, out var operationAtStart, out var operationAtEnd,
                out var startIndex, out var endIndex))
                return this;

            // Debug.Assert(operationAtStart == SubtractionEndOperation.Undefined);
            // Debug.Assert(operationAtEnd == SubtractionEndOperation.Undefined);

            var list = _orderedList.UnderlyingList;

            if (operationAtStart == EndOperation.OutsideEnd
                    && operationAtEnd == EndOperation.OutsideEnd)
            {
                list.RemoveRange(startIndex, endIndex - startIndex + 1);
                return this;
            }

            if (operationAtStart == EndOperation.InsideInterval
                && operationAtEnd == EndOperation.OutsideEnd)
            {
                list[startIndex] = (list[startIndex].From, interval.From);
                if (endIndex > startIndex)
                    list.RemoveRange(startIndex + 1, endIndex - startIndex);
                return this;
            }

            if (operationAtStart == EndOperation.OutsideEnd
                && operationAtEnd == EndOperation.InsideInterval)
            {
                list[endIndex] = (interval.To, list[endIndex].To);
                list.RemoveRange(startIndex, endIndex - startIndex);
                return this;
            }

            // if (operationAtStart == SubtractionEndOperation.Manipulate
            //        && operationAtEnd == SubtractionEndOperation.Manipulate)

            Interval1d it1 = (list[startIndex].From, interval.From);
            Interval1d it2 = (interval.To, list[endIndex].To);

            list[startIndex] = it1;

            if (startIndex == endIndex)
            {
                list.Insert(startIndex + 1, it2);
            }
            else
            {
                var removeCnt = endIndex - startIndex - 1;
                list[endIndex] = it2;
                if (removeCnt > 0)
                    list.RemoveRange(startIndex + 1, removeCnt);
            }

            return this;
        }
        public Interval1dSet IntersectWith(Interval1d interval)
        {
            CheckIntervalArgument(interval);

            if (_orderedList.Count == 0
                || !DetermineEndInformation(interval, out var operationAtStart, out var operationAtEnd,
                out var startIndex, out var endIndex))
            {
                _orderedList.Clear();

                return this;
            }

            var list = _orderedList.UnderlyingList;

            if (operationAtStart == EndOperation.OutsideEnd
                    && operationAtEnd == EndOperation.OutsideEnd)
            {
                goto removeOtherIntervals;
            }

            if (operationAtStart == EndOperation.InsideInterval
                    && operationAtEnd == EndOperation.OutsideEnd)
            {
                list[startIndex] = (interval.From, list[startIndex].To);
                goto removeOtherIntervals;
            }

            if (operationAtStart == EndOperation.OutsideEnd
                && operationAtEnd == EndOperation.InsideInterval)
            {
                list[endIndex] = (list[endIndex].From, interval.To);
                goto removeOtherIntervals;
            }

            if (endIndex == startIndex)
            {
                list[0] = interval;
                list.RemoveRange(1, list.Count - 1);

                return this;
            }
            else
            {
                list[startIndex] = (interval.From, list[startIndex].To);
                list[endIndex] = (list[endIndex].From, interval.To);
            }

            removeOtherIntervals:

            if (endIndex != list.Count - 1)
                list.RemoveRange(endIndex + 1, list.Count - endIndex - 1);
            list.RemoveRange(0, startIndex);

            return this;
        }

        public Interval1dSet UnionWith(IInterval1dSet set)
        {
            foreach (var it in set.Intervals)
                UnionWith(it);

            return this;
        }
        
        public Interval1dSet SubtractBy(IInterval1dSet set)
        {
            foreach (var it in set.Intervals)
                SubtractBy(it);

            return this;
        }
        
        public Interval1dSet IntersectWith(IInterval1dSet set)
        {
            foreach (var it in set.Intervals)
                IntersectWith(it);

            return this;
        }

        ///// <summary>
        ///// Find split points in <paramref name="interval"/>, according to existing intervals in the set.
        ///// </summary>
        ///// <param name="interval"></param>
        //private void FindCharacteristicPoint(Interval1d interval)
        //{
        //    var list = _orderedList.UnderlyingList;
        //    _tempListForSplitPoints.Clear();

        //    for (var i = 0; i < list.Count; i++)
        //    {
        //        var pt = list[i].From;

        //        if (interval.ContainsOpen(pt, _tolerance))
        //            _tempListForSplitPoints.Add(pt);

        //        pt = list[i].To;

        //        if (interval.ContainsOpen(pt, _tolerance))
        //            _tempListForSplitPoints.Add(pt);
        //    }
        //}

        /// <summary>
        /// Compact continous segments into one.
        /// This may have no effect depending on implementation.
        /// </summary>
        public Interval1dSet Compact()
        {
            if (_orderedList.Count <= 1) return this;

            var list = _orderedList.UnderlyingList;
            for (var i = 0; i < list.Count; i++)
            {
                var found = false;
                var curLast = list[i].To;
                var j = i + 1;

                for (; j < list.Count; j++)
                {
                    if ((list[j].From - curLast).CloseToZero(_tolerance))
                    {
                        found = true;
                        curLast = list[j].To;
                    }
                    else
                    {
                        break;
                    }
                }

                if (found)
                {
                    --j;
                    list[i] = (list[i].From, list[j].To);
                    list.RemoveRange(i + 1, j - i);
                }
            }

            return this;
        }

        object ICloneable.Clone() => Clone();

        /// <summary>
        /// Get the current intervals.
        /// Modifying the collection is highly unrecommended as it may break consistency and yield unpredicted outcomes.
        /// </summary>
        public ICollection<Interval1d> Intervals => _orderedList;
        /// <summary>
        /// Determines if the set is valid and compactable by <see cref="Compact"/>.
        /// </summary>
        /// <param name="compactable">If the set is compactable</param>
        /// <returns></returns>
        public bool CheckValidity(out bool compactable)
        {
            var compactableout = false;
            compactable = false;
            var list = _orderedList.UnderlyingList;

            for (var i = 0; i < list.Count; i++)
            {
                var iv = list[i];
                if (iv.Length < Tolerance || !iv.IsValid)
                    return false;

                if (i != list.Count - 1)
                {
                    var iv2 = list[i + 1];
                    var compareResult = _dblComparer.Compare(iv.To, iv2.From);
                    if (compareResult > 0)
                        return false;

                    if (compareResult == 0)
                        compactableout = true;
                }
            }

            compactable = compactableout;
            return true;
        }

        /// <summary>
        /// Get the total length of intervals inside the set.
        /// </summary>
        /// <returns><see cref="double.PositiveInfinity"/> if the set includes the infinity point.</returns>
        public double GetTotalLength()
        {
            var sum = 0.0;
            foreach (var it in _orderedList.UnderlyingList)
            {
                if (!it.From.IsFinite() || !it.To.IsFinite())
                    return double.PositiveInfinity;

                sum += it.Length;
            }

            return sum;
        }

        void IInterval1dSet.UnionWith(Interval1d interval)
            => UnionWith(interval);

        void IInterval1dSet.UnionWith(IInterval1dSet set)
            => UnionWith(set);

        void IInterval1dSet.SubtractBy(Interval1d interval)
            => SubtractBy(interval);

        void IInterval1dSet.SubtractBy(IInterval1dSet set)
            => SubtractBy(set);

        void IInterval1dSet.IntersectWith(Interval1d interval)
            => IntersectWith(interval);

        void IInterval1dSet.IntersectWith(IInterval1dSet set)
            => IntersectWith(set);

        void IInterval1dSet.Compact() => Compact();
    }
}

