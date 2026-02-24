using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;


namespace antoinegleisberg.Utils
{
    public static class Utils
    {
        public static void InvokeAfter(float delayInSeconds, Action action)
        {
            IEnumerator InvokeAfterCoroutine() {
                yield return new WaitForSeconds(delayInSeconds);
                action();
            }
            CoroutineRunner.GetRunner().StartCoroutine(InvokeAfterCoroutine());
        }

        public static List<Transform> Children(this Transform transform)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in transform)
            {
                children.Add(child);
            }
            return children;
        }

        private static void DestroyGameObjectByName(string name)
        {
            GameObject gameObject = GameObject.Find(name);
            if (gameObject == null) return;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(gameObject);
            }
            else
#endif
            {
                Object.Destroy(gameObject);
            }
        }
    }
}
