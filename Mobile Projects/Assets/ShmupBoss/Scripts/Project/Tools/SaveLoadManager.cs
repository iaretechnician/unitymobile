using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace ShmupBoss
{
    /// <summary>
    /// Saves, loads, renames and deletes save files.
    /// </summary>
    public static class SaveLoadManager
    {
        /// <summary>
        /// Returns the path of the save folder which is stored in a persistent data path.
        /// </summary>
        private static string SaveFolderPath
        {
            get
            {
                return System.IO.Path.Combine(Application.persistentDataPath, ProjectConstants.SaveFolderName);
            }
        }
        
        /// <summary>
        /// Creates a folder in the save folder path if no folder has already existed.
        /// </summary>
        public static void VerifySaveFolder()
        {
            if (Directory.Exists(SaveFolderPath))
            {
                return;
            }

            Directory.CreateDirectory(SaveFolderPath);
        }

        public static void RenameSaveFolder(string newName)
        {
            if (Directory.Exists(SaveFolderPath))
            {
                Directory.Delete(SaveFolderPath, true);
            }

            ProjectConstants.SaveFolderName = newName;
            VerifySaveFolder();
        }

        public static void SaveFile<T>(string fileName, T data) where T: class
        {
            if(fileName == null)
            {
                return;
            }

            string savePath = System.IO.Path.Combine(SaveFolderPath, fileName + ".json");

            // Creates a text save file.
            if (!File.Exists(savePath))
            {
                StreamWriter savedFile = File.CreateText(savePath);
                savedFile.Close();
            }

            string savedDataAsJson = JsonUtility.ToJson(data);
            File.WriteAllText(savePath, savedDataAsJson);
        }

        public static T LoadFile<T>(string fileName) where T: class
        {
            if(fileName == null)
            {
                return null;
            }

            T loadData = null;

            string savePath = System.IO.Path.Combine(SaveFolderPath, fileName + ".json");

            if (savePath.Contains("://"))
            {
                UnityWebRequest www = UnityWebRequest.Get(savePath);

                if(www == null)
                {
                    return null;
                }

                www.SendWebRequest();

                loadData = JsonUtility.FromJson<T>(www.downloadHandler.text);
            }

            if (File.Exists(savePath))
            {
                string DataAsJson = File.ReadAllText(savePath);
                loadData = JsonUtility.FromJson<T>(DataAsJson);
            }

            return loadData;
        }

        /// <summary>
        /// Deletes the save file located in the save folder path if it existed.
        /// </summary>
        /// <param name="fileName">The file name you wish to delete.</param>
        /// <returns>True if it has been deleted, and false if it couldn't find a save file to delete.</returns>
        public static bool DeleteSaveFile(string fileName)
        {
            string filePath = System.IO.Path.Combine(SaveFolderPath, fileName + ".json");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }

            return false;
        }
    }
}