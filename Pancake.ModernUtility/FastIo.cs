using System;
using System.IO;
using System.Runtime.CompilerServices;

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

        public static void Read<T>(this Stream stream, T[] array)
            where T : unmanaged
        {
            if (array.Length == 0) return;

            fixed (T* ptr = &array[0])
            {
                var span = new Span<byte>(ptr, sizeof(T) * array.Length);
                stream.Read(span);
            }
        }
        public static int ReinterpretCopy<TSource, TDestination>(TSource[] source, TDestination[] destination)
            where TSource : unmanaged
            where TDestination : unmanaged
        {
            if (sizeof(TSource) != sizeof(TDestination)) throw new InvalidOperationException("Sizes mismatch.");

            if (source.Length == 0) return 0;

            fixed (TSource* ptrSource = &source[0])
            fixed (TDestination* ptrDest = &destination[0])
            {
                var actualCopyLength = (source.Length < destination.Length) ? source.Length : destination.Length;
                var spanSource = new ReadOnlySpan<byte>(ptrSource, sizeof(TSource) * actualCopyLength);
                var spanTarget = new Span<byte>(ptrDest, sizeof(TDestination) * actualCopyLength);

                spanSource.CopyTo(spanTarget);

                return actualCopyLength;
            }
        }

        // TODO: Isn't this action illegal? Needs to be paired with No-GC region.
        public static unsafe ReadOnlySpan<TDestination> Reinterpret<TSource, TDestination>(TSource[] source)
            where TSource : unmanaged
            where TDestination : unmanaged
        {
            if (sizeof(TSource) != sizeof(TDestination)) throw new InvalidOperationException("Sizes mismatch.");

            if (source.Length == 0) return ReadOnlySpan<TDestination>.Empty;

            fixed (TSource* ptrSource = &source[0])
            {
                var spanSource = new ReadOnlySpan<TDestination>(ptrSource, source.Length);
                return spanSource;
            }
        }
    }
}
