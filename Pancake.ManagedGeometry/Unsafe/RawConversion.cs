using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pancake.ManagedGeometry.Unsafe
{
    public unsafe static class RawConversion
    {
        /// <summary>
        /// Effectively converting one struct type to another of the same size.
        /// You should never write into the converted array.
        /// Do not use this function unless you really know what you are doing.
        /// It may CRASH your program.
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTo"></typeparam>
        /// <param name=""></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TTo[] ReinterpretArray<TFrom, TTo>(TFrom[] fromArray)
            where TFrom : unmanaged
            where TTo : unmanaged
        {
            // https://stackoverflow.com/a/42078952

            if (sizeof(TFrom) != sizeof(TTo)) ThrowInvalidOperationInvalidType();

            var newVal = default(TTo[]);

            var oldValRef = __makeref(fromArray);
            var newValRef = __makeref(newVal);

            *(IntPtr*)&newValRef = *(IntPtr*)&oldValRef;

            return __refvalue(newValRef, TTo[]);
        }

        private static void ThrowInvalidOperationInvalidType() 
            => throw new InvalidOperationException("Two types have different sizes.");
    }
}
