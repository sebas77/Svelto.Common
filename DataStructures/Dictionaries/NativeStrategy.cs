using System;
using System.Runtime.CompilerServices;
<<<<<<< HEAD
using System.Runtime.InteropServices;
using DBC.Common;
=======
>>>>>>> dfdce3b4c46481199a04d9cfea6488a1a66a91cb
using Svelto.Common;

namespace Svelto.DataStructures
{
    public struct NativeStrategy<T> : IBufferStrategy<T> where T : struct
    {
<<<<<<< HEAD
        Allocator _nativeAllocator;
        NB<T>     _realBuffer;
#if UNITY_COLLECTIONS
        [Unity.Collections.LowLevel.Unsafe.NativeDisableUnsafePtrRestriction]
#endif
        IntPtr _buffer;

        public NativeStrategy(uint size, Allocator nativeAllocator) : this() { Alloc(size, nativeAllocator); }

        public void Alloc(uint newCapacity, Allocator nativeAllocator)
        {
            _nativeAllocator = nativeAllocator;

            Check.Require(this._realBuffer.ToNativeArray(out _) == IntPtr.Zero
                        , "can't alloc an already allocated buffer");

            var realBuffer =
                MemoryUtilities.Alloc((uint) (newCapacity * MemoryUtilities.SizeOf<T>()), _nativeAllocator);
            NB<T>      b      = new NB<T>(realBuffer, newCapacity);
            _buffer = IntPtr.Zero;
            this._realBuffer = b;
        }

        public void Resize(uint newCapacity)
        {
            Check.Require(newCapacity > 0, "Resize requires a size greater than 0");
            Check.Require(newCapacity > capacity, "can't resize to a smaller size");

            var pointer = _realBuffer.ToNativeArray(out _);
            var sizeOf  = MemoryUtilities.SizeOf<T>();
            pointer = MemoryUtilities.Realloc(pointer, (uint) (capacity * sizeOf), (uint) (newCapacity * sizeOf)
                                            , Allocator.Persistent);
            NB<T> b = new NB<T>(pointer, newCapacity);
            _buffer     = IntPtr.Zero;
            _realBuffer = b;
        }

        public void Clear()     => _realBuffer.Clear();

        public void FastClear() => _realBuffer.FastClear();
=======
        IBuffer<T> buffer;
        NB<T> realBuffer;

        public NativeStrategy(uint size):this()
        {
            Alloc(size);
        }

        public void Alloc(uint newCapacity)
        {
            DBC.Common.Check.Require(buffer == null || buffer.ToNativeArray(out _) == IntPtr.Zero, "can't alloc an already allocated buffer");

            var   realBuffer = MemoryUtilities.Alloc((uint) (newCapacity * MemoryUtilities.SizeOf<T>()), Allocator.Persistent);
            NB<T> b          = new NB<T>(realBuffer, newCapacity);
            buffer = b;
            this.realBuffer = b;
        }

        public void Clear() => buffer.Clear();
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
            if (_buffer == IntPtr.Zero)
                _buffer = GCHandle.ToIntPtr(GCHandle.Alloc(_realBuffer, GCHandleType.Normal));
            
            return (IBuffer<T>) GCHandle.FromIntPtr(_buffer).Target;
        }

        public NB<T> ToRealBuffer() { return _realBuffer; }

        public int       capacity           => _realBuffer.capacity;

        public Allocator allocationStrategy => _nativeAllocator;

        public void Dispose()
        {
            if (_realBuffer.ToNativeArray(out _) != IntPtr.Zero)
            {
                if (_buffer != IntPtr.Zero)
                    GCHandle.FromIntPtr(_buffer).Free();
                MemoryUtilities.Free(_realBuffer.ToNativeArray(out _), Allocator.Persistent);
            }
            else
                Console.LogWarning($"trying to dispose disposed buffer. Type held: {typeof(T)}");

            _realBuffer = default;
=======
            get => ref realBuffer[index];
        }

        public IntPtr ToNativeArray() { return realBuffer.ToNativeArray(out _); } //todo: this should be internal 
        public IBuffer<T> ToBuffer() { return buffer; }
        
        public int capacity => realBuffer.capacity;

        public void Resize(uint newCapacity)
        {
            DBC.Common.Check.Require(newCapacity > 0, "Resize requires a size greater than 0");
            DBC.Common.Check.Require(newCapacity > capacity, "can't resize to a smaller size");

            var pointer = buffer.ToNativeArray(out _);
            var sizeOf = MemoryUtilities.SizeOf<T>();
            pointer = MemoryUtilities.Realloc(pointer, (uint) (capacity * sizeOf), (uint) (newCapacity * sizeOf), 
                                              Allocator.Persistent);
            NB<T> b = new NB<T>(pointer, newCapacity);
            buffer = b;
            realBuffer = b;
        }

        public void Dispose()
        {
            if (buffer != null && buffer.ToNativeArray(out _) != IntPtr.Zero)
                MemoryUtilities.Free(buffer.ToNativeArray(out _), Allocator.Persistent);
            else
                if (buffer == null)
                    Svelto.Console.LogWarning($"trying to dispose a never allocated buffer. Type held: {typeof(T)}");
                else
                   if (buffer.ToNativeArray(out _) != IntPtr.Zero)
                       Svelto.Console.LogWarning($"trying to dispose disposed buffer. Type held: {typeof(T)}");
            
            realBuffer = default;
            buffer = realBuffer;
>>>>>>> dfdce3b4c46481199a04d9cfea6488a1a66a91cb
        }
    }
}