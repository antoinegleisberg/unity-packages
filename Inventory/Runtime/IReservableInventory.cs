using System.Collections.Generic;

namespace antoinegleisberg.Inventory
{
    public enum ReservationType
    {
        Items,
        Capacity
    }

    public class ReservationToken<T>
    {
        private ReservationType _reservationType;
        private IReadOnlyDictionary<T, int> _items;

        public ReservationType ReservationType => _reservationType;
        public IReadOnlyDictionary<T, int> Items => _items;

        internal ReservationToken(ReservationType reservationType, IReadOnlyDictionary<T, int> items)
        {
            _reservationType = reservationType;
            _items = items;
        }
    }

    public interface IReservableInventory<T> : IInventory<T>
    {
        public ReservationToken<T> ReserveItemsForRetrieval(IReadOnlyDictionary<T, int> items);
        public ReservationToken<T> ReserveItemsForRetrieval(T item, int count) => ReserveItemsForRetrieval(new Dictionary<T, int>() { { item, count } });
        public ReservationToken<T> ReserveItemForRetrieval(T item) => ReserveItemsForRetrieval(item, 1);
        
        public ReservationToken<T> ReserveCapacityForItems(IReadOnlyDictionary<T, int> items);
        public ReservationToken<T> ReserveCapacityForItems(T item, int count) => ReserveCapacityForItems(new Dictionary<T, int>() { { item, count } });
        public ReservationToken<T> ReserveCapacityForItem(T item) => ReserveCapacityForItems(item, 1);
    
        public void FulfillReservation(ReservationToken<T> token);

        public void CancelReservation(ReservationToken<T> token);
    }
}
