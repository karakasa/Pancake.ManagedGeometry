using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Factory
{
    public interface IInterval1dSet
    {
        public void Clear();
        public int Count { get; }
        public void UnionWith(Interval1d interval);

        public void UnionWith(IInterval1dSet set);
        public void SubtractBy(Interval1d interval);
        public void SubtractBy(IInterval1dSet set);
        public void IntersectWith(Interval1d interval);
        public void IntersectWith(IInterval1dSet set);

        /// <summary>
        /// Compact continous segments into one.
        /// This may have no effect depending on implementation.
        /// </summary>
        public void Compact();
        public IEnumerable<Interval1d> Intervals { get; }
    }
}
