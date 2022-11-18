using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Utility
{
    internal struct BasedEnumerable<T, R> : IEnumerable<T>
        where R : IEnumerator<T>
    {
        private readonly R _pe;
        public BasedEnumerable(R enumerator)
        {
            _pe = enumerator;
        }
        public IEnumerator<T> GetEnumerator() => _pe;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    internal static class BasedEnumerable
    {
        public static BasedEnumerable<T, R> Create<T, R>(R enumerator)
            where R : IEnumerator<T>
        {
            return new BasedEnumerable<T, R>(enumerator);
        }
    }
}
