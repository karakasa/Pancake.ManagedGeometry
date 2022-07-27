using Pancake.ManagedGeometry.Algo.DataStructure;
using Pancake.ManagedGeometry.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Pancake.ManagedGeometry.Factory;

namespace Pancake.ManagedGeometry.Algo
{
    public class Interval1dSet : IInterval1dSet
    {
        private readonly OrderedListBasicImpl<Interval1d> _orderedList;
        private readonly double _tolerance;
        private readonly NonOverlappingInterval1dComparer _comparer;
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
        public void Clear()
        {
            _orderedList.Clear();
        }

        public int Count => _orderedList.Count;
        public void UnionWith(Interval1d interval)
        {
            if (interval.Length < _tolerance)
            {
                throw new ArgumentException("Interval smaller than tolerance.");
            }

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
        public void SubtractBy(Interval1d interval)
        {
            throw new NotImplementedException();
        }
        public void IntersectWith(Interval1d interval)
        {
            throw new NotImplementedException();
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
        public IEnumerable<Interval1d> Intervals => _orderedList;
    }
}
