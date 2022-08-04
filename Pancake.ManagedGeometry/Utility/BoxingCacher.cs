using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Utility
{
    internal static class BoxingCacher<T, TBoxed> where T : struct
    {
        private static readonly object _cache = new T();
        public static TBoxed Cache => (TBoxed)_cache;
    }
}
