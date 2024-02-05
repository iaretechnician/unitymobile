using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// A player weapon which fires bullets based on a particle system and have multiple upgrade stages.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Weapon Particle Player")]
    public class WeaponParticlePlayer : WeaponParticle
    {
        public event CoreDelegate OnStageChanged;

        public WeaponParticleStage[] Stages;

        public override WeaponParticleStage CurrentStage
        {
            get
            {
                return Stages[StageIndex];
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

        public override float Rate
        {
            get
            {
                return rate * RateControlMultiplier * CurrentMultiplier.PlayerWeaponsRateMultiplier;
            }
        }

        public override float DamageMultiplier
        {
            get
            {
                return CurrentMultiplier.PlayerMunitionDamageMultiplier;
            }
        }

        public override float SpeedMultiplier
        {
            get
            {
                return CurrentMultiplier.PlayerMunitionSpeedMultiplier;
            }
        }

        protected override int layerToHit
        {
            get
            {
                return ProjectLayers.Enemies;
            }
        }

        protected override void OnParticleCollision(GameObject HitTarget)
        {
            base.OnParticleCollision(HitTarget);           

            if (HitTarget.layer == layerToHit)
            {
                Enemy enemy = HitTarget.GetComponent<Enemy>();

                if (enemy == null)
                {
                    Debug.Log("WeaponParticlePlayer.cs: You seem to have assignd an " +
                        "enemy layer to a game object with no enemy componenet.");

                    return;
                }

                enemy.TakeHit(Damage);
            }
        }

        protected void RaiseOnStageChanged()
        {
            OnStageChanged?.Invoke(null);
        }
    }
}