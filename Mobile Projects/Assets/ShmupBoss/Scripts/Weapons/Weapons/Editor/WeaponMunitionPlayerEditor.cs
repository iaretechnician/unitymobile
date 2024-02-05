using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    /// <summary>
    /// Editor script for creating preview buttons to approximate how the weapon fired munition would look like 
    /// after a time period.
    /// </summary>
    [CustomEditor(typeof(WeaponMunitionPlayer))]
    public class WeaponMunitionPlayerEditor : Editor
    {
        float previewDuration = ProjectConstants.DefaultPreviewDuration;
        int stageIndex;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Preview Settings", EditorStyles.boldLabel);

            previewDuration = EditorGUILayout.FloatField("Preview Duration", previewDuration);
            stageIndex = EditorGUILayout.IntField("Preview Stage", stageIndex);

            WeaponMunitionPlayer weaponMunitionPlayer = (WeaponMunitionPlayer)target;

            if (GUILayout.Button("Preview"))
            {
                Debug.Log("If you can't see any preview please double check your prefab is already dragged " +
                    "into the scene and that you are not looking inside the prefab window, preview munition " +
                    "are not visible in the prefab menu. Additionally please also Increase the munition speed, " +
                    "firing rate or preview duration or that you are previewing the correct stage number.");

                weaponMunitionPlayer.DestroyPreviewMunitionInEditor();
                weaponMunitionPlayer.CreatePreviewMunitionInEditor(previewDuration, stageIndex);
            }

            if (GUILayout.Button("Delete Preview"))
            {
                weaponMunitionPlayer.DestroyPreviewMunitionInEditor();
            }
        }
    }
}