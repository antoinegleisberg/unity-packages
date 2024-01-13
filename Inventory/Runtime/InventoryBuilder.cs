using System;


namespace antoinegleisberg.Inventory
{
    public static class InventoryBuilder
    {
        /// <summary>
        /// Creates an inventory with a limited size. The number of slots is unlimited, but the total number of items is limited.
        /// </summary>
        /// <typeparam name="T">The type of item to be stored</typeparam>
        /// <param name="maxCapacity">The capacity of the inventory</param>
        /// <param name="itemSizes">A function associating for every item the size of the item</param>
        /// <returns>The created inventory</returns>
        public static IInventory<T> CreateLimitedCapacityInventory<T>(int maxCapacity, Func<T, int> itemSizes) => new UnlimitedSlotsInventory<T>(maxCapacity, itemSizes);

        /// <summary>
        /// Creates an inventory with a limited number of slots. The number of items for each slot is unlimited
        /// </summary>
        /// <typeparam name="T">The type of item to be stored</typeparam>
        /// <param name="maxSlots">The number of available slots in the inventory</param>
        /// <returns>The created inventory</returns>
        public static IInventory<T> CreateLimitedSlotsInventory<T>(int maxSlots) => new LimitedSlotsInventory<T>(maxSlots);

        /// <summary>
        /// Creates an inventory of infinite size.
        /// </summary>
        /// <typeparam name="T">The type of item to be stored</typeparam>
        /// <returns>The created inventory</returns>
        public static IInventory<T> CreateUnlimitedSizeInventory<T>() => new UnlimitedSizeInventory<T>();

        /// <summary>
        /// Creates an inventory with a limited number of slots. The number of items for each slot is also limited.
        /// </summary>
        /// <typeparam name="T">The type of item to be stored</typeparam>
        /// <param name="maxSlots">The number of available slots in the inventory</param>
        /// <param name="maxStackSize">A function associating for every item the stack size of the item</param>
        /// <returns>The created inventory</returns>
        public static IInventory<T> CreateLimitedItemsAndSlotsInventory<T>(int maxSlots, Func<T, int> maxStackSize) => new LimitedSlotSizeInventory<T>(maxSlots, maxStackSize);
    }
}
