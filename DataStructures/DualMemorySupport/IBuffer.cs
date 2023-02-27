using System;

namespace Svelto.DataStructures
{
    public interface IBufferBase
    {
    }

    public interface INativeBuffer
    {
        public IntPtr ToNativeArray(out int capacity);
    }

    public interface IBuffer<T>:IBufferBase
    {
        void CopyTo(uint sourceStartIndex, T[] destination, uint destinationStartIndex, uint count);
        void Clear();
        
        int capacity { get; }
        bool isValid  { get; }
    }
}