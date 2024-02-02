using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace antoinegleisberg.SceneManagement
{
    public class SceneManager : MonoBehaviour
    {
        public static SceneManager Instance { get; private set; }

        private HashSet<string> _loadedScenes;

        private void Awake()
        {
            Instance = this;
            _loadedScenes = new HashSet<string>();
        }

        public void LoadScene(string sceneName, Action onSceneLoaded = null)
        {
            LoadScenes(new List<string>() { sceneName }, onSceneLoaded);
        }

        public void LoadScenes(List<string> scenes, Action onScenesLoaded = null)
        {
            StartCoroutine(LoadScenesCoroutine(scenes, onScenesLoaded));
        }

        public void UnloadScenes(string sceneName, Action onSceneUnloaded = null)
        {
            UnloadScenes(new List<string>() { sceneName }, onSceneUnloaded);
        }

        public void UnloadScenes(List<string> scenes, Action onScenesUnloaded = null)
        {
            StartCoroutine(UnloadScenesCoroutine(scenes, onScenesUnloaded));
        }

        private IEnumerator LoadScenesCoroutine(List<string> scenes, Action onScenesLoaded)
        {
            List<AsyncOperation> loadingOperations = new List<AsyncOperation>();

            foreach (string sceneName in scenes)
            {
                if (_loadedScenes.Contains(sceneName))
                {
                    continue;
                }

                loadingOperations.Add(LoadSceneAsync(sceneName));
            }

            foreach (AsyncOperation loadingOperation in loadingOperations)
            {
                yield return new WaitUntil(() => loadingOperation.isDone);
            }

            onScenesLoaded?.Invoke();
        }

        private AsyncOperation LoadSceneAsync(string sceneName)
        {
            AsyncOperation loadingOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            _loadedScenes.Add(sceneName);
            return loadingOperation;
        }

        private IEnumerator UnloadScenesCoroutine(List<string> scenes, Action onScenesUnloaded)
        {
            List<AsyncOperation> unloadingOperations = new List<AsyncOperation>();

            foreach (string sceneName in scenes)
            {
                if (!_loadedScenes.Contains(sceneName))
                {
                    continue;
                }

                unloadingOperations.Add(UnloadSceneAsync(sceneName));
            }

            foreach (AsyncOperation loadingOperation in unloadingOperations)
            {
                yield return new WaitUntil(() => loadingOperation.isDone);
            }

            onScenesUnloaded?.Invoke();
        }

        private AsyncOperation UnloadSceneAsync(string sceneName)
        {
            AsyncOperation unloadingOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            _loadedScenes.Remove(sceneName);
            return unloadingOperation;
        }
    }
}
