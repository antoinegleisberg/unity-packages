using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
using antoinegleisberg.Utils;
using Logger = antoinegleisberg.Utils.Logger;

namespace antoinegleisberg.Saving
{
    internal static class FileDataHandler
    {
        private static readonly string _dataDirectoryPath = Application.persistentDataPath;

        public static void SaveData(string filePath, object data)
        {
            string fullPath = Path.Combine(_dataDirectoryPath, filePath);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                string dataToStore = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Converters = { new ColorConverter(), new Vector3Converter(), new QuaternionConverter() }
                });

                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToStore);
                    }
                }
                Logger.Info($"Save data to {fullPath}");
            }
            catch (Exception e)
            {
                Logger.Critical($"Error when saving data to file {fullPath}: {e.Message}");
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
                    }

                    loadedData = JsonConvert.DeserializeObject(dataToLoad, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        Converters = { new ColorConverter(), new Vector3Converter(), new QuaternionConverter() }
                    });
                    Logger.Info($"Read save data from {fullPath}");
                }
                catch (Exception e)
                {
                    Logger.Fatal($"Error when reading save from file {fullPath}: {e.Message}");
                }
            }
            return loadedData;
        }

        public static void DeleteFile(string filePath)
        {
            string fullPath = Path.Combine(_dataDirectoryPath, filePath);

            if (!File.Exists(fullPath))
            {
                Logger.Error($"File to delete did not exist:\n{fullPath}");
                return;
            }

            Logger.Info($"Deleted file: {fullPath}");
            File.Delete(fullPath);
        }
    }
}