using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    /// <summary>
    /// Adds an initialize camera button which calculates the game field without starting the game.
    /// </summary>
    [CustomEditor(typeof(LevelCamera))]
    public class LevelCameraEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            LevelCamera cam = (LevelCamera)target;

            if (GUILayout.Button("Initialize camera"))
            {
                cam.Initialize();
            }
        }
    }
}