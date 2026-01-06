using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace antoinegleisberg.Utils
{
    public class ScriptableObjectDB<T> where T : ScriptableObject
    {
        private Dictionary<string, T> _items;

        public ScriptableObjectDB(string resourcesPath)
        {
            _items = Resources.LoadAll<T>(resourcesPath).Select(item => item).ToDictionary(item => item.name, item => item);
        }

        public T GetItemByName(string name)
        {
            if (!_items.ContainsKey(name))
            {
                Debug.LogError($"ScriptableObjectDB does not contain an item with the name {name}");
                return null;
            }

            return _items[name];
        }

        public Dictionary<string, T> GetAllItems()
        {
            return _items;
        }
    }
}
