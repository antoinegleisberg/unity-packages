using System;
using System.Collections.Generic;


namespace antoinegleisberg.InventorySystem
{
    public interface IInventory<T>
    {
        public void AddItems(T item, int count);
        public void AddItem(T item) => AddItems(item, 1);
        public bool CanRemoveItems(T item, int count) => GetItemCount(item) >= count;
        public void RemoveItems(T item, int count);
        public bool Contains(T item);
        public int GetItemCount(T item);
        public bool IsEmpty();
        public List<T> GetItemsList();
    }

    public static class InventoryBuilder<T>
    {
        public static IInventory<T> CreateUnlimitedSlotsInventory(int maxItems) => new UnlimitedSlotsInventory<T>(maxItems);

        public static IInventory<T> CreateLimitedSlotsInventory(int maxSlots) => new LimitedSlotsInventory<T>(maxSlots);

        public static IInventory<T> CreateUnlimitedSizeInventory() => new UnlimitedSizeInventory<T>();

        public static IInventory<T> CreateLimitedSlotSizeInventory(int maxSlots, Func<T, int> maxStackSize) => new LimitedSlotSizeInventory<T>(maxSlots, maxStackSize);
    }
}