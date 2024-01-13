using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace antoinegleisberg.Inventory
{
    internal class UnlimitedSizeInventory<T> : IInventory<T>
    {
        public void AddItems(Dictionary<T, int> items)
        {
            throw new NotImplementedException();
        }

        public bool CanAddItems(Dictionary<T, int> items)
        {
            throw new NotImplementedException();
        }

        public int GetItemCount(T item)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty()
        {
            throw new NotImplementedException();
        }

        public Dictionary<T, int> Items()
        {
            throw new NotImplementedException();
        }

        public void RemoveItems(Dictionary<T, int> items)
        {
            throw new NotImplementedException();
        }
    }
}
