using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace antoinegleisberg.InventorySystem
{
    public class Inventory
    {
        private Dictionary<Item, int> _items;
        [SerializeField] private int _maxNbStacks;
        [SerializeField] private int _nbStacks;

        public Inventory(int maxNbStacks)
        {
            _items = new Dictionary<Item, int>();
            _maxNbStacks = maxNbStacks;
            _nbStacks = 0;
        }

        public void AddItems(Item item, int amount)
        {
            if (Contains(item)) _items[item] += amount;
            else _items.Add(item, amount);
        }
        public void AddItem(Item item) => AddItems(item, 1);
        public bool CanRemoveItems(Item item, int amount) => GetNbItems(item) >= amount;
        public void RemoveItems(Item item, int amount)
        {
            Assert.IsTrue(GetNbItems(item) >= amount);
            _items[item] -= amount;
            if (_items[item] == 0) _items.Remove(item);
        }
        public void RemoveItem(Item item) => RemoveItems(item, 1);

        public bool Contains(Item item) => GetNbItems(item) > 0;

        public int GetNbItems(Item item)
        {
            if (_items.ContainsKey(item))
            {
                return _items[item];
            }
            return 0;
        }

        public bool IsEmpty() => _items.Count == 0;

        public Dictionary<Item, int> GetItems() => _items;

        public List<Item> GetListItems()
        {
            List<Item> listItems = new List<Item>();
            foreach (Item item in _items.Keys)
            {
                listItems.Add(item);
            }
            return listItems;
        }
    }
}