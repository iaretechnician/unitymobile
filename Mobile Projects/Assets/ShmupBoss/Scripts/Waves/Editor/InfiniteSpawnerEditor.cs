using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    /// <summary>
    /// Adds a button for printing the level streams to know which and when stream will be spawned.
    /// </summary>
    [CustomEditor(typeof(InfiniteSpawner))]
    public class InfiniteSpawnerEditor : Editor
    {
        /// <summary>
        /// The number of levels which will be printed with the streams that go inside them.
        /// </summary>
        private int levelsLimit = 50;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Console Print Levels Streams");

            levelsLimit = EditorGUILayout.IntField("Levels Limit", levelsLimit);

            InfiniteSpawner infiniteSpawner = (InfiniteSpawner)target;

            if (GUILayout.Button("Print Levels Streams"))
            {
                infiniteSpawner.PrintLevelStreams(levelsLimit);
            }
        }
    }
}