using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BavarianKitchen
{
    public class SceneLoader : MonoBehaviour
    {
        // This is just an example script to demonstrate scene loading.
        
        [SerializeField] private SceneField _sceneToLoad;
        [SerializeField] private SceneField _sceneToUnload;

        void Start()
        {
            StartCoroutine(SwitchScene());
        }

        private IEnumerator SwitchScene()
        {
            yield return new WaitForSeconds(1f);

            SceneManager.Instance.LoadScenes(new List<string> { _sceneToLoad }, () => Debug.Log("I am done loading scenes !!"));
            SceneManager.Instance.UnloadScenes(new List<string> { _sceneToUnload });
        }
    }
}
