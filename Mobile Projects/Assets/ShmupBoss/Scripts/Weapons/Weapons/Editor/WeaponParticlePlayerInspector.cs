using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    /// <summary>
    /// Draws an inspector with tabs for every stage which makes the particle weapon easier to modify.
    /// </summary>
    [CustomEditor(typeof(WeaponParticlePlayer))]
    public class WeaponParticlePlayerInspector : Editor
    {
        /// <summary>
        /// The selected player particle weapon instance.
        /// </summary>
        private WeaponParticle weaponParticle
        {
            get
            {
                return (WeaponParticle)target;
            }
        }

        /// <summary>
        /// The game object of the selected particle weapon instance.
        /// </summary>
        private GameObject go
        {
            get
            {
                return weaponParticle.gameObject;
            }
        }

        /// <summary>
        /// The particle system of the particle weapon instance that is currently selected.
        /// </summary>
        private ParticleSystem ps
        {
            get
            {
                return go.GetComponent<ParticleSystem>();
            }
        }

        private SerializedProperty IsInitialized;
        private SerializedProperty PS;
        private SerializedProperty Stages;

        [SerializeField]
        private bool isShootingDisabled;

        /// <summary>
        /// the firing time of the next shot (When another particle is allowed to fire.)
        /// </summary>
        [SerializeField]
        private float nextShot;

        /// <summary>
        /// All the currently inspected particle weapon parameters.
        /// </summary>
        [SerializeField]
        private WeaponParticleStage activeStage;

        /// <summary>
        /// a serialized version of all the currently inspected particle weapon parameters.
        /// </summary>
        [SerializeField]
        private SerializedWeaponParticleStage serializedWeaponParticleStage;

        private float timeSinceEditorStartup
        {
            get
            {
                return (float)EditorApplication.timeSinceStartup;
            }
        }

        [SerializeField]
        static int activeTabIndex = -1;
        private int ActiveTabIndex
        {
            get
            {
                return activeTabIndex;
            }
            set
            {
                if (activeTabIndex != value)
                {
                    ChangeTab(value);
                }

                activeTabIndex = value;
            }
        }

        private void OnEnable()
        {
            IsInitialized = serializedObject.FindProperty("IsInitialized");
            PS = serializedObject.FindProperty("PS");
            Stages = serializedObject.FindProperty("Stages");

            if (!EditorApplication.isPlaying)
            {
                weaponParticle.Initialize();
            }

            EditorApplication.update += Update;
        }

        private void Update()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            if (activeStage == null)
            {
                return;
            }

            if (isShootingDisabled)
            {
                return;
            }

            if (timeSinceEditorStartup >= nextShot)
            {
                weaponParticle.Fire(FacingDirections.Player);
                nextShot = timeSinceEditorStartup + 1f / activeStage.Rate;
            }
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawInspector();

            if (!EditorApplication.isPlaying)
            {
                if (!IsInitialized.boolValue)
                {
                    weaponParticle.Initialize();
                }

                if (activeStage != null)
                {
                    UpdateWeaponToActiveStage();
                }                    
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawInspector()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add Stage", GUILayout.Height(25)))
            {
                Stages.InsertArrayElementAtIndex(Stages.arraySize);
            }

            if (GUILayout.Button("Remove Stage", GUILayout.Height(25)) && Stages.arraySize > 0)
            {
                Stages.DeleteArrayElementAtIndex(Stages.arraySize - 1);
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Draws tab buttons for all stages
            for (int i = 0; i < Stages.arraySize; i++)
            {
                if (i == ActiveTabIndex)
                {
                    // This will close the already opened stage tab.
                    if (!GUILayout.Toggle(true, "Stage " + (i + 1).ToString(), GUI.skin.button, GUILayout.Height(40)))
                    {
                        ActiveTabIndex = -1;
                    }

                    // This will draw the open tab and enable changing its settings.
                    if (serializedWeaponParticleStage != null)
                    {
                        activeStage = serializedWeaponParticleStage.GetWeaponStageData();
                        WeaponParticleInspectorTools.DrawStage(activeStage, this);
                        serializedWeaponParticleStage.UpdateSerializedProperty(activeStage);
                    }
                }
                // This will open up a newly selected stage tab.
                else if (GUILayout.Toggle(false, "Stage " + (i + 1).ToString(), GUI.skin.button, GUILayout.Height(40)))
                {
                    ActiveTabIndex = i;
                }

                GUILayout.Space(5);
            }
        }

        private void UpdateWeaponToActiveStage()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            weaponParticle.SetToStage(activeStage);
        }

        private void ChangeTab(int newTabIndex)
        {
            UpdateSerializedActiveStage(newTabIndex);
            SetShooting(newTabIndex);
            ResetStageGUI();
            ps.Clear();
        }

        private void UpdateSerializedActiveStage(int index)
        {
            if (index >= Stages.arraySize || index < 0)
            {
                serializedWeaponParticleStage = null;
                activeStage = null;
            }
            else
            {
                serializedWeaponParticleStage = new SerializedWeaponParticleStage(Stages.GetArrayElementAtIndex(index));
            }
        }

        private void SetShooting(int value)
        {
            if (value == -1)
            {
                isShootingDisabled = true;
            }
            else
            {
                isShootingDisabled = false;
            }

            if (activeStage != null && activeStage.Rate != 0)
            {
                nextShot = timeSinceEditorStartup + 1f / activeStage.Rate;
            }
        }

        private void ResetStageGUI()
        {
            WeaponParticleInspectorTools.ResetGUI();
        }

        /// <summary>
        /// Draw the particles colliders size.
        /// </summary>
        private void OnSceneGUI()
        {
            if (PS.objectReferenceValue != null)
            {
                WeaponParticleInspectorTools.DrawParticlesColliderSize(activeStage, ps);
            }
        }
    }    
}