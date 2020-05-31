using System;
using System.Runtime.CompilerServices;

namespace Svelto.DataStructures
{
    public interface IBufferBase<T>
    {
    }
    
    public interface IBuffer<T>:IBufferBase<T>
    {
        //ToDo to remove
        ref T this[uint index] { get; }
        //ToDo to remove
        ref T this[int index] { get; }
        
<<<<<<< HEAD
        void CopyTo(uint sourceStartIndex, T[] destination, uint destinationStartIndex, uint size);
        void Clear();
        
        T[]    ToManagedArray();
        IntPtr ToNativeArray(out int capacity);

        int capacity { get; }
    }
    
    public static class IBufferExtensionN
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static NB<T> ToFast<T>(this IBuffer<T> buffer) where T:unmanaged
        {
            DBC.Common.Check.Assert(buffer is NB<T>, "impossible conversion");
            return (NB<T>) buffer;
        }
    }
    
    public static class IBufferExtensionM
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static MB<T> ToFast<T>(this IBufferBase<T> buffer)
        {
            DBC.Common.Check.Assert(buffer is MB<T>, "impossible conversion");
            return (MB<T>) buffer;
        }
=======
        void CopyFrom<TBuffer>(TBuffer array,  uint startIndex,       uint size) where TBuffer : IBuffer<T>;
        void CopyFrom(T[]              source, uint sourceStartIndex, uint destinationStartIndex, uint size);
        void CopyFrom(ICollection<T>   source);
        void CopyTo(T[]                destination, uint sourceStartIndex, uint destinationStartIndex, uint size);
        void Clear(uint startIndex, uint count);
        void Clear();
        void UnorderedRemoveAt(int index);
        T[]  ToManagedArray();
        IntPtr ToNativeArray();
        GCHandle Pin();

        uint capacity { get; }
        uint count { get; }
>>>>>>> 800c1a9abe35986fabb6562178e27d3b17c34b5c
    }
}