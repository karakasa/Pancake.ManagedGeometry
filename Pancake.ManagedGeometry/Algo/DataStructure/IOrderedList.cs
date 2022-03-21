using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Algo.DataStructure
{
    public interface IOrderedList<T> : ICollection<T>
    {
        public int LowerBoundIndex(T value);
    }
}
