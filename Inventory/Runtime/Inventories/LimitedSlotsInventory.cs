using System;
using System.Collections.Generic;


namespace antoinegleisberg.InventorySystem
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

        public void AddItems(T item, int count)
        {
            if (_items.ContainsKey(item))
            {
                _items[item] += count;
            }
            else if (_items.Count < _maxSlots)
            {
                _items.Add(item, count);
            }
            else
            {
                throw new Exception("Inventory is full");
            }
        }

        public bool Contains(T item)
        {
            return _items.ContainsKey(item);
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

        public List<T> GetItemsList()
        {
            return new List<T>(_items.Keys);
        }

        public bool IsEmpty()
        {
            return _items.Count == 0;
        }

        public void RemoveItems(T item, int count)
        {
            if (count <= 0)
            {
                return;
            }

            if (!_items.ContainsKey(item) || _items[item] < count)
            {
                throw new Exception("Not enough items");
            }
            
            _items[item] -= count;
            if (_items[item] == 0)
            {
                _items.Remove(item);
            }
        }
    }
}
