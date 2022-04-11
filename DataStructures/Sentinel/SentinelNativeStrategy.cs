﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Svelto.DataStructures;

namespace Svelto.Common.DataStructures
{
    /// <summary>
    /// internal structure just for the sake of creating the sentinel dictionary
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct SentinelNativeStrategy<T> : IBufferStrategy<T> where T : struct
    {
#if DEBUG && !PROFILE_SVELTO
        static SentinelNativeStrategy()
        {
            if (TypeType.isUnmanaged<T>() == false)
                throw new DBC.Common.PreconditionException("Only unmanaged data can be stored natively");
        }
#endif
        public SentinelNativeStrategy(uint size, Allocator allocator, bool clear = true) : this()
        {
            Alloc(size, allocator, clear);
        }

        public int       capacity           => _realBuffer.capacity;
        public Allocator allocationStrategy => _nativeAllocator;

        public void Alloc(uint newCapacity, Allocator allocator, bool clear)
        {
#if DEBUG && !PROFILE_SVELTO
            if (!(this._realBuffer.ToNativeArray(out _) == IntPtr.Zero))
                throw new DBC.Common.PreconditionException("can't alloc an already allocated buffer");
#endif
            _nativeAllocator = allocator;

            IntPtr        realBuffer = MemoryUtilities.Alloc<T>(newCapacity, _nativeAllocator, clear);
            SentinelNB<T> b          = new SentinelNB<T>(realBuffer, newCapacity);
            _invalidHandle = true;
            _realBuffer    = b;
        }

        public void Resize(uint newSize, bool copyContent = true)
        {
            if (newSize != capacity)
            {
                IntPtr pointer = _realBuffer.ToNativeArray(out _);
                pointer = MemoryUtilities.Realloc<T>(pointer, newSize, _nativeAllocator,
                    newSize > capacity ? (uint)capacity : newSize, copyContent);
                SentinelNB<T> b = new SentinelNB<T>(pointer, newSize);
                _realBuffer    = b;
                _invalidHandle = true;
            }
        }

        public IntPtr AsBytesPointer()
        {
            throw new NotImplementedException();
        }

        public void SerialiseFrom(IntPtr bytesPointer)
        {
            throw new NotImplementedException();
        }

        public void ShiftLeft(uint index, uint count)
        {
            DBC.Common.Check.Require(index < capacity, "out of bounds index");
            DBC.Common.Check.Require(count < capacity, "out of bounds count");

            if (count == index)
                return;

            DBC.Common.Check.Require(count > index, "wrong parameters used");

            var array = _realBuffer.ToNativeArray(out _);

            MemoryUtilities.MemMove<T>(array, index + 1, index, count - index);
        }

        public void ShiftRight(uint index, uint count)
        {
            DBC.Common.Check.Require(index < capacity, "out of bounds index");
            DBC.Common.Check.Require(count < capacity, "out of bounds count");

            if (count == index)
                return;

            DBC.Common.Check.Require(count > index, "wrong parameters used");

            var array = _realBuffer.ToNativeArray(out _);

            MemoryUtilities.MemMove<T>(array, index, index + 1, count - index);
        }

        public bool isValid => _realBuffer.isValid;

        public void Clear() => _realBuffer.Clear();

        public ref T this[uint index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _realBuffer[index];
        }

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _realBuffer[index];
        }

        /// <summary>
        /// Note on the code of this method. Interfaces cannot be held by this structure as it must be used by Burst.
        /// This method could return directly _realBuffer, but this would cost of a boxing allocation.
        /// Using the GCHandle.Alloc I will occur to the boxing, but only once as long as the native handle is still
        /// valid
        /// </summary>
        /// <returns></returns>
        IBuffer<T> IBufferStrategy<T>.ToBuffer()
        {
            //handle has been invalidated, dispose of the hold GCHandle (if exists)
            if (_invalidHandle == true && ((IntPtr)_cachedReference != IntPtr.Zero))
            {
                _cachedReference.Free();
                _cachedReference = default;
            }

            _invalidHandle = false;
            if (((IntPtr)_cachedReference == IntPtr.Zero))
            {
                _cachedReference = GCHandle.Alloc(_realBuffer, GCHandleType.Normal);
            }

            return (IBuffer<T>)_cachedReference.Target;
        }

        public SentinelNB<T> ToRealBuffer()
        {
            return _realBuffer;
        }

        public void Dispose()
        {
            if ((IntPtr)_cachedReference != IntPtr.Zero)
                _cachedReference.Free();

            if (_realBuffer.ToNativeArray(out _) != IntPtr.Zero)
                MemoryUtilities.Free(_realBuffer.ToNativeArray(out _), Allocator.Persistent);
            else
                throw new Exception("trying to dispose disposed buffer");

            _cachedReference = default;
            _realBuffer      = default;
        }

        Allocator     _nativeAllocator;
        SentinelNB<T> _realBuffer;
        bool          _invalidHandle;

#if UNITY_COLLECTIONS || UNITY_JOBS || UNITY_BURST
        [Unity.Collections.LowLevel.Unsafe.NativeDisableUnsafePtrRestriction]
#endif
        GCHandle _cachedReference;
    }
}