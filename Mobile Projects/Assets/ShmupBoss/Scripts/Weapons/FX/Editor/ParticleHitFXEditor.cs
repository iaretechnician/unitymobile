using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    [CustomEditor(typeof(ParticleHitFX))]
    public class ParticleHitFXEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            ParticleHitFX particleHitFX = (ParticleHitFX)target;

            if (GUILayout.Button("Preview"))
            {
                Debug.Log("You can only view the preview if the prefab is dropped inside the scene.");

                particleHitFX.DestroyPreviewFXsInEditor();
                particleHitFX.CreatePreviewFXsInEditor();
            }

            if (GUILayout.Button("Delete Preview"))
            {
                particleHitFX.DestroyPreviewFXsInEditor();
            }
        }
    }
}