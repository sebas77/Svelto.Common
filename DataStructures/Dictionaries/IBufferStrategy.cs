using System;
<<<<<<< HEAD
using Svelto.Common;
=======
>>>>>>> dfdce3b4c46481199a04d9cfea6488a1a66a91cb

namespace Svelto.DataStructures
{
    public interface IBufferStrategy<T>: IDisposable
    {
        int capacity { get; }

<<<<<<< HEAD
        void Alloc(uint size, Allocator nativeAllocator);
=======
        void Alloc(uint size);
>>>>>>> dfdce3b4c46481199a04d9cfea6488a1a66a91cb
        void Resize(uint newCapacity);
        void Clear();
        
        ref T this[uint index] { get ; }
        ref T this[int index] { get ; }
        
<<<<<<< HEAD
        IBuffer<T> ToBuffer();
=======
        IntPtr ToNativeArray();
        IBuffer<T> ToBuffer();
        void FastClear();
>>>>>>> dfdce3b4c46481199a04d9cfea6488a1a66a91cb
    }
}