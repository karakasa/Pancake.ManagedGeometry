using System;
using System.Collections.Generic;
using System.Text;

namespace Pancake.ManagedGeometry.Utility
{
    internal static class LanguageExtensions
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }
    }
}
