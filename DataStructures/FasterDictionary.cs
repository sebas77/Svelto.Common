using System;
using System.Collections;
using System.Collections.Generic;

namespace Svelto.DataStructures.Experimental
{
    public class FasterDictionary<TKey, TValue> : IDictionary<TKey, TValue> where TKey : IComparable<TKey>
    {
        public FasterDictionary(int size)
        {
            _valuesInfo = new Node[size];
            _values     = new TValue[size];
            _buckets    = new int[HashHelpers.GetPrime(size)];
        }

        public FasterDictionary()
        {
            _valuesInfo = new Node[1];
            _values = new TValue[1];
            _buckets = new int[1];
        }

        ICollection<TKey> IDictionary<TKey,TValue>.Keys
        {
            get { throw new NotImplementedException(); }
        }
        
        public FasterDictionaryKeys Keys
        {
            get { throw new NotImplementedException(); }
        }

        ICollection<TValue> IDictionary<TKey,TValue>.Values
        {
            get { throw new NotImplementedException(); }
        }

        public ReadOnlyCollectionStruct<TValue> Values
        {
            get { return new ReadOnlyCollectionStruct<TValue>(_values, _freeValueCellIndex); }
        }

        public TValue[] GetValuesArray(out int count)
        {
            count = _freeValueCellIndex;

            return _values;
        }
        
