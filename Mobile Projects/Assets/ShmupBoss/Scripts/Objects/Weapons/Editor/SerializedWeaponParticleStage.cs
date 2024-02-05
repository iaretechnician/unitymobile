using UnityEditor;
using ShmupBoss;

namespace ShmupBossEditor
{
    /// <summary>
    /// Contains all the serialized properties of class: WeaponParticleStage which can be used to deal 
    /// with it for the inspector.
    /// </summary>
    public class SerializedWeaponParticleStage
    {
        public SerializedProperty Rate;
        public SerializedProperty Damage;
        public SerializedProperty Size;
        public SerializedProperty ColliderSize;
        public SerializedProperty Speed;
        public SerializedProperty RotationSpeed;

        public SerializedProperty BulletsNumber;
        public SerializedProperty ArcAngleSpread;
        public SerializedProperty ArcRadius;

        public SerializedProperty Distance;
        public SerializedProperty Overtime;

        public SerializedProperty IsFollowingRotation;

        public SerializedProperty BulletMaterial;

        public SerializedProperty FiringSFX;
        public SerializedProperty SfxVolume;
        public SerializedProperty SfxCoolDownTime;

        public SerializedProperty TrailMaterial;
        public SerializedProperty TrailTime;
        public SerializedProperty TrailWidth;

        public SerializedProperty Curve;
        public SerializedProperty CurveRange;

        /// <summary>
        /// Constructor using the class: WeaponParticleStage as a serialized property.
        /// </summary>
        /// <param name="SerializedStage">WeaponParticleStage class as a serialized property.</param>
        public SerializedWeaponParticleStage(SerializedProperty SerializedStage)
        {            
            Rate = SerializedStage.FindPropertyRelative("Rate");
            Damage = SerializedStage.FindPropertyRelative("Damage");
            Speed = SerializedStage.FindPropertyRelative("Speed");
            Size = SerializedStage.FindPropertyRelative("Size");
            ColliderSize = SerializedStage.FindPropertyRelative("ColliderSize");
            RotationSpeed = SerializedStage.FindPropertyRelative("RotationSpeed");

            BulletsNumber = SerializedStage.FindPropertyRelative("BulletsNumber");
            ArcAngleSpread = SerializedStage.FindPropertyRelative("ArcAngleSpread");
            ArcRadius = SerializedStage.FindPropertyRelative("ArcRadius");

            Distance = SerializedStage.FindPropertyRelative("Distance");
            Overtime = SerializedStage.FindPropertyRelative("Overtime");

            IsFollowingRotation = SerializedStage.FindPropertyRelative("IsFollowingRotation");

            BulletMaterial = SerializedStage.FindPropertyRelative("BulletMaterial");

            FiringSFX = SerializedStage.FindPropertyRelative("FiringSFX");
            SfxVolume = SerializedStage.FindPropertyRelative("SfxVolume");
            SfxCoolDownTime = SerializedStage.FindPropertyRelative("SfxCoolDownTime");

            TrailMaterial = SerializedStage.FindPropertyRelative("TrailMaterial");
            TrailTime = SerializedStage.FindPropertyRelative("TrailTime");
            TrailWidth = SerializedStage.FindPropertyRelative("TrailWidth");

            Curve = SerializedStage.FindPropertyRelative("Curve");
            CurveRange = SerializedStage.FindPropertyRelative("CurveRange");
        }

        /// <summary>
        /// Updates all the serialized properties by a WeaponParticleStage class.
        /// </summary>
        /// <param name="stage">The WeaponParticleStage whose fields will be used to update the 
        /// serialized properties.</param>
        public void UpdateSerializedProperty(WeaponParticleStage stage)
        {            
            Rate.floatValue = stage.Rate;
            Damage.floatValue = stage.Damage;
            Speed.floatValue = stage.Speed;
            Size.floatValue = stage.Size;
            ColliderSize.floatValue = stage.ColliderSize;
            RotationSpeed.floatValue = stage.RotationSpeed;

            BulletsNumber.intValue = stage.BulletsNumber;
            ArcAngleSpread.floatValue = stage.ArcAngleSpread;
            ArcRadius.floatValue = stage.ArcRadius;

            Distance.floatValue = stage.Distance;
            Overtime.floatValue = stage.Overtime;

            IsFollowingRotation.boolValue = stage.IsFollowingRotation;

            BulletMaterial.objectReferenceValue = stage.BulletMaterial;

            FiringSFX.objectReferenceValue = stage.FiringSFX;
            SfxVolume.floatValue = stage.SfxVolume;
            SfxCoolDownTime.floatValue = stage.SfxCoolDownTime;

            TrailMaterial.objectReferenceValue = stage.TrailMaterial;
            TrailTime.floatValue = stage.TrailTime;
            TrailWidth.floatValue = stage.TrailWidth;

            Curve.animationCurveValue = stage.Curve;
            CurveRange.floatValue = stage.CurveRange;
        }

        /// <summary>
        /// Translates serialized properties into a newly created instance of class: WeaponParticleStage
        /// </summary>
        /// <returns>The class instance of: WeaponParticleStage which has all the values of the serialized 
        /// properties.</returns>
        public WeaponParticleStage GetWeaponStageData()
        {
            WeaponParticleStage stage = new WeaponParticleStage();
            
            stage.Rate = Rate.floatValue;
            stage.Damage = Damage.floatValue;           
            stage.Speed = Speed.floatValue;
            stage.Size = Size.floatValue;
            stage.ColliderSize = ColliderSize.floatValue;
            stage.RotationSpeed = RotationSpeed.floatValue;

            stage.BulletsNumber = BulletsNumber.intValue;
            stage.ArcAngleSpread = ArcAngleSpread.floatValue;
            stage.ArcRadius = ArcRadius.floatValue;

            stage.Distance = Distance.floatValue;
            stage.Overtime = Overtime.floatValue;

            stage.IsFollowingRotation = IsFollowingRotation.boolValue;

            stage.BulletMaterial = BulletMaterial.objectReferenceValue;

            stage.FiringSFX = FiringSFX.objectReferenceValue;
            stage.SfxVolume = SfxVolume.floatValue;
            stage.SfxCoolDownTime = SfxCoolDownTime.floatValue;

            stage.TrailMaterial = TrailMaterial.objectReferenceValue;
            stage.TrailTime = TrailTime.floatValue;
            stage.TrailWidth = TrailWidth.floatValue;

            stage.Curve = Curve.animationCurveValue;
            stage.CurveRange = CurveRange.floatValue;

            return stage;
        }
    }
}