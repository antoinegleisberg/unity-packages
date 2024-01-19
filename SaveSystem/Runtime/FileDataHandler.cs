using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace antoinegleisberg.SaveSystem
{
    public static class FileDataHandler
    {
        private static readonly string _dataDirectoryPath = Application.persistentDataPath;

        public static void SaveData(string filePath, object data)
        {
            string fullPath = Path.Combine(_dataDirectoryPath, filePath);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                string dataToStore = JsonUtility.ToJson(data, true);

                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToStore);
                    }

                    // or 
                    // BinaryFormatter binaryFormatter = new BinaryFormatter();
                    // binaryFormatter.Serialize(stream, data);
                }
                Debug.Log($"Save data to {fullPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error when saving data to file {fullPath}: {e.Message}");
            }
        }

        public static object LoadData(string filePath)
        {
            string fullPath = Path.Combine(_dataDirectoryPath, filePath);

            object loadedData = null;
            if (File.Exists(fullPath))
            {
                try
                {
                    string dataToLoad = "";
                    using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                        
                        // or
                        // BinaryFormatter binaryFormatter = new BinaryFormatter();
                        // loadedData = binaryFormatter.Deserialize(stream);
                    }

                    loadedData = JsonUtility.FromJson<object>(dataToLoad);
                    Debug.Log($"Read save data from {fullPath}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error when reading save from file {fullPath}: {e.Message}");
                }
            }
            return loadedData;
        }

        public static void DeleteFile(string filePath)
        {
            string fullPath = Path.Combine(_dataDirectoryPath, filePath);

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"File to delete did not exist:\n{fullPath}");
                return;
            }

            Debug.Log($"Deleted file: {fullPath}");
            File.Delete(fullPath);
        }
    }
}