using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    /// <summary>
    /// Adds an edit menu button to enable deleting the save and settings files.
    /// </summary>
    public class FileManagement : ScriptableObject
    {
        /// <summary>
        /// Deletes the save and settings files using the SaveLoadManager script.
        /// </summary>
        [MenuItem("Edit/Shmup Boss/Delete Save Files", false, 1101)]
        private static void Delete()
        {
            SaveLoadManager.DeleteSaveFile(ProjectConstants.SaveFileName);
            SaveLoadManager.DeleteSaveFile(ProjectConstants.SettingsFileName);
        }
    }
}