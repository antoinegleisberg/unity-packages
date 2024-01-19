using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using antoinegleisberg.Types;

namespace antoinegleisberg.SaveSystem
{
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        private SerializableDictionary<string, object> _gameState = new SerializableDictionary<string, object>();

        private void Awake()
        {
            Instance = this;
        }

        public void LoadSave(string filePath)
        {
            object gameData = FileDataHandler.LoadData(filePath);

            _gameState = (SerializableDictionary<string, object>)gameData;

            LoadEntitiesState(FindObjectsOfType<SaveableEntity>().ToList());
        }

        public void SaveGame(string filePath)
        {
            SaveEntitiesState(FindObjectsOfType<SaveableEntity>().ToList());

            FileDataHandler.SaveData(filePath, _gameState);
        }

        public void DeleteSaveFile(string filePath)
        {
            FileDataHandler.DeleteFile(filePath);
        }

        public void LoadScene(string sceneName)
        {
            List<SaveableEntity> saveableEntities = FindSaveableEntitiesInScene(sceneName);

            LoadEntitiesState(saveableEntities);
        }

        public void SaveScene(string sceneName)
        {
            List<SaveableEntity> saveableEntities = FindSaveableEntitiesInScene(sceneName);

            SaveEntitiesState(saveableEntities);
        }

        private void LoadEntitiesState(List<SaveableEntity> saveableEntities)
        {
            foreach (SaveableEntity saveableEntity in saveableEntities)
            {
                //string sceneName = saveableEntity.gameObject.scene.name;
                //string uid = saveableEntity.GetComponent<GuidHolder>().UniqueId;
                //if (_gameState.ContainsKey(sceneName) && _gameState[sceneName].ContainsKey(uid))
                //{
                //    saveableEntity.LoadData(_gameState[sceneName][uid]);
                //}
                string uid = saveableEntity.GetComponent<GuidHolder>().UniqueId;
                if (_gameState.ContainsKey(uid))
                {
                    saveableEntity.LoadData(_gameState[uid]);
                }
            }
        }

        private void SaveEntitiesState(List<SaveableEntity> saveableEntities)
        {
            foreach (SaveableEntity saveableEntity in saveableEntities)
            {
                //string sceneName = saveableEntity.gameObject.scene.name;
                //string uid = saveableEntity.GetComponent<GuidHolder>().UniqueId;
                //if (!_gameState.ContainsKey(sceneName))
                //{
                //    _gameState.Add(sceneName, new SerializableDictionary<string, object>());
                //}
                //_gameState[sceneName][uid] = saveableEntity.GetSaveData();
                string uid = saveableEntity.GetComponent<GuidHolder>().UniqueId;
                _gameState[uid] = saveableEntity.GetSaveData();
                Debug.Log($"Saved data for {saveableEntity.name}: {_gameState[uid]}");
            }
        }

        //private void AddSaveableObjects(string sceneName)
        //{
        //    RemoveSaveableObjects(sceneName);
        //    gameState.Add(sceneName, FindSaveableObjects(sceneName));
        //}

        //private void RemoveSaveableObjects(string sceneName)
        //{
        //    if (gameState.ContainsKey(sceneName))
        //        gameState.Remove(sceneName);
        //}

        private List<SaveableEntity> FindSaveableEntitiesInScene(string sceneName)
        {
            List<SaveableEntity> saveables =
                FindObjectsOfType<SaveableEntity>()
                .Where(x => x.gameObject.scene.name == sceneName)
                .ToList();

            //if (saveables.Count > 0)
            //{
            //    StringBuilder builder = new StringBuilder($"Found {saveables.Count} saveables in scene {sceneName}: ");
            //    foreach (ISaveable saveableObject in saveables)
            //    {
            //        builder.AppendLine(saveableObject.ToString());
            //    }
            //    Debug.Log(builder.ToString());
            //}

            return saveables;
        }
    }
}