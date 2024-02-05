using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Player munition weapon which can use multiple weapon munition stages and its parameters are 
    /// affected by multiplier data.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Weapon Munition Player")]
    public class WeaponMunitionPlayer : WeaponMunition
    {
        public event CoreDelegate OnStageChanged;

        public override float Rate
        {
            get
            {
                return rate * RateControlMultiplier * CurrentMultiplier.PlayerWeaponsRateMultiplier;
            }
        }

        [Header("Upgrade Stages")]
        [SerializeField]
        private WeaponMunitionStage[] stages;
        public WeaponMunitionStage[] Stages
        {
            get
            {
                return stages;
            }
        }

        private int stageIndex;
        public int StageIndex
        {
            get
            {
                return stageIndex;
            }
            set
            {
                if (value >= Stages.Length || value < 0)
                {
                    return;
                }

                if (stageIndex != value)
                {
                    stageIndex = value;
                    SetToStage(CurrentStage);
                    RaiseOnStageChanged();
                }
            }
        }

        public override WeaponMunitionStage CurrentStage
        {
            get
            {
                if(Stages.Length == 0)
                {
                    Debug.Log("WeaponMunitionPlayer.cs: One of your player munition weapons " +
                        "does not have any stages, please double check your player munition weapons");

                    return null;
                }
                
                return Stages[StageIndex];
            }
        }

        private void OnValidate()
        {
            if (stages == null || stages.Length <= 0)
            {
                return;
            }

            foreach (WeaponMunitionStage stage in stages)
            {
                if (stage.MunitionScale == Vector3.zero)
                {
                    stage.MunitionScale = Vector3.one;
                }
            }           
        }

        protected override bool CanFindMunitionPool()
        {
            if (MunitionPoolPlayer.Instance == null)
            {
                Debug.Log("WeaponMunitionPlayer.cs: A weapon is trying to fire but it can't find " +
                    "the player munition pool instance.");

                return false;
            }

            return true;
        }

        protected override GameObject SpawnMunition()
        {
            return MunitionPoolPlayer.Instance.Spawn(MunitionPrefab.name);
        }

        /// <summary>
        /// Creates in the scene an approximate preview of how the munition would look like after the weapon 
        /// has been operating for the preview duration.<br></br>
        /// This preview does not take into account the rotation of the weapon, locally or nested, or any input 
        /// rotation or the scatter angle.
        /// </summary>
        /// <param name="previewDuration">How the fired munition would like after this duration of time.</param>
        /// <param name="previewStageIndex">The player weapon upgrade stage which is being previewed.</param>
        public void CreatePreviewMunitionInEditor(float previewDuration, int previewStageIndex)
        {
            if(previewStageIndex > stages.Length-1 || previewStageIndex < 0)
            {
                Debug.Log("WeaponMunitionPlayer.cs: the stage number you are attempting to preview is beyond " +
                    "the numbers of stages that you have, please change your preview stage number.");

                return;
            }

            WeaponMunitionStage stageToPreview = stages[previewStageIndex];

            CreatePreviewMunitionInEditor(previewDuration, stageToPreview, false);
        }

        protected void RaiseOnStageChanged()
        {
            OnStageChanged?.Invoke(null);
        }
    }
}