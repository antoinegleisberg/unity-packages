using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace antoinegleisberg.InventorySystem
{
    internal class LimitedSlotSizeInventory<T> : IInventory<T>
    {
        public LimitedSlotSizeInventory(int maxSlots, Func<T, int> maxStackSize)
        {
            
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
