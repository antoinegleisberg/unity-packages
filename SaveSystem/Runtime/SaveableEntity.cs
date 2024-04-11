using System.Collections.Generic;
using UnityEngine;

namespace antoinegleisberg.Saving
{
    [RequireComponent(typeof(GuidHolder))]
    public class SaveableEntity : MonoBehaviour
    {
        public object GetSaveData()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            foreach (ISaveable savable in GetComponents<ISaveable>())
            {
                state[savable.GetType().ToString()] = savable.GetSaveData();
            }
            return state;
        }
        
        public void LoadData(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
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
