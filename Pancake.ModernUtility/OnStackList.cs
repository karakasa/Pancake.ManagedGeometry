using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Pancake.ModernUtility;

public ref struct OnStackList<T>(Span<T> buffer)
{
    private readonly Span<T> _buffer = buffer;
    private List<T>? _fallback;
    public int Count { readonly get; private set; }
    public void Add(T item)
    {
        ++Count;

        if (_fallback is not null)
        {
            _fallback.Add(item);
            return;
        }

        if (Count <= _buffer.Length)
        {
            _buffer[Count - 1] = item;
        }
        else
        {
            AddOnThreshold(item);
        }
    }

    public void AddRange(ReadOnlySpan<T> items)
    {
        var newCnt = Count + items.Length;

        if (_fallback is not null)
        {
            Count = newCnt;
            _fallback.AddRange(items);
            return;
        }

        if (newCnt > _buffer.Length)
        {
            AddRangeOnThreshold(items, newCnt);
            return;
        }

        items.CopyTo(_buffer[Count..]);
        Count = newCnt;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddRangeOnThreshold(ReadOnlySpan<T> items, int newCnt)
    {
        _fallback = new(newCnt);
        _fallback.AddRange(_buffer[..Count]);
        _fallback.AddRange(items);

        Count = newCnt;
    }

    public readonly Span<T> AsSpan()
    {
        if (_fallback is not null) return CollectionsMarshal.AsSpan(_fallback);
        return _buffer[..Count];
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void AddOnThreshold(T item)
    {
        _fallback = new(_buffer.Length + 1);
        _fallback.AddRange(_buffer);
        _fallback.Add(item);
    }

    public void Clear()
    {
        _fallback = null;
        Count = 0;
    }

    public readonly T this[int index]
    {
        get => _fallback is null ? _buffer[index] : _fallback[index];

        set
        {
            if (_fallback is null)
            {
                _buffer[index] = value;
            }
            else
            {
                _fallback[index] = value;
            }
        }
    }
}
