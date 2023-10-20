using System;
using System.Collections.Generic;


namespace antoinegleisberg.InventorySystem
{
    internal class LimitedSlotsInventory<T> : IInventory<T>
    {
        public LimitedSlotsInventory(int maxSlots)
        {
            throw new NotImplementedException();
        }

        public void AddItems(T item, int count)
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public int GetItemCount(T item)
        {
            throw new NotImplementedException();
        }

        public List<T> GetItemsList()
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        public void RemoveItems(T item, int count)
        {
            throw new NotImplementedException();
        }
    }
}
