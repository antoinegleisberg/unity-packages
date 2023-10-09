using System.IO;
using UnityEditor;
using UnityEngine;

namespace antoinegleisberg.Tools.Editor
{
    public static class ToolsMenu
    {
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            CreateDirectories("", "_Project");
            CreateDirectories("_Project", "Scripts", "Art", "Scenes");

            AssetDatabase.Refresh();
        }

        public static void CreateDirectories(string root, params string[] directories)
        {
            string path = Path.Combine(Application.dataPath, root);
            foreach (string directory in directories)
            {
                Directory.CreateDirectory(Path.Combine(path, directory));
            }
        }

        [MenuItem("Tools/Setup/Create Default Game Objects")]
        public static void CreateDefaultGameObjects()
        {
            GameObject Managers = new GameObject("Managers");
            GameObject Divider1 = new GameObject("--------");
            GameObject Environment = new GameObject("Environment");
            GameObject Divider2 = new GameObject("--------");
            GameObject Units = new GameObject("Units");
            GameObject Divider3 = new GameObject("--------");
            GameObject Canvases = new GameObject("Canvases");
            GameObject Divider4 = new GameObject("--------");
            GameObject Systems = new GameObject("Systems");
        }
    }
}