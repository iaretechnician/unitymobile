using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    [CustomEditor(typeof(FxSpawnerEnemy))]
    public class FxSpawnerEnemyEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            FxSpawnerEnemy fxSpawnerenemy = (FxSpawnerEnemy)target;

            if (GUILayout.Button("Preview"))
            {
                Debug.Log("You can only view the preview if the prefab is dropped inside the scene.");

                fxSpawnerenemy.DestroyPreviewFXsInEditor();
                fxSpawnerenemy.CreatePreviewFXsInEditor();
            }

            if (GUILayout.Button("Delete Preview"))
            {
                fxSpawnerenemy.DestroyPreviewFXsInEditor();
            }
        }
    }
}