using System;
using System.Runtime.CompilerServices;

namespace NVorbis
{
    // Need fake Memory<T> and Span<T>
    // Only the exact interface used by NVorbis has been implemented.
    // The performance benefits of Memory/Span in C#8 aren't duplicated,
    // only the logical functionality.
    readonly struct Memory<T>
    {
        private readonly T[] _object;
        private readonly int _index;
        private readonly int _length;

        public static Memory<T> Empty => default;

        public int Length => _length;
        public Span<T> Span => new Span<T>(_object, _index, _length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory(T[] array)
        {
            if (array == null)
            {
                this = default;
                return; // returns default
            }

            if (!typeof(T).IsValueType && array.GetType() != typeof(T[]))
            {
                throw new ArrayTypeMismatchException();
            }

            _object = array;
            _index = 0;
            _length = array.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory(T[] array, int start, int length)
        {
            if (array == null)
            {
                if (start != 0 || length != 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this = default;
                return; // returns default
            }

            if (!typeof(T).IsValueType && array.GetType() != typeof(T[]))
            {
                throw new ArrayTypeMismatchException();
            }

            if ((uint)start > (uint)array.Length || (uint)length > (uint)(array.Length - start))
            {
                throw new ArgumentOutOfRangeException();
            }

            _object = array;
            _index = start;
            _length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(Memory<T> destination)
        {
            if ((uint)_length <= (uint)destination.Length)
            {
                Array.Copy(_object, _index, destination._object, destination._index, _length);
            }
            else
            {
                throw new ArgumentException("destination is shorter than the source Span<T>.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> Slice(int start)
        {
            if ((uint)start > (uint)_length)
            {
                throw new ArgumentOutOfRangeException("start is less than zero or greater than Length.");
            }

            return new Memory<T>(_object, _index + start, _length - start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<T> Slice(int start, int length)
        {
            if ((uint)start > (uint)_length || (uint)length > (uint)(_length - start))
            {
                throw new ArgumentOutOfRangeException("start is less than zero or greater than Length or length is greater than Length - start");
            }

            return new Memory<T>(_object, _index + start, length);
        }
    }

    readonly struct Span<T>
    {
        private readonly T[] _object;
        private readonly int _index;
        private readonly int _length;

        public int Length => _length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span(T[] array, int start, int length)
        {
            if (array == null)
            {
                if (start != 0 || length != 0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                this = default;
                return; // returns default
            }

            if (!typeof(T).IsValueType && array.GetType() != typeof(T[]))
            {
                throw new ArrayTypeMismatchException();
            }

            if ((uint)start > (uint)array.Length || (uint)length > (uint)(array.Length - start))
            {
                throw new ArgumentOutOfRangeException();
            }

            _object = array;
            _index = start;
            _length = length;
        }

        public T this[int index] => _object[_index + index];
    }
}
