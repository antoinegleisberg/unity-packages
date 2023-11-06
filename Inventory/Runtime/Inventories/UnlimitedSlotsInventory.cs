using System;
using System.Collections.Generic;


namespace antoinegleisberg.InventorySystem
{
    internal class UnlimitedSlotsInventory<T> : IInventory<T>
    {
        private Dictionary<T, int> _items;
        private int _maxItems;
        private int _itemCount;

        public UnlimitedSlotsInventory(int maxItems)
        {
            _items = new Dictionary<T, int>();
            _maxItems = maxItems;
            _itemCount = 0;
        }

        public int RemainingCapacity => _maxItems - _itemCount;

        public bool Contains(T item) => GetItemCount(item) > 0;

        public bool IsEmpty() => _itemCount == 0;

        public void AddItems(T item, int amount)
        {
            if (amount > RemainingCapacity)
            {
                throw new Exception("Not enough space in inventory");
            }

            _itemCount += amount;
            if (Contains(item))
            {
                _items[item] += amount;
            }
            else
            {
                _items.Add(item, amount);
            }
        }

        public void RemoveItems(T item, int amount)
        {
            if (GetItemCount(item) < amount)
            {
                throw new Exception("Cannot remove more items from inventory than the amount it contains");
            }

            _items[item] -= amount;
            _itemCount -= amount;

            if (_items[item] == 0)
            {
                _items.Remove(item);
            }
        }

        public int GetItemCount(T item)
        {
            if (_items.ContainsKey(item))
            {
                return _items[item];
            }
            return 0;
        }

        public List<T> GetItemsList()
        {
            List<T> listItems = new List<T>();
            foreach (T item in _items.Keys)
            {
                listItems.Add(item);
            }
            return listItems;
        }
    }
}
