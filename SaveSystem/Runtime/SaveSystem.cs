using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace antoinegleisberg.SaveSystem
{
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        private Dictionary<string, Dictionary<string, object>> _gameState;

        private void Awake()
        {
            Instance = this;
            _gameState = new Dictionary<string, Dictionary<string, object>>();
        }

        public void LoadSave(string filePath)
        {
            object gameData = FileDataHandler.LoadData(filePath);

            _gameState = (Dictionary<string, Dictionary<string, object>>)gameData;

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
                string sceneName = saveableEntity.gameObject.scene.name;
                string uid = saveableEntity.GetComponent<GuidHolder>().UniqueId;
                if (_gameState.ContainsKey(sceneName) && _gameState[sceneName].ContainsKey(uid))
                {
                    saveableEntity.LoadData(_gameState[sceneName][uid]);
                }
            }
        }

        private void SaveEntitiesState(List<SaveableEntity> saveableEntities)
        {
            foreach (SaveableEntity saveableEntity in saveableEntities)
            {
                string sceneName = saveableEntity.gameObject.scene.name;
                string uid = saveableEntity.GetComponent<GuidHolder>().UniqueId;
                _gameState[sceneName][uid] = saveableEntity.GetSaveData();
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