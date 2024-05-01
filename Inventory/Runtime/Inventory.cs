using antoinegleisberg.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace antoinegleisberg.Inventory
{
    public partial class Inventory<T> : IInventory<T>
    {
        private int _maxCapacity;
        private int _maxSlots;
        private Func<T, int> _itemSizes;
        private Func<T, int> _slotSizes;
        private Dictionary<T, int> _itemCapacities;

        private Dictionary<T, int> _items;

        private Inventory() { }

        private Inventory(int maxCapacity, int maxSlots, Func<T, int> itemSizes, Func<T, int> slotSizes, Dictionary<T, int> itemCapacities, Dictionary<T, int> items) 
        {
            _maxCapacity = maxCapacity;
            _maxSlots = maxSlots;
            _itemSizes = itemSizes;
            _slotSizes = slotSizes;
            _itemCapacities = itemCapacities;

            _items = new Dictionary<T, int>();

            if (items != null)
            {
                AddItems(items);
            }
        }

        public void AddItems(Dictionary<T, int> items)
        {
            if (!CanAddItems(items))
            {
                throw new Exception("Cannot add requested items");
            }

            foreach (T item in items.Keys)
            {
                _items[item] = _items.GetValueOrDefault(item, 0) + items[item];
            }
        }

        public bool CanAddItems(Dictionary<T, int> items)
        {
            if (_maxCapacity >= 0)
            {
                int addedCapacity = CalculateCapacity(items);
                int occupiedCapacity = CalculateCapacity(_items);

                if (occupiedCapacity + addedCapacity > _maxCapacity)
                {
                    return false;
                }
            }

            if (_maxSlots >= 0)
            {
                if (CalculateNumberOfSlots(items.Merge(_items)) > _maxSlots)
                {
                    return false;
                }
            }

            if (_itemCapacities != null)
            {
                foreach (T item in items.Keys)
                {
                    // Check if that item is allowed in this inventory
                    if (!_itemCapacities.Keys.Contains(item))
                    {
                        return false;
                    }

                    // Check if there is enough capacity left for that item
                    if (_items.GetValueOrDefault(item, 0) + items[item] > _itemCapacities[item])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public int GetItemCount(T item)
        {
            return _items.GetValueOrDefault(item, 0);
        }

        public bool IsEmpty()
        {
            return _items.Count == 0;
        }

        public IReadOnlyDictionary<T, int> Items()
        {
            return _items;
        }

        public bool ContainsItems(Dictionary<T, int> items)
        {
            foreach (KeyValuePair<T, int> item in items)
            {
                if (!_items.ContainsKey(item.Key) || GetItemCount(item.Key) < item.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public void RemoveItems(Dictionary<T, int> items)
        {
            if (!ContainsItems(items))
            {
                throw new Exception("These items are not available for removal");
            }

            List<T> itemsToRemove = new List<T>();
            foreach (T item in items.Keys)
            {
                _items[item] -= items[item];
                if (_items[item] == 0)
                {
                    itemsToRemove.Add(item);
                }
            }
            foreach (T item in itemsToRemove)
            {
                _items.Remove(item);
            }
        }

        private int CalculateCapacity(Dictionary<T, int> items)
        {
            int capacity = 0;
            foreach (T item in items.Keys)
            {
                capacity += items[item] * _itemSizes(item);
            }
            return capacity;
        }

        private int CalculateNumberOfSlots(Dictionary<T, int> items)
        {
            if (_slotSizes == null)
            {
                return items.Count();
            }

            int nSlots = 0;
            foreach (T item in items.Keys)
            {
                nSlots += (int)Mathf.Ceil((float)items[item] / _slotSizes(item));
            }
            return nSlots;
        }
    }
}
