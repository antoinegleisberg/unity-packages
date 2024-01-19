using antoinegleisberg.Types;
using System.Collections.Generic;
using UnityEngine;

namespace antoinegleisberg.SaveSystem
{
    [RequireComponent(typeof(GuidHolder))]
    public class SaveableEntity : MonoBehaviour
    {
        public object GetSaveData()
        {
            SerializableDictionary<string, object> state = new SerializableDictionary<string, object>();
            foreach (ISaveable savable in GetComponents<ISaveable>())
            {
                state[savable.GetType().ToString()] = savable.GetSaveData();
                Debug.Log($"Found saveable data for entity {savable.GetType()}: {savable.GetSaveData()}");
            }
            return state;
        }
        
        public void LoadData(object state)
        {
            SerializableDictionary<string, object> stateDict = (SerializableDictionary<string, object>)state;
            foreach (ISaveable savable in GetComponents<ISaveable>())
            {
                string id = savable.GetType().ToString();

                if (stateDict.ContainsKey(id))
                {
                    savable.LoadData(stateDict[id]);
                }
            }
        }
    }
}
