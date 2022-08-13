﻿using Pancake.ManagedGeometry.Algo.DataStructure;
using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Pancake.ManagedGeometry.Factory;
using System.Diagnostics;

namespace Pancake.ManagedGeometry.Algo
{
    public class Interval1dSet : IInterval1dSet, ICloneable
    {
        private readonly OrderedListBasicImplWithComparer
            <Interval1d, NonOverlappingInterval1dComparerStruct> _orderedList;
        private readonly double _tolerance;
        private readonly NonOverlappingInterval1dComparerStruct _comparer;
        private readonly EpsilonComparer _dblComparer;

        private readonly List<double> _tempListForSplitPoints = new();
        private readonly List<Interval1d> _tempListForSegmentToAdd = new();
        private Interval1d[] _tempArrayInterval = new Interval1d[1];
        public Interval1dSet() : this(MathUtils.ZeroTolerance)
        {

        }

        public Interval1dSet(double tolerance)
        {
            _tolerance = tolerance;
            _comparer = new(_tolerance);
            _dblComparer = new(_tolerance);
            _orderedList = new(null, _comparer);
        }
        private Interval1dSet(Interval1dSet another)
        {
            _tolerance = another._tolerance;
            _comparer = new(_tolerance);
            _dblComparer = new(_tolerance);

            _orderedList = OrderedListBasicImplWithComparer<Interval1d,
                NonOverlappingInterval1dComparerStruct>.CreateWithSortedData(
                another._orderedList.UnderlyingList, _comparer);
        }
        public Interval1dSet Clone()
        {
            return new Interval1dSet(this);
        }
        public void Clear()
        {
            _orderedList.Clear();
        }

        public Interval1dSet CreateComplement()
            => CreateComplement((double.NegativeInfinity, double.PositiveInfinity));
        public Interval1dSet CreateComplement(Interval1d totalRange)
        {
            var set = new Interval1dSet();
            set.UnionWith(totalRange);
            set.SubtractBy(this);
            set.Compact();

            return set;
        }
        public int Count => _orderedList.Count;
        private static void ThrowTooSmallInterval()
        {
            throw new ArgumentException("Interval smaller than tolerance.");
        }
        public void UnionWith(Interval1d interval)
        {
            if (interval.Length < _tolerance)
                ThrowTooSmallInterval();

            if (_orderedList.Count == 0)
            {
                _orderedList.Add(interval);
                return;
            }

            FindCharacteristicPoint(interval);

            var list = _orderedList.UnderlyingList;

            if (_tempListForSplitPoints.Count == 0)
            {
                if (list.All(iv => !iv.Contains(interval, _tolerance)))
                    _orderedList.Add(interval);

                return;
            }

            var splitPoints = _tempListForSplitPoints.DistinctByComparer(_dblComparer);

            _tempListForSegmentToAdd.Clear();

            _tempArrayInterval[0] = interval;
            foreach (var segment in _tempArrayInterval.SplitAtSorted(splitPoints))
            {
                if (list.All(iv => !iv.Contains(segment, _tolerance)))
                    _tempListForSegmentToAdd.Add(segment);
            }

            foreach (var it in _tempListForSegmentToAdd)
                _orderedList.Add(it);

            _tempListForSegmentToAdd.Clear();
            _tempListForSplitPoints.Clear();
        }
        public void SubtractByCopied(Interval1d interval)
        {
            if (interval.Length < _tolerance)
                ThrowTooSmallInterval();

            if (_orderedList.Count == 0)
            {
                return;
            }

            var list = _orderedList.UnderlyingList;
            var list2 = new List<Interval1d>();

            var from = interval.From;
            var to = interval.To;

            foreach (var it in list)
            {
                var containsFrom = it.Contains(from, _tolerance);
                var containsTo = it.Contains(to, _tolerance);

                if (!containsFrom && !containsTo)
                {
                    if (from < it.From && to > it.To)
                    {

                    }
                    else
                    {
                        list2.Add(it);
                    }

                    continue;
                }

                if (containsFrom && !containsTo)
                {
                    AddToList(list2, it.From, from);
                    continue;
                }

                if (!containsFrom && containsTo)
                {
                    AddToList(list2, to, it.To);
                    continue;
                }

                if (containsFrom && containsTo)
                {
                    AddToList(list2, it.From, from);
                    AddToList(list2, to, it.To);
                }
            }

            _orderedList.ReplaceUnderlyingList(list2);
        }
        private void AddToList(List<Interval1d> list, double from, double to)
        {
            if (to.CloseToExtended(from, _tolerance)) return;
            list.Add(new Interval1d(from, to));
        }
        private enum SubtractionEndOperation
        {
            Undefined,
            Remove,
            Manipulate
        }

