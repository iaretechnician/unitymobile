using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    [CustomEditor(typeof(WeaponFireFX))]
    public class WeaponFireFxEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            WeaponFireFX weaponFireFX = (WeaponFireFX)target;

            if (GUILayout.Button("Preview"))
            {
                Debug.Log("If you can't see any preview, please not that you can only view the preview if the " +
                    "prefab is dropped inside the scene.");

                weaponFireFX.DestroyPreviewFXsInEditor();
                weaponFireFX.CreatePreviewFXsInEditor();
            }

            if (GUILayout.Button("Delete Preview"))
            {
                weaponFireFX.DestroyPreviewFXsInEditor();
            }
        }
    }
}