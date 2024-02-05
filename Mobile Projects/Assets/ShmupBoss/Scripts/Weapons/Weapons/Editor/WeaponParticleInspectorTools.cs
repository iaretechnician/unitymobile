using UnityEngine;
using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    /// <summary>
    /// Tools for drawing the particle weapon stages and gizmos.
    /// </summary>
    public static class WeaponParticleInspectorTools
    {
        [SerializeField]
        static bool isCurveTabActivated;

        [SerializeField]
        static bool isTrailTabActivated;

        [SerializeField]
        static ObjectContainer BulletMaterialCont;

        [SerializeField]
        static ObjectContainer FiringSoundCont;

        [SerializeField]
        static ObjectContainer TrailMaterialCont;

        static WeaponParticleInspectorTools()
        {
            ResetGUI();
        }

        public static void ResetGUI()
        {
            isCurveTabActivated = false;
            isTrailTabActivated = false;

            BulletMaterialCont = new ObjectContainer(null);
            FiringSoundCont = new ObjectContainer(null);
            TrailMaterialCont = new ObjectContainer(null);
        }

        /// <summary>
        /// Draws the particle weapon stage, typically after its tab has been selected.
        /// </summary>
        /// <param name="stage">The particle weapon stage to draw.</param>
        /// <param name="editor">The inspector which the stage will be drawn in.</param>
        public static void DrawStage(WeaponParticleStage stage, Editor editor)
        {
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            
            if (BulletMaterialCont.ContainedObject != null)
            {
                stage.BulletMaterial = BulletMaterialCont.ContainedObject;
            }

            if (GUILayout.Button("Pick Bullet", GUILayout.Height(20)))
            {
                PickByIcon Bullet_PickWindow = EditorWindow.GetWindow<PickByIcon>(true, "Picking Bullet");
                Bullet_PickWindow.WindowsInitialize(BulletMaterialCont, "bullets", "mat", editor);
            }
             
            BulletMaterialCont.ContainedObject = EditorGUILayout.ObjectField("", stage.BulletMaterial, typeof(Material), false);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            if (FiringSoundCont.ContainedObject != null)
            {
                stage.FiringSFX = FiringSoundCont.ContainedObject;
            }

            FiringSoundCont.ContainedObject = EditorGUILayout.ObjectField("SFX :", stage.FiringSFX, typeof(AudioClip), false);

            GUILayout.Space(5);
            stage.SfxVolume = EditorGUILayout.Slider("SFX Volume :", stage.SfxVolume, 0.0f, ProjectConstants.MaxAudioClipVolume);

            GUILayout.Space(5);
            stage.SfxCoolDownTime = EditorGUILayout.FloatField("SFX Cool Down Time :", stage.SfxCoolDownTime);

            GUILayout.Space(5);
            stage.Rate = EditorGUILayout.FloatField("Rate :", stage.Rate);

            GUILayout.Space(5);
            stage.Damage = EditorGUILayout.FloatField("Damage :", stage.Damage);

            GUILayout.Space(5);
            stage.Size = EditorGUILayout.FloatField("Size :", stage.Size);

            GUILayout.Space(5);
            stage.ColliderSize = EditorGUILayout.FloatField("Collider Size :", stage.ColliderSize);

            GUILayout.Space(5);
            stage.Speed = EditorGUILayout.FloatField("Speed :", stage.Speed);

            GUILayout.Space(5);
            stage.RotationSpeed = EditorGUILayout.FloatField("RotationSpeed :", stage.RotationSpeed);

            GUILayout.Space(5);
            stage.BulletsNumber = EditorGUILayout.IntField("Bullet Number :", stage.BulletsNumber);

            GUILayout.Space(5);
            stage.ArcAngleSpread = EditorGUILayout.FloatField("Arc Angle Spread :", stage.ArcAngleSpread);

            GUILayout.Space(5);
            stage.ArcRadius = EditorGUILayout.FloatField("Arc Radius :", stage.ArcRadius);

            GUILayout.Space(5);
            stage.Distance = EditorGUILayout.FloatField("Distance :", stage.Distance);

            GUILayout.Space(5);
            stage.Overtime = EditorGUILayout.FloatField("Overtime :", stage.Overtime);

            GUILayout.Space(5);
            stage.IsFollowingRotation = EditorGUILayout.Toggle("Is FollowingRotation :", stage.IsFollowingRotation);

            // Responsible for opening up the curve tab and showing its fields.
            GUILayout.Space(20);
            isCurveTabActivated = GUILayout.Toggle(isCurveTabActivated, "Curve", GUI.skin.button, GUILayout.Height(25));

            if (isCurveTabActivated)
            {
                GUILayout.Space(10);

                stage.Curve = EditorGUILayout.CurveField(stage.Curve, Color.cyan, new Rect(0, -1, 1, 2), GUILayout.Height(30));

                GUILayout.Space(5);
                stage.CurveRange = EditorGUILayout.FloatField("Curve Range :", stage.CurveRange);

                GUILayout.Space(10);
            }

            // Responsible for opening up the trail tab and showing its fields.
            GUILayout.Space(10);
            isTrailTabActivated = GUILayout.Toggle(isTrailTabActivated, "Trail", GUI.skin.button, GUILayout.Height(25));

            if (isTrailTabActivated)
            {
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();

                if (TrailMaterialCont.ContainedObject != null)
                {
                    stage.TrailMaterial = TrailMaterialCont.ContainedObject;
                }

                if (GUILayout.Button("Pick Trail", GUILayout.Height(20)))
                {
                    PickByIcon Trail_PickWindow = EditorWindow.GetWindow<PickByIcon>(true, "Picking Trail");
                    Trail_PickWindow.WindowsInitialize(TrailMaterialCont, "trails", "mat", editor);
                }

                TrailMaterialCont.ContainedObject = EditorGUILayout.ObjectField("", stage.TrailMaterial, typeof(Material), false);

                GUILayout.EndHorizontal();

                if (GUILayout.Button("Remove Trail", GUILayout.Height(20)))
                {
                    stage.TrailMaterial = null;
                    TrailMaterialCont.ContainedObject = null;
                }

                GUILayout.Space(5);
                stage.TrailTime = EditorGUILayout.FloatField("Trail Time :", stage.TrailTime);

                GUILayout.Space(5);
                stage.TrailWidth = EditorGUILayout.FloatField("Trail Width :", stage.TrailWidth);

                GUILayout.Space(10);
            }
        }

        public static void DrawParticlesColliderSize(WeaponParticleStage stage, ParticleSystem ps)
        {
            if (stage == null)
            {
                return;
            }

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[100];
            ps.GetParticles(particles);

            for (int i = 0; i < particles.Length; i++)
            {
                Handles.DrawWireArc(particles[i].position, Vector3.forward, Vector3.right, 360.0f, stage.ColliderSize * 0.5f * stage.Size);
            }
        }
    }   
}