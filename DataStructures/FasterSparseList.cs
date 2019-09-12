namespace Svelto.DataStructures
{
    public struct FasterSparseList<T>
    {
        internal FasterSparseList(FasterDenseList<T> denseSet)
        {
            _denseSet = denseSet;
            _sparseSet = new FasterList<uint>();
        }

        public bool TryGetValue(uint key, out T item)
        {
            if (key < _sparseSet.Count && _sparseSet[key] != NOT_USED_SLOT && _sparseSet[key] - 1 < _denseSet.Count)
            {
                item = _denseSet[_sparseSet[key] - 1];
                return true;
            }

            item = default;
            return false;
        }
        
        public bool TryRecycleValue<U>(uint key, out T item) where U:class, T
        {
            if (TryGetValue(key, out item) == true) return true;

            if (_denseSet.ReuseOneSlot<U>(key, out var item2) == true)
            {
                item = item2;
                _sparseSet.Add(key, _denseSet.Count);

                return true;
            }

            return false;
        }

        public void Add(uint key, T  item)
        {
            _denseSet.Push((key, item));
            
            _sparseSet.Add(key, _denseSet.Count);
        }
        
        public void FastClear()
        {
            _sparseSet.FastClear();
        }

        readonly FasterDenseList<T> _denseSet;
        readonly FasterList<uint>   _sparseSet;
        const ulong NOT_USED_SLOT = 0;
    }
}