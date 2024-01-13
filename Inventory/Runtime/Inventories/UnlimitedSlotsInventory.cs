using System;
using System.Collections.Generic;
using UnityEngine;

namespace antoinegleisberg.Inventory
{
    internal class UnlimitedSlotsInventory<T> : IInventory<T>
    {
        private Dictionary<T, int> _items;
        private int _maxCapacity;
        private int _capacity;
        private Func<T, int> _itemSizes;

        public UnlimitedSlotsInventory(int maxCapacity, Func<T, int> itemSizes)
        {
            _items = new Dictionary<T, int>();
            _maxCapacity = maxCapacity;
            _capacity = 0;
            _itemSizes = itemSizes;
        }

        public bool IsEmpty() => _capacity == 0;

        public void RemoveItems(Dictionary<T, int> items)
        {
            foreach (KeyValuePair<T, int> item in items)
            {
                if (GetItemCount(item.Key) < item.Value)
                {
                    throw new Exception("Cannot remove more items from inventory than the count it contains");
                }

                _items[item.Key] -= item.Value;
                _capacity -= item.Value * _itemSizes(item.Key);

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
            return 0;
        }

        public bool CanAddItems(Dictionary<T, int> items)
        {
            int currentCapacity = _capacity;
            foreach (KeyValuePair<T, int> item in items)
            {
                currentCapacity += item.Value * _itemSizes(item.Key);
            }
            return currentCapacity <= _maxCapacity;
        }

        public void AddItems(Dictionary<T, int> items)
        {
            if (!CanAddItems(items))
            {
                throw new Exception("Cannot add more items to inventory than its capacity");
            }

            foreach (KeyValuePair<T, int> item in items)
            {
                _capacity += item.Value * _itemSizes(item.Key);
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

        public Dictionary<T, int> Items()
        {
            return new Dictionary<T, int>(_items);
        }
    }
}
