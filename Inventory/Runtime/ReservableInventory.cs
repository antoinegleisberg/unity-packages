using antoinegleisberg.Utils;
using System;
using System.Collections.Generic;


namespace antoinegleisberg.Inventory
{
    public partial class ReservableInventory<T> : IReservableInventory<T>
    {
        // Actual items in the inventory
        private Inventory<T> _actualInventory;
        // Inventory, minus the items that have been reserved for retrieval
        private Inventory<T> _availableInventory;
        // The inventory contains the actual items as well as the fictitiously added reserved items (capacity reservations)
        private Inventory<T> _inventoryIncludingReservedCapacity;
        // This inventory contains only the items that have been reserved for retrieval (item reservations)
        private Inventory<T> _reservedItemsForRetreival;
        // This inventory tracks the total items reserved for capacity (capacity reservations)
        private Inventory<T> _capacityReservations;

        private ReservableInventory(int maxCapacity, int maxSlots, Func<T, int> itemSizes, Func<T, int> slotSizes, IReadOnlyDictionary<T, int> itemCapacities, IReadOnlyDictionary<T, int> items)
        {
            _actualInventory = Inventory<T>.CreateBuilder().Build();
            _availableInventory = Inventory<T>.CreateBuilder().Build();
            _inventoryIncludingReservedCapacity = Inventory<T>.CreateBuilder()
                .WithLimitedCapacity(maxCapacity, itemSizes)
                .WithLimitedSlots(maxSlots)
                .WithLimitedSlotSize(slotSizes)
                .WithPredeterminedItemSet(itemCapacities)
                .WithItems(items)
                .Build();
            _reservedItemsForRetreival = Inventory<T>.CreateBuilder().Build();
            _capacityReservations = Inventory<T>.CreateBuilder().Build();
        }

        public (int addedQuantity, int remainingQuantity) AddAsManyAsPossible(T item, int count)
        {
            (int addedQuantity, int remainingQuantity) = _inventoryIncludingReservedCapacity.AddAsManyAsPossible(item, count);
            _actualInventory.AddItems(new Dictionary<T, int> { { item, addedQuantity } });
            _availableInventory.AddItems(new Dictionary<T, int> { { item, addedQuantity } });
            return (addedQuantity, remainingQuantity);
        }

        public void AddItems(IReadOnlyDictionary<T, int> items)
        {
            _inventoryIncludingReservedCapacity.AddItems(items);
            _actualInventory.AddItems(items);
            _availableInventory.AddItems(items);
        }

        public bool CanAddItems(IReadOnlyDictionary<T, int> items)
        {
            return _inventoryIncludingReservedCapacity.CanAddItems(items);
        }

        public bool ContainsAvailableItems(IReadOnlyDictionary<T, int> items)
        {
            return _availableInventory.ContainsAvailableItems(items);
        }

        public int GetAvailableCapacityForItem(T item)
        {
            return _inventoryIncludingReservedCapacity.GetAvailableCapacityForItem(item);
        }

        public int GetAvailableItemCount(T item)
        {
            return _availableInventory.GetAvailableItemCount(item);
        }

        public IReadOnlyDictionary<T, int> AvailableItems()
        {
            return _availableInventory.Items();
        }

        public bool IsEmpty()
        {
            return _actualInventory.IsEmpty();
        }

        public IReadOnlyDictionary<T, int> Items()
        {
            return _actualInventory.Items();
        }

        public void RemoveItems(IReadOnlyDictionary<T, int> items)
        {
            if (!ContainsAvailableItems(items))
            {
                throw new InvalidOperationException("Cannot remove requested items");
            }
            _inventoryIncludingReservedCapacity.RemoveItems(items);
            _actualInventory.RemoveItems(items);
            _availableInventory.RemoveItems(items);
        }

        public ReservationToken<T> ReserveCapacityForItems(IReadOnlyDictionary<T, int> items)
        {
            _inventoryIncludingReservedCapacity.AddItems(items);
            _capacityReservations.AddItems(items);
            return new ReservationToken<T>(ReservationType.Capacity, items);
        }

        public ReservationToken<T> ReserveItemsForRetrieval(IReadOnlyDictionary<T, int> items)
        {
            if (!ContainsAvailableItems(items))
            {
                Logger.Error("Cannot reserve requested items. Debug information:");
                Logger.Debug($"Actual Inventory: {_actualInventory.GetDebugInformation()}");
                Logger.Debug($"Available Inventory: {_availableInventory.GetDebugInformation()}");
                Logger.Debug($"Inventory Including Reserved Capacity: {_inventoryIncludingReservedCapacity.GetDebugInformation()}");
                Logger.Debug($"Reserved Items For Retrieval: {_reservedItemsForRetreival.GetDebugInformation()}");
                Logger.Debug($"Capacity Reservations: {_capacityReservations.GetDebugInformation()}");
                throw new InvalidOperationException("Cannot reserve requested items");
            }
            _reservedItemsForRetreival.AddItems(items);
            _availableInventory.RemoveItems(items);

            return new ReservationToken<T>(ReservationType.Items, items);
        }

        public void FulfillReservation(ReservationToken<T> token)
        {
            if (token.ReservationType == ReservationType.Items)
            {
                if (!_reservedItemsForRetreival.ContainsAvailableItems(token.Items))
                {
                    throw new InvalidOperationException("Cannot fulfill this reservation");
                }
                _reservedItemsForRetreival.RemoveItems(token.Items);
                _inventoryIncludingReservedCapacity.RemoveItems(token.Items);
                _actualInventory.RemoveItems(token.Items);
            }
            else if (token.ReservationType == ReservationType.Capacity)
            {
                if (!_capacityReservations.ContainsAvailableItems(token.Items))
                {
                    throw new InvalidOperationException("Cannot fulfill this reservation");
                }
                _capacityReservations.RemoveItems(token.Items);
                _availableInventory.AddItems(token.Items);
                _actualInventory.AddItems(token.Items);
            }
            else
            {
                throw new InvalidOperationException("Unknown reservation type");
            }
        }

        public void CancelReservation(ReservationToken<T> token)
        {
            if (token.ReservationType == ReservationType.Items)
            {
                if (!_reservedItemsForRetreival.ContainsAvailableItems(token.Items))
                {
                    throw new InvalidOperationException("Cannot cancel this reservation");
                }
                _reservedItemsForRetreival.RemoveItems(token.Items);
                _availableInventory.AddItems(token.Items);
            }
            else if (token.ReservationType == ReservationType.Capacity)
            {
                if (!_capacityReservations.ContainsAvailableItems(token.Items))
                {
                    throw new InvalidOperationException("Cannot cancel this reservation");
                }
                _capacityReservations.RemoveItems(token.Items);
                _inventoryIncludingReservedCapacity.RemoveItems(token.Items);
            }
            else
            {
                throw new InvalidOperationException("Unknown reservation type");
            }
        }

        public int OccupiedCapacity()
        {
            return _actualInventory.OccupiedCapacity();
        }
    }
}
