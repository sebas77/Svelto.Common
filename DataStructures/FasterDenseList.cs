using System;
using System.Collections;
using System.Collections.Generic;

namespace Svelto.DataStructures
{
    public class FasterDenseList<T>:IEnumerable<T>
    {
        public uint Count => (uint) keys.Count;

        public FasterDenseList()
        {
            keys = new FasterList<uint>();
            values = new FasterList<T>();
        }
        
        public FasterSparseList<T> SparseSet()
        {
            return new FasterSparseList<T>(this);
        }
        
        public FasterDenseListEnumerator<T> GetEnumerator()
        {
            return new FasterDenseListEnumerator<T>(this);
        }

        public void FastClear()
        {
            keys.ResetToReuse();
            values.ResetToReuse();
        }
        
        internal bool ReuseOneSlot<U>(uint index, out U item) where U:class, T
        {
            if (values.ReuseOneSlot(out item) == true)
            {
                keys.ReuseOneSlot<uint>();

                keys[keys.Count - 1] = index;

                return true;
            }

            item = default;
            return false;
        }
        
        internal void Push((uint index, T item) value)
        {
            keys.Push(value.index);
            
            values.Push(value.item);
        }
        
        internal ref T this[uint index] => ref values[index];

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        
        readonly FasterList<uint> keys;
        readonly FasterList<T>    values;

        public struct DenseIterator<T>
        {
            public DenseIterator(uint currentIndex, FasterDenseList<T> fasterSparseList)
            {
                _currentIndex = currentIndex;
                _fasterSparseList = fasterSparseList;
            }

            public ref T    Value => ref _fasterSparseList.values[_currentIndex];
            public     uint Key   => _fasterSparseList.keys[_currentIndex];
        
            readonly uint               _currentIndex;
            readonly FasterDenseList<T> _fasterSparseList;
        }
    }

    public struct FasterDenseListEnumerator<T>:IEnumerator<FasterDenseList<T>.DenseIterator<T>>
    {
        internal FasterDenseListEnumerator(FasterDenseList<T> denseList):this()
        {
            _fasterDenseList = denseList;
            _currentIndex = -1;
        }

        public bool MoveNext()
        {
            if (++_currentIndex < _fasterDenseList.Count)
                return true;

            return false;
        }

        public void Reset()
        {
            _currentIndex = -1;
        }

        public FasterDenseList<T>.DenseIterator<T> Current =>
            new FasterDenseList<T>.DenseIterator<T>((uint) _currentIndex, _fasterDenseList);

        object IEnumerator.Current => throw new NotImplementedException();

        public void Dispose()
        {}
        
        readonly FasterDenseList<T> _fasterDenseList;
        int                         _currentIndex;
    }
}