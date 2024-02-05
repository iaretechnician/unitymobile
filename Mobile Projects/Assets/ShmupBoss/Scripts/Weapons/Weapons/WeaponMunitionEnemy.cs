using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Enemy munition weapon which uses a single weapon munition stage and its parameters affected by multiplier data.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Weapon Munition Enemy")]
    public class WeaponMunitionEnemy : WeaponMunition
    {
        public override float Rate
        {
            get
            {
                return rate * RateControlMultiplier * CurrentMultiplier.EnemiesWeaponsRateMultiplier;
            }
        }

        [SerializeField]
        private WeaponMunitionStage stage;
        public override WeaponMunitionStage CurrentStage
        {
            get
            {
                if (stage == null)
                {
                    Debug.Log("WeaponMunitionEnemy.cs: One of your enemies munition weapons " +
                        "does not have a stage, please double check your enemies munition weapons are not empty");

                    return null;
                }

                return stage;
            }
        }

        private void OnValidate()
        {
            if (stage == null)
            {
                return;
            }

            if (stage.MunitionScale == Vector3.zero)
            {
                stage.MunitionScale = Vector3.one;
            }
        }

        protected override bool CanFindMunitionPool()
        {
            if (MunitionPoolEnemy.Instance == null)
            {
                Debug.Log("WeaponMunitionEnemy.cs: an enemy weapon is trying to fire but " +
                    "it can't find the enemy munition pool instance.");

                return false;
            }

            return true;
        }

        protected override GameObject SpawnMunition()
        {
            return MunitionPoolEnemy.Instance.Spawn(MunitionPrefab.name);
        }

        /// <summary>
        /// Creates in the scene an approximate preview of how the munition would look like after the weapon 
        /// has been operating for the preview duration.<br></br>
        /// This preview does not take into account the rotation of the weapon, locally or nested, or any input 
        /// rotation or the scatter angle.
        /// </summary>
        /// <param name="previewDuration">How the fired munition would like after this duration of time.</param>
        public void CreatePreviewMunitionInEditor(float previewDuration)
        {
            CreatePreviewMunitionInEditor(previewDuration, stage, true);
        }
    }
}