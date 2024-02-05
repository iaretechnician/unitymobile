using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    [CustomEditor(typeof(MunitionHitFX))]
    public class MunitionHitFXEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            MunitionHitFX munitionHitFX = (MunitionHitFX)target;

            if (GUILayout.Button("Preview"))
            {
                Debug.Log("You can only view the preview if the prefab is dropped inside the scene.");

                munitionHitFX.DestroyPreviewFXsInEditor();
                munitionHitFX.CreatePreviewFXsInEditor();
            }

            if (GUILayout.Button("Delete Preview"))
            {
                munitionHitFX.DestroyPreviewFXsInEditor();
            }
        }
    }
}