using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace antoinegleisberg.SaveSystem
{
    public static class SaveSystem
    {
        public static void LoadSave(string filePath)
        {
            object gameData = FileDataHandler.LoadData(filePath);

            if (gameData == null)
            {
                // it is a new save
                return;
            }

            LoadEntitiesState(Object.FindObjectsOfType<SaveableEntity>().ToList(), gameData);
        }

        public static void SaveGame(string filePath)
        {
            object gameState = SaveEntitiesState(Object.FindObjectsOfType<SaveableEntity>().ToList());

            FileDataHandler.SaveData(filePath, gameState);
        }

        public static void DeleteSaveFile(string filePath)
        {
            FileDataHandler.DeleteFile(filePath);
        }

        public static void LoadScene(string sceneName, object entitiesState)
        {
            List<SaveableEntity> saveableEntities = FindSaveableEntitiesInScene(sceneName);

            LoadEntitiesState(saveableEntities, entitiesState);
        }

        public static void LoadScene(string sceneName, string filePath)
        {
            object entitiesState = FileDataHandler.LoadData(filePath);
            
            LoadScene(sceneName, entitiesState);
        }

        public static object SaveScene(string sceneName)
        {
            List<SaveableEntity> saveableEntities = FindSaveableEntitiesInScene(sceneName);

            object entitiesState = SaveEntitiesState(saveableEntities);

            return entitiesState;
        }

        public static void SaveScene(string sceneName, string filePath)
        {
            object entitiesState = SaveScene(sceneName);

            FileDataHandler.SaveData(filePath, entitiesState);
        }

        public static object LoadObject(string fileName)
        {
            return FileDataHandler.LoadData(fileName);
        }

        public static void SaveObject(string fileName, object data)
        {
            FileDataHandler.SaveData(fileName, data);
        }

        private static void LoadEntitiesState(List<SaveableEntity> saveableEntities, object entitiesState)
        {
            Dictionary<string, object> gameState = (Dictionary<string, object>)entitiesState;

            foreach (SaveableEntity saveableEntity in saveableEntities)
            {
                string uid = saveableEntity.GetComponent<GuidHolder>().UniqueId;
                if (gameState.ContainsKey(uid))
                {
                    saveableEntity.LoadData(gameState[uid]);
                }
            }
        }

        private static object SaveEntitiesState(List<SaveableEntity> saveableEntities)
        {
            Dictionary<string, object> gameState = new Dictionary<string, object>();

            foreach (SaveableEntity saveableEntity in saveableEntities)
            {
                string uid = saveableEntity.GetComponent<GuidHolder>().UniqueId;
                gameState[uid] = saveableEntity.GetSaveData();
            }

            return gameState;
        }

        private static List<SaveableEntity> FindSaveableEntitiesInScene(string sceneName)
        {
            List<SaveableEntity> saveables =
                Object.FindObjectsOfType<SaveableEntity>()
                .Where(x => x.gameObject.scene.name == sceneName)
                .ToList();

            return saveables;
        }
    }
}