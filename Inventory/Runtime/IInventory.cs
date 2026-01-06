using System.Collections.Generic;


namespace antoinegleisberg.Inventory
{
    public interface IInventory<T>
    {
        public bool CanAddItems(IReadOnlyDictionary<T, int> items);
        public bool CanAddItems(T item, int count) => CanAddItems(new Dictionary<T, int>() { { item, count } });
        public bool CanAddItem(T item) => CanAddItems(item, 1);

        public void AddItems(IReadOnlyDictionary<T, int> items);
        public void AddItems(T item, int count) => AddItems(new Dictionary<T, int>() { { item, count } });
        public void AddItem(T item) => AddItems(item, 1);
        
        public bool ContainsAvailableItems(IReadOnlyDictionary<T, int> items);
        public bool ContainsAvailableItems(T item, int count) => ContainsAvailableItems(new Dictionary<T, int>() { { item, count } });
        public bool ContainsAvailableItem(T item) => ContainsAvailableItems(item, 1);
        
        public void RemoveItems(IReadOnlyDictionary<T, int> items);
        public void RemoveItems(T item, int count) => RemoveItems(new Dictionary<T, int>() { { item, count } });
        
        public IReadOnlyDictionary<T, int> AvailableItems();

        public int GetAvailableItemCount(T item);

        public int GetAvailableCapacityForItem(T item);

        public (int addedQuantity, int remainingQuantity) AddAsManyAsPossible(T item, int count);

        public (IReadOnlyDictionary<T, int> addedItems, IReadOnlyDictionary<T, int> remainingItems) AddAsManyAsPossible(IReadOnlyDictionary<T, int> items)
        {
            Dictionary<T, int> addedItems = new Dictionary<T, int>();
            Dictionary<T, int> remainingItems = new Dictionary<T, int>();

            foreach (KeyValuePair<T, int> item in items)
            {
                (int addedQuantity, int remainingQuantity) = AddAsManyAsPossible(item.Key, item.Value);
                addedItems.Add(item.Key, addedQuantity);
                remainingItems.Add(item.Key, remainingQuantity);
            }

            return (addedItems, remainingItems);
        }

        public bool IsEmpty();
        
        public IReadOnlyDictionary<T, int> Items();

        public int OccupiedCapacity();
    }
}