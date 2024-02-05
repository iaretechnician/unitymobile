using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    /// <summary>
    /// Adds a shake camera button, to make it easy to test the parameters of the camera shake.
    /// </summary>
    [CustomEditor(typeof(CameraPlayerTracker))]
    public class CameraPlayerTrackerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CameraPlayerTracker cameraPlayerTracker = (CameraPlayerTracker)target;

            if (GUILayout.Button("Shake Camera In Unity Player"))
            {
                cameraPlayerTracker.StartShaking();
            }
        }
    }
}