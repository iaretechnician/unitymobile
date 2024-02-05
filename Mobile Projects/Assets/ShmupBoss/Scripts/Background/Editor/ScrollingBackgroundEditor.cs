using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    /// <summary>
    /// Adds preview and delete preview buttons to the scrolling background to enable you to test how your 
    /// background will appear in the level.
    /// </summary>
    [CustomEditor(typeof(ScrollingBackground))]
    public class ScrollingBackgroundEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ScrollingBackground scrollingBackground = (ScrollingBackground)target;

            if (GUILayout.Button("Preview"))
            {
                scrollingBackground.DestroyPreviewBackgroundsInEditor();
                scrollingBackground.CreatePreviewBackgrounds();
            }

            if (GUILayout.Button("Delete Preview"))
            {
                scrollingBackground.DestroyPreviewBackgroundsInEditor();
            }
        }
    }
}