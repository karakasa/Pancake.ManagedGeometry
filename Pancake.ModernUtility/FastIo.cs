using System;
using System.IO;

namespace Pancake.ModernUtility
{
    public unsafe static class FastIo
    {
        public static void Write<T>(this Stream stream, T[] array)
            where T : unmanaged
        {
            if (array.Length == 0) return;

            fixed (T* ptr = &array[0])
            {
                var span = new ReadOnlySpan<byte>(ptr, sizeof(T) * array.Length);
                stream.Write(span);
            }
        }
    }
}
