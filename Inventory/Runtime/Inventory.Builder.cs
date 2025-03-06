using System;
using System.Collections.Generic;

namespace antoinegleisberg.Inventory
{
    public partial class Inventory<T>
    {
        public static Builder CreateBuilder() => new Builder();

        public class Builder
        {
            private int _maxCapacity = -1;
            private Func<T, int> _itemSizes = null;
            
            private int _maxSlots = -1;
            private Func<T, int> _maxSlotSize = null;

            private IReadOnlyDictionary<T, int> _itemCapacities = null;
            
            private IReadOnlyDictionary<T, int> _items = null;

            internal Builder() { }

            /// <summary>
            /// Adds a capacity limit to the inventory
            /// </summary>
            /// <param name="capacity"></param>
            /// <param name="itemSizes"></param>
            /// <returns></returns>
            public Builder WithLimitedCapacity(int capacity, Func<T, int> itemSizes)
            {
                if (capacity < 0)
                {
                    throw new ArgumentException("Capacity must be positive or 0");
                }
                if (itemSizes == null)
                {
                    throw new ArgumentException("You must provide item sizes");
                }

                _maxCapacity = capacity;
                _itemSizes = itemSizes;
                return this;
            }

            /// <summary>
            /// Limits the number of distinct items the inventory can store
            /// </summary>
            /// <param name="maxSlots"></param>
            /// <returns></returns>
            public Builder WithLimitedSlots(int maxSlots)
            {
                if (maxSlots < 0)
                {
                    throw new ArgumentException("MaxSlots must be positive or zero");
                }

                _maxSlots = maxSlots;
                return this;
            }

            /// <summary>
            /// For an inventory with a limited amount of slots, also limits the size of a slot.
            /// Requires the maximum amount of slots to also be set.
            /// </summary>
            /// <param name="maxSlotSize"></param>
            /// <returns></returns>
            public Builder WithLimitedSlotSize(Func<T, int> maxSlotSize)
            {
                if (maxSlotSize == null)
                {
                    throw new ArgumentException("You must provide slot sizes");
                }

                _maxSlotSize = maxSlotSize;
                return this;
            }

            /// <summary>
            /// Limits the inventory to contain at most the specified capacity.
            /// </summary>
            /// <param name="itemCapacities"></param>
            /// <returns></returns>
            public Builder WithPredeterminedItemSet(IReadOnlyDictionary<T, int> itemCapacities)
            {
                if (itemCapacities == null)
                {
                    throw new ArgumentException("You must provide item capacities");
                }

                _itemCapacities = itemCapacities;
                return this;
            }

            /// <summary>
            /// Fills the inventory with an initial set of items
            /// </summary>
            /// <param name="items"></param>
            /// <returns></returns>
            public Builder WithItems(IReadOnlyDictionary<T, int> items)
            {
                if (items == null)
                {
                    throw new ArgumentException("You must provide non-null items");
                }

                _items = items;
                return this;
            }

            public Inventory<T> Build()
            {
                if (_maxSlotSize != null && _maxSlots < 0)
                {
                    // Cannot enforce a slot size with an infinite amount of slots !
                    throw new Exception("Please specify a maximum amount of slots in order to limit the slot size!");
                }
                return new Inventory<T>(_maxCapacity, _maxSlots, _itemSizes, _maxSlotSize, _itemCapacities, _items);
            }
        }
    }
}
