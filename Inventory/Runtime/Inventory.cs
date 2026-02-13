using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using antoinegleisberg.Types;

using Logger = antoinegleisberg.Utils.Logger;


namespace antoinegleisberg.Inventory
{
    public partial class Inventory<T> : IInventory<T>
    {
        private int _maxCapacity;
        private int _maxSlots;
        private Func<T, int> _itemSizes;
        private Func<T, int> _slotSizes;
        private IReadOnlyDictionary<T, int> _itemCapacities;

        private Dictionary<T, int> _items;

        private Inventory() { }

        private Inventory(int maxCapacity, int maxSlots, Func<T, int> itemSizes, Func<T, int> slotSizes, IReadOnlyDictionary<T, int> itemCapacities, IReadOnlyDictionary<T, int> items) 
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

        public void AddItems(IReadOnlyDictionary<T, int> items)
        {
            if (!CanAddItems(items))
            {
                throw new InvalidOperationException("Cannot add requested items");
            }

            foreach (T item in items.Keys)
            {
                _items[item] = _items.GetValueOrDefault(item, 0) + items[item];
            }
        }

        public bool CanAddItems(IReadOnlyDictionary<T, int> items)
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

        public int GetAvailableItemCount(T item)
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

        public IReadOnlyDictionary<T, int> AvailableItems()
        {
            return _items;
        }

        public bool ContainsAvailableItems(IReadOnlyDictionary<T, int> items)
        {
            foreach (KeyValuePair<T, int> item in items)
            {
                if (item.Value <= 0)
                {
                    continue;
                }
                if (!_items.ContainsKey(item.Key) || GetAvailableItemCount(item.Key) < item.Value)
                {
                    return false;
                }
            }
            return true;
        }

        public void RemoveItems(IReadOnlyDictionary<T, int> items)
        {
            if (!ContainsAvailableItems(items))
            {
                throw new InvalidOperationException("These items are not available for removal");
            }

            foreach (T item in new List<T>(items.Keys))
            {
                if (items[item] <= 0)
                {
                    continue;
                }
                _items[item] -= items[item];
                if (_items[item] == 0)
                {
                    _items.Remove(item);
                }
            }
        }

        public int GetAvailableCapacityForItem(T item)
        {
            int totalAvailableCapacity = int.MaxValue;
            if (_maxCapacity >= 0)
            {
                int occupiedCapacity = CalculateCapacity(_items);
                int availableCapacity = Mathf.FloorToInt((float)(_maxCapacity - occupiedCapacity) / _itemSizes(item));
                totalAvailableCapacity = Mathf.Min(totalAvailableCapacity, availableCapacity);
            }
            if (_maxSlots >= 0)
            {
                int occupiedSlots = CalculateNumberOfSlots(_items);
                int availableSlots = _maxSlots - occupiedSlots;
                int availableCapacity;
                if (_slotSizes != null)
                {
                    availableCapacity = Mathf.FloorToInt((float)availableSlots * _slotSizes(item));  // capacity from filling up free slots
                    if (GetAvailableItemCount(item) % _slotSizes(item) != 0)
                    {
                        // if there is a partially filled slot, account for the remaining capacity from filling up that slot
                        availableCapacity += _slotSizes(item) - (GetAvailableItemCount(item) % _slotSizes(item));
                    }
                    availableSlots *= _slotSizes(item);
                }
                else if (availableSlots > 0)
                {
                    // If slots can hold any capacity and there is a free slot, then there is infinite capacity
                    availableCapacity = int.MaxValue;
                }
                else
                {
                    // All slots are taken, there is only capacity if there is already some of that item in the inventory
                    availableCapacity = GetAvailableItemCount(item) > 0 ? int.MaxValue : 0;
                }
                totalAvailableCapacity = Mathf.Min(totalAvailableCapacity, availableCapacity);
            }
            if (_itemCapacities != null)
            {
                if (!_itemCapacities.ContainsKey(item))
                {
                    return 0;
                }
                int availableCapacity = _itemCapacities[item] - GetAvailableItemCount(item);
                totalAvailableCapacity = Mathf.Min(totalAvailableCapacity, availableCapacity);
            }
            return totalAvailableCapacity;
        }

        public (int addedQuantity, int remainingQuantity) AddAsManyAsPossible(T item, int count)
        {
            int addedQuantity = Mathf.Min(GetAvailableCapacityForItem(item), count);
            AddItems(new Dictionary<T, int>() { { item, addedQuantity } });
            return (addedQuantity, count - addedQuantity);
        }

        public int OccupiedCapacity()
        {
            return CalculateCapacity(_items);
        }

        public string GetDebugInformation()
        {
            return
                $"Config: maxCapacity={_maxCapacity}; maxSlots={_maxSlots};" +
                $"itemCapacities={string.Join(", ", _itemCapacities.Select((KeyValuePair<T, int> kvp) => $"{kvp.Key}:{kvp.Value}"))}" +
                $"items={string.Join(", ", _items.Select((KeyValuePair<T, int> kvp) => $"{kvp.Key}:{kvp.Value}"))}";
        }

        private int CalculateCapacity(IReadOnlyDictionary<T, int> items)
        {
            int capacity = 0;
            foreach (T item in items.Keys)
            {
                capacity += items[item] * _itemSizes(item);
            }
            return capacity;
        }

        private int CalculateNumberOfSlots(IReadOnlyDictionary<T, int> items)
        {
            if (_slotSizes == null)
            {
                return items.Count();
            }

            int nSlots = 0;
            foreach (T item in items.Keys)
            {
                nSlots += Mathf.CeilToInt((float)items[item] / _slotSizes(item));
            }
            return nSlots;
        }
    }
}
