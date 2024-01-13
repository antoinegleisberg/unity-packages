using antoinegleisberg.Types;
using System.Collections.Generic;


namespace antoinegleisberg.Inventory
{
    public interface IInventory<T>
    {
        public bool CanAddItems(Dictionary<T, int> items);
        public bool CanAddItems(T item, int count) => CanAddItems(new Dictionary<T, int>() { { item, count } });
        public void CanAddItem(T item) => CanAddItems(item, 1);
        public void AddItems(Dictionary<T, int> items);
        public void AddItems(T item, int count) => AddItems(new Dictionary<T, int>() { { item, count } });
        public void AddItem(T item) => AddItems(item, 1);
        public bool ContainsItems(Dictionary<T, int> items)
        {
            foreach (KeyValuePair<T, int> item in items)
            {
                if (!Contains(item.Key) || GetItemCount(item.Key) < item.Value)
                {
                    return false;
                }
            }
            return true;
        }
        public bool ContainsItems(T item, int count) => ContainsItems(new Dictionary<T, int>() { { item, count } });
        public bool Contains(T item) => GetItemCount(item) > 0;
        public void RemoveItems(Dictionary<T, int> items);
        public void RemoveItems(T item, int count) => RemoveItems(new Dictionary<T, int>() { { item, count } });
        public int GetItemCount(T item);
        public bool IsEmpty();
        public Dictionary<T, int> Items();
    }
}