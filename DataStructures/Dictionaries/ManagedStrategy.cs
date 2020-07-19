using System;
using System.Runtime.CompilerServices;
<<<<<<< HEAD
using Svelto.Common;
=======
>>>>>>> dfdce3b4c46481199a04d9cfea6488a1a66a91cb

namespace Svelto.DataStructures
{
    public struct ManagedStrategy<T> : IBufferStrategy<T>
    {
<<<<<<< HEAD
        IBuffer<T> _buffer;
        MB<T> _realBuffer;

        public ManagedStrategy(uint size):this()
        {
            Alloc(size, Allocator.None);
        }

        public void Alloc(uint size, Allocator nativeAllocator)
        {
            MB<T> b = new MB<T>();
            b.Set(new T[size]);
            this._realBuffer = b;
            _buffer = null;
        }

=======
        IBuffer<T> buffer;
        MB<T> realBuffer;

        public ManagedStrategy(uint size):this()
        {
            Alloc(size);
        }

        public void Alloc(uint size)
        {
            MB<T> b = new MB<T>();
            b.Set(new T[size]);
            buffer = b;
            this.realBuffer = b;
        }

        public int capacity => buffer.capacity;

>>>>>>> dfdce3b4c46481199a04d9cfea6488a1a66a91cb
        public void Resize(uint newCapacity)
        {
            DBC.Common.Check.Require(newCapacity > 0, "Resize requires a size greater than 0");
            
<<<<<<< HEAD
            var realBuffer = _realBuffer.ToManagedArray();
            Array.Resize(ref realBuffer, (int) newCapacity);
            MB<T> b = new MB<T>();
            b.Set(realBuffer);
            this._realBuffer = b;
            _buffer = null;
        }

        public int capacity => _realBuffer.capacity;

        public void Clear() => _realBuffer.Clear();
        public void FastClear() => _realBuffer.FastClear();
=======
            var realBuffer = buffer.ToManagedArray();
            Array.Resize(ref realBuffer, (int) newCapacity);
            MB<T> b = new MB<T>();
            b.Set(realBuffer);
            buffer = b;
            this.realBuffer = b;
        }

        public void Clear() => realBuffer.Clear();
        public void FastClear() => realBuffer.FastClear();
>>>>>>> dfdce3b4c46481199a04d9cfea6488a1a66a91cb

        public ref T this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
<<<<<<< HEAD
            get => ref _realBuffer[index];
=======
            get => ref realBuffer[index];
>>>>>>> dfdce3b4c46481199a04d9cfea6488a1a66a91cb
        }

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
<<<<<<< HEAD
            get => ref _realBuffer[index];
        }

        public IBuffer<T> ToBuffer()
        {
            if (_buffer == null)
                _buffer = _realBuffer;
            
            return _buffer;
        }
=======
            get => ref realBuffer[index];
        }

        public IntPtr ToNativeArray() => throw new NotImplementedException();
        public IBuffer<T> ToBuffer() => buffer;
>>>>>>> dfdce3b4c46481199a04d9cfea6488a1a66a91cb

        public void Dispose() {  }
    }
}