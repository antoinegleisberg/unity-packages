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
}