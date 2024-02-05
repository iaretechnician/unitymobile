using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    [CustomEditor(typeof(FxSpawnerPlayer))]
    public class FxSpawnerPlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            FxSpawnerPlayer fxSpawnerPlayer = (FxSpawnerPlayer)target;

            if (GUILayout.Button("Preview"))
            {
                Debug.Log("You can only view the preview if the prefab is dropped inside the scene.");
                
                fxSpawnerPlayer.DestroyPreviewFXsInEditor();
                fxSpawnerPlayer.CreatePreviewFXsInEditor();
            }

            if (GUILayout.Button("Delete Preview"))
            {
                fxSpawnerPlayer.DestroyPreviewFXsInEditor();
            }
        }
    }
}