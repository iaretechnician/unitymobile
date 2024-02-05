using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    /// <summary>
    /// Draws an inspector for the enemy particle weapon to make it easier to change.
    /// </summary>
    [CustomEditor(typeof(WeaponParticleEnemy))]
    public class WeaponParticleEnemyInspector : Editor
    {
        /// <summary>
        /// The selected enemy particle weapon instance.
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

        private SerializedProperty IsInitialized;
        private SerializedProperty PS;
        private SerializedProperty Stage;

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

        private void OnEnable()
        {
            IsInitialized = serializedObject.FindProperty("IsInitialized");
            PS = serializedObject.FindProperty("PS");
            Stage = serializedObject.FindProperty("Stage");

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

            if (timeSinceEditorStartup >= nextShot)
            {
                weaponParticle.Fire(FacingDirections.NonPlayer);
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
            if (serializedWeaponParticleStage == null)
            {
                ActivateTab();
            }

            activeStage = serializedWeaponParticleStage.GetWeaponStageData();
            WeaponParticleInspectorTools.DrawStage(activeStage, this);
            serializedWeaponParticleStage.UpdateSerializedProperty(activeStage);
        }

        private void UpdateWeaponToActiveStage()
        {
            if (EditorApplication.isPlaying)
            {
                return;
            }

            weaponParticle.SetToStage(activeStage);
        }

        private void ActivateTab()
        {
            UpdateSerializedActiveStage();
            ResetStageGUI();
        }

        private void UpdateSerializedActiveStage()
        {
            serializedWeaponParticleStage = new SerializedWeaponParticleStage(Stage);
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
                WeaponParticleInspectorTools.DrawParticlesColliderSize(activeStage, (ParticleSystem)PS.objectReferenceValue);
            }
        }
    }
}