using System;
using System.Collections;
using System.Collections.Generic;

namespace Svelto.DataStructures
{
    public struct ReadOnlyCollectionStruct<T> : ICollection<T>
    {
        public static ReadOnlyCollectionStruct<T> DefaultList = new ReadOnlyCollectionStruct<T>(new T[0], 0);

        public ReadOnlyCollectionStruct(T[] values, int count)
        {
            _values = values;
            _count = count;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _count;  }
        }

        public bool IsReadOnly
        {
            get { return true;  }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }
        public object SyncRoot
        {
            get { return null; }
        }
        
        T[] _values;
        int _count;


        public T this[int i]
        {
            get { return _values[i]; }
        }
    }
}