        private const bool UseInplaceSubtraction = true;
        public void SubtractBy(Interval1d interval)
        {
            if (UseInplaceSubtraction)
            {
                SubtractByInplace(interval);
            }
            else
            {
                SubtractByCopied(interval);
            }
        }
        public void SubtractByInplace(Interval1d interval)
        {
            if (interval.Length < _tolerance)
                ThrowTooSmallInterval();

            if (_orderedList.Count == 0)
            {
                return;
            }

            var list = _orderedList.UnderlyingList;
            var listCnt = list.Count;

            SubtractionEndOperation operationAtStart = SubtractionEndOperation.Undefined;
            SubtractionEndOperation operationAtEnd = SubtractionEndOperation.Undefined;

            var startIndex = -1;
            var endIndex = -1;

            for (var i = 0; i < listCnt; i++)
            {
                if (list[i].From.CloseToExtended(interval.From, _tolerance)
                    || list[i].From > interval.From)
                {
                    startIndex = i;
                    operationAtStart = SubtractionEndOperation.Remove;
                    break;
                }

                if (list[i].ContainsOpen(interval.From, _tolerance))
                {
                    startIndex = i;
                    operationAtStart = SubtractionEndOperation.Manipulate;
                    break;
                }
            }

            if (startIndex == -1)
            {
                // The interval is larger than the largest in the current set
                return;
            }

            for (var i = startIndex; i < listCnt; i++)
            {
                if (list[i].To.CloseToExtended(interval.To, _tolerance)
                    || (list[i].To < interval.To
                    && (i == listCnt - 1 || list[i + 1].From >= interval.To - _tolerance)))
                {
                    endIndex = i;
                    operationAtEnd = SubtractionEndOperation.Remove;
                    break;
                }

                if (list[i].ContainsOpen(interval.To, _tolerance))
                {
                    endIndex = i;
                    operationAtEnd = SubtractionEndOperation.Manipulate;
                    break;
                }
            }

            if (endIndex == -1)
            {
                // The interval is smaller than the smallest in the current set
                return;
            }

            // Debug.Assert(operationAtStart == SubtractionEndOperation.Undefined);
            // Debug.Assert(operationAtEnd == SubtractionEndOperation.Undefined);

            if (operationAtStart == SubtractionEndOperation.Remove
                    && operationAtEnd == SubtractionEndOperation.Remove)
            {
                list.RemoveRange(startIndex, endIndex - startIndex + 1);
                return;
            }

            if (operationAtStart == SubtractionEndOperation.Manipulate
                && operationAtEnd == SubtractionEndOperation.Remove)
            {
                list[startIndex] = (list[startIndex].From, interval.From);
                if (endIndex > startIndex)
                    list.RemoveRange(startIndex + 1, endIndex - startIndex);
                return;
            }

            if (operationAtStart == SubtractionEndOperation.Remove
                && operationAtEnd == SubtractionEndOperation.Manipulate)
            {
                list[endIndex] = (interval.To, list[endIndex].To);
                list.RemoveRange(startIndex, endIndex - startIndex);
                return;
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
        }
        public void IntersectWith(Interval1d interval)
        {
            if (interval.Length < _tolerance)
                ThrowTooSmallInterval();

            if (_orderedList.Count == 0)
            {
                return;
            }

            FindCharacteristicPoint(interval);
        }

        public void UnionWith(IInterval1dSet set)
        {
            foreach (var it in set.Intervals)
                UnionWith(it);
        }
        
        public void SubtractBy(IInterval1dSet set)
        {
            foreach (var it in set.Intervals)
                SubtractBy(it);
        }
        
        public void IntersectWith(IInterval1dSet set)
        {
            foreach (var it in set.Intervals)
                IntersectWith(it);
        }

        /// <summary>
        /// Find split points in <paramref name="interval"/>, according to existing intervals in the set.
        /// </summary>
        /// <param name="interval"></param>
        private void FindCharacteristicPoint(Interval1d interval)
        {
            var list = _orderedList.UnderlyingList;
            _tempListForSplitPoints.Clear();

            for (var i = 0; i < list.Count; i++)
            {
                var pt = list[i].From;

                if (interval.ContainsOpen(pt, _tolerance))
                    _tempListForSplitPoints.Add(pt);

                pt = list[i].To;

                if (interval.ContainsOpen(pt, _tolerance))
                    _tempListForSplitPoints.Add(pt);
            }
        }

        /// <summary>
        /// Compact continous segments into one.
        /// This may have no effect depending on implementation.
        /// </summary>
        public void Compact()
        {
            if (_orderedList.Count <= 1) return;

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
        }

        object ICloneable.Clone()
            => Clone();

        public ICollection<Interval1d> Intervals => _orderedList;
    }
}

