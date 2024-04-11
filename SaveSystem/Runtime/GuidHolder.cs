using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace antoinegleisberg.Saving
{
    [ExecuteAlways]
    public class GuidHolder : MonoBehaviour
    {
        [SerializeField] private string _uid = "";
        private static Dictionary<string, GuidHolder> _globalLookup = new Dictionary<string, GuidHolder>();

        public string UniqueId {
            get { return _uid; }
            set { _uid = value; }
        }

        private void Awake()
        {
            // Handles objects created at runtime
            CreateUID();
        }

#if UNITY_EDITOR
        // Update method used for generating UUID of the SavableEntity
        private void Update()
        {
            // don't execute in playmode
            if (Application.IsPlaying(gameObject)) return;

            // don't generate Id for prefabs (prefab scene will have path as null)
            if (String.IsNullOrEmpty(gameObject.scene.path)) return;

            CreateUID();
        }
#endif

        private void CreateUID()
        {
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("_uid");

            while (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = Guid.NewGuid().ToString();
                serializedObject.ApplyModifiedProperties();
            }

            _globalLookup[property.stringValue] = this;
        }

        private bool IsUnique(string candidate)
        {
            if (!_globalLookup.ContainsKey(candidate)) return true;

            if (_globalLookup[candidate] == this) return true;

            // Handle scene unloading cases
            if (_globalLookup[candidate] == null)
            {
                _globalLookup.Remove(candidate);
                return true;
            }

            // Handle edge cases like designer manually changing the UUID
            if (_globalLookup[candidate].UniqueId != candidate)
            {
                _globalLookup.Remove(candidate);
                return true;
            }

            return false;
        }
    }
}