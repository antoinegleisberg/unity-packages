using System;


namespace antoinegleisberg.InventorySystem
{
    public static class InventoryBuilder
    {
        public static IInventory<T> CreateUnlimitedSlotsInventory<T>(int maxItems) => new UnlimitedSlotsInventory<T>(maxItems);

        public static IInventory<T> CreateLimitedSlotsInventory<T>(int maxSlots) => new LimitedSlotsInventory<T>(maxSlots);

        public static IInventory<T> CreateUnlimitedSizeInventory<T>() => new UnlimitedSizeInventory<T>();

        public static IInventory<T> CreateLimitedSlotSizeInventory<T>(int maxSlots, Func<T, int> maxStackSize) => new LimitedSlotSizeInventory<T>(maxSlots, maxStackSize);
    }
}
