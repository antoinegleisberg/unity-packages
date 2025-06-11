using UnityEngine;

namespace antoinegleisberg.Utils
{
    public static class Utils
    {
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