        public int Count
        {
            get { return _freeValueCellIndex; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
        
        public void Add(TKey key, TValue value)
        {
            Add(key, ref value);
        }

        public void Add(TKey key, ref TValue value)
        {
            if (AddValue(key, ref value) == false)
            {
                throw new FasterDictionaryException("Key already present");
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            if (_freeValueCellIndex == 0) return;
            
            _freeValueCellIndex = 0;
            
            Array.Clear(_buckets, 0, _buckets.Length);
            Array.Clear(_values, 0, _values.Length);
            Array.Clear(_valuesInfo, 0, _valuesInfo.Length);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key)
        {
            uint findIndex;
            if (FindIndex(key, _buckets, _valuesInfo, out findIndex))
            {
                return true;
            }

            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new FasterDictionaryKeyValueEnumerator(this);
        }
        
        public FasterDictionaryKeyValueEnumerator GetEnumerator()
        {
            return new FasterDictionaryKeyValueEnumerator(this);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }
        
        protected uint GetValueIndex(TKey index)
        {
            return GetIndex(index, _buckets, _valuesInfo);
        }

        public bool TryGetValue(TKey key, out TValue result)
        {
            uint findIndex;
            if (FindIndex(key, _buckets, _valuesInfo, out findIndex))
            {
                result = _values[findIndex];
                return true;
            }

            result = default(TValue);
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void AddCapacity(int size)
        {
            throw new NotImplementedException();
        }

        public TValue this[TKey key]
        {
            get
            {
                return _values[GetIndex(key, _buckets, _valuesInfo)];
            }

            set { AddValue(key, ref value); }
        }

        bool AddValue(TKey key, ref TValue value)
        {
//get the hash and bucket index
            int hash        = key.GetHashCode() & int.MaxValue;
            int bucketIndex = hash % _buckets.Length;

            //buckets value -1 means it's empty
            var valueIndex = GetBucketIndex(_buckets[bucketIndex]);
            if (valueIndex == -1)
                //create the infonode at the last position and fill it with the relevant information
                _valuesInfo[_freeValueCellIndex] = new Node(ref key, hash);
            else
            {
                int currentValueIndex = valueIndex;
                do
                {
                    //must check if the key already exists in the dictionary
                    //for some reason this is way faster they use Comparer<TKey>.default, should investigate
                    if (_valuesInfo[currentValueIndex].hashcode == hash 
                        && _valuesInfo[currentValueIndex].key.CompareTo(key) == 0)
                            return false;

                    currentValueIndex = _valuesInfo[currentValueIndex].previous;
                }
                while (currentValueIndex != -1);

                //oops collision!
                _collisions++;
                //create a new one that points to the existing one
                //new one prev = the first in the bucket
                _valuesInfo[_freeValueCellIndex] = 
                    new Node(ref key, hash, valueIndex);
                //the first in the bucket next = new one
                _valuesInfo[valueIndex].next = 
                    (int) _freeValueCellIndex;
            }

            //item with this bucketIndex will point to the last value created
            _buckets[bucketIndex] = _freeValueCellIndex + 1;

            _values[_freeValueCellIndex] = value;

            if (++_freeValueCellIndex == _values.Length)
            {
                Array.Resize(ref _values, 
                    HashHelpers.ExpandPrime((int) _freeValueCellIndex));
                Array.Resize(ref _valuesInfo, 
                    HashHelpers.ExpandPrime((int) _freeValueCellIndex));
            }

            //too many collisions?
            if (_collisions > _buckets.Length)
            {
                //we need more space and less collisions
                _buckets = new int[HashHelpers.ExpandPrime(_collisions)];

                _collisions = 0;

                //we need to scan all the values inserted so far
                //to recompute the collision indices
                for (int i = 0; i < _freeValueCellIndex; i++)
                {
                    //get the original hash code and find the new bucketIndex
                    bucketIndex = (_valuesInfo[i].hashcode) % _buckets.Length;
                    //bucketsIndex can be -1 or a next value. If it's -1
                    //means no collisions. If there is collision, it will
                    //link to the next value index and the bucket will
                    //be updated with the current one. In this way we can
                    //rebuild the linkedlist.
                    valueIndex = GetBucketIndex(_buckets[bucketIndex]);
                    if (valueIndex != -1)
                    {
                        _collisions++;
                        _valuesInfo[i].previous      = valueIndex;
                        _valuesInfo[valueIndex].next = i;
                    }
                    else
                    {
                        _valuesInfo[i].next     = -1;
                        _valuesInfo[i].previous = -1;
                    }

                    //buckets at bucketIndex will remember the value/valueInfo 
                    //index for that bucketIndex. 
                    _buckets[bucketIndex] = i + 1;
                }
            }

            return true;
        }

        public bool Remove(TKey key)
        {
            int hash = (key.GetHashCode() & int.MaxValue);
            int bucketIndex = hash % _buckets.Length;

            //first update the buckets
            int valueIndex = GetBucketIndex(_buckets[bucketIndex]);

            if (valueIndex >= _freeValueCellIndex) return false;

            while (valueIndex != -1)
            {
                if (_valuesInfo[valueIndex].hashcode == hash && _valuesInfo[valueIndex].key.CompareTo(key) == 0)
                {
                    //se il bucket index punta direttamente alla cela da rimuovere
                    if (GetBucketIndex(_buckets[bucketIndex]) == valueIndex) //facciamo puntare alla cella precedente, potrebbe essere -1! significa che il backet è vuoto
                        _buckets[bucketIndex] = _valuesInfo[valueIndex].previous + 1;
                    else
                    {
                        if (_valuesInfo[valueIndex].previous == -1 && _valuesInfo[valueIndex].next == -1)
                            _buckets[bucketIndex] = -1 + 1;
                        else
                        {
                            UpdateLinkedList(valueIndex, _valuesInfo);
                        }
                    }

                    break;
                }

                valueIndex = (_valuesInfo[valueIndex].previous);
            }

            if (valueIndex == -1) return false;

            _freeValueCellIndex--;
            //value index è la cella dalla value list da rimuovere, se è l'ultima della lista non c 'èd bisogno di f are altro
            if (_freeValueCellIndex != valueIndex)
            {
                //non dobbiamo lasciare comunque buchi, quindi dobbiamo spostare l'\ultima valueinfo
                //questo significa cambiare la sua posizione e il puntatore nel bucket
                //prendiamo il bucket index dell'ultima cella che sostituiremo con quella da rimuovere
                var movingBucketIndex = _valuesInfo[_freeValueCellIndex].hashcode % _buckets.Length;

                //se il bucket da muovere punta direttamente alla cella da muovere
                if (GetBucketIndex(_buckets[movingBucketIndex]) == _freeValueCellIndex)
                { //lo facciamo puntare alla cella da riempire
                    _buckets[movingBucketIndex] = valueIndex + 1;
                }
                //altrimenti lasciamo le cose come stanno e aggiustiamo i puntatori
                int next = _valuesInfo[_freeValueCellIndex].next;
                int previous = _valuesInfo[_freeValueCellIndex].previous;

                if (next != -1)
                    _valuesInfo[next].previous = valueIndex;
                if (previous != -1)
                    _valuesInfo[previous].next = valueIndex;

                _valuesInfo[valueIndex] = _valuesInfo[_freeValueCellIndex];
                _values[valueIndex] = _values[_freeValueCellIndex];
            }

            return true;
        }
        
        public void Trim()
        {
            if (HashHelpers.ExpandPrime(_freeValueCellIndex) < _valuesInfo.Length)
            {
                Array.Resize(ref _values, 
                             HashHelpers.ExpandPrime(_freeValueCellIndex));
                Array.Resize(ref _valuesInfo, 
                             HashHelpers.ExpandPrime(_freeValueCellIndex));
            }
        }

        static int GetBucketIndex(int i)
        {
            return i - 1;
        }

        protected bool FindIndex(TKey key, out uint findIndex)
        {
            int hash        = (key.GetHashCode() & int.MaxValue);
            int bucketIndex = hash % _buckets.Length;

            int valueIndex = GetBucketIndex(_buckets[bucketIndex]);

            while (valueIndex != -1)
            {
                //for some reason this is way faster than using
                //Comparer<TKey>.default, should investigate
                if (_valuesInfo[valueIndex].hashcode == hash && 
                    _valuesInfo[valueIndex].key.CompareTo(key) == 0)
                {
                    findIndex = (uint) valueIndex;
                    return true;
                }

                valueIndex = _valuesInfo[valueIndex].previous;
            }

            findIndex = 0;
            return false;
        }

        static uint GetIndex(TKey key, int[] buckets, Node[] valuesInfo)
        {
            uint findIndex;
            if (FindIndex(key, buckets, valuesInfo, out findIndex)) return findIndex;

            throw new Exception();
        }

        static bool FindIndex(TKey key, int[] buckets, Node[] valuesInfo, out uint findIndex)
        {
            int hash        = (key.GetHashCode() & int.MaxValue);
            int bucketIndex = hash % buckets.Length;

            int valueIndex = GetBucketIndex(buckets[bucketIndex]);

            while (valueIndex != -1)
            {
                //for some reason this is way faster they use Comparer<TKey>.default, should investigate
                if (valuesInfo[valueIndex].hashcode == hash && valuesInfo[valueIndex].key.CompareTo(key) == 0)
                {
                    findIndex = (uint) valueIndex;
                    return true;
                }

                valueIndex = valuesInfo[valueIndex].previous;
            }
            findIndex = 0;
            return false;
        }

        static void UpdateLinkedList(int index, Node[] valuesInfo)
        {
            int next = valuesInfo[index].next;
            int previous = valuesInfo[index].previous;

            if (next != -1)
                valuesInfo[next].previous = previous;
            if (previous != -1)
                valuesInfo[previous].next = next;
        }

        public struct FasterDictionaryKeyValueEnumerator : IEnumerator<KeyValuePair<TKey, TValue>>
        {
            public FasterDictionaryKeyValueEnumerator(FasterDictionary<TKey,TValue> dic):this()
            {
                _dic = dic;
                _index = -1;
                _count = dic.Count;
            }

            public void Dispose()
            {}

            public bool MoveNext()
            {
                if (_count != _dic.Count)
                    throw new FasterDictionaryException("can't modify a dictionary during its iteration");
                
                if (_index < _count - 1)
                {
                    _index++;
                    return true;
                }

                return false;
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public KeyValuePair<TKey, TValue> Current { get { return new KeyValuePair<TKey, TValue>(_dic._valuesInfo[_index].key, _dic._values[_index]); } }

            object IEnumerator.Current
            {
                get { throw new NotImplementedException(); }
            }
            
            readonly FasterDictionary<TKey, TValue> _dic;
            readonly int _count;
            
            int _index;
        }

        internal struct Node
        {
            public readonly TKey   key;
            public readonly int hashcode;
            public          int previous;
            public          int next;

            public Node(ref TKey key, int hash, int v)
            {
                this.key = key;
                hashcode = hash;
                previous = v;
                next     = -1;
            }

            public Node(ref TKey key, int hash)
            {
                this.key = key;
                hashcode = hash;
                previous = -1;
                next     = -1;
            }
        }
        
        public struct FasterDictionaryKeys : ICollection<TKey>
        {
            internal FasterDictionaryKeys(FasterDictionary<TKey, TValue> dic):this()
            {
                _keys = dic._valuesInfo;
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
            
            public FasterDictionaryKeyEnumerator<TKey> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public void Add(TKey item)
            {
                throw new NotImplementedException();
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public bool Contains(TKey item)
            {
                throw new NotImplementedException();
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(TKey item)
            {
                throw new NotImplementedException();
            }

            public int Count { get; }
            public bool IsReadOnly { get; }
            
            Node[] _keys;
        }
        
        public struct FasterDictionaryKeyEnumerator<TKey>:IEnumerator<TKey>
        {
            public bool MoveNext()
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }

            public TKey Current { get; }

            object IEnumerator.Current => Current;

            public void Dispose()
            {
                throw new NotImplementedException();
            }
        }

        
        protected TValue[] _values;
        
        Node[] _valuesInfo;
        int[]  _buckets;
        int    _freeValueCellIndex;
        int    _collisions;
    }

    public class FasterDictionaryException : Exception
    {
        public FasterDictionaryException(string keyAlreadyExisting):base(keyAlreadyExisting)
        {}
    }
}

