using System;
using System.Collections.Generic;


namespace antoinegleisberg.Inventory
{
    internal class LimitedSlotsInventory<T> : IInventory<T>
    {
        private Dictionary<T, int> _items;
        private int _maxSlots;

        public LimitedSlotsInventory(int maxSlots)
        {
            _maxSlots = maxSlots;
            _items = new Dictionary<T, int>();
        }
        
        public bool CanAddItems(Dictionary<T, int> items)
        {
            int occupiedSlots = _items.Count;
            foreach (KeyValuePair<T, int> item in items)
            {
                if (!_items.ContainsKey(item.Key))
                {
                    occupiedSlots++;
                }
            }
            return occupiedSlots <= _maxSlots;
        }

        public void AddItems(Dictionary<T, int> items)
        {
            if (!CanAddItems(items))
            {
                throw new Exception("Inventory is full");
            }

            foreach (KeyValuePair<T, int> item in items)
            {
                if (_items.ContainsKey(item.Key))
                {
                    _items[item.Key] += item.Value;
                }
                else
                {
                    _items.Add(item.Key, item.Value);
                }
            }
        }

        public void RemoveItems(Dictionary<T, int> items)
        {
            foreach (KeyValuePair<T, int> item in items)
            {
                if (item.Value <= 0)
                {
                    continue;
                }

                if (!_items.ContainsKey(item.Key) || _items[item.Key] < item.Value)
                {
                    throw new Exception("Not enough items");
                }

                _items[item.Key] -= item.Value;
                if (_items[item.Key] == 0)
                {
                    _items.Remove(item.Key);
                }
            }
        }

        public int GetItemCount(T item)
        {
            if (_items.ContainsKey(item))
            {
                return _items[item];
            }
            else
            {
                return 0;
            }
        }

        public bool IsEmpty()
        {
            return _items.Count == 0;
        }

        public Dictionary<T, int> Items()
        {
            return new Dictionary<T, int>(_items);
        }
    }
}
