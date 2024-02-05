using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// An enemy weapon which fires bullets based on a particle system.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Weapon Particle Enemy")]
    public class WeaponParticleEnemy : WeaponParticle
    {
        public WeaponParticleStage Stage;

        public override WeaponParticleStage CurrentStage
        {
            get
            {
                return Stage;
            }
        }

        public override float Rate
        {
            get
            {
                return rate * RateControlMultiplier * CurrentMultiplier.EnemiesWeaponsRateMultiplier;
            }
        }

        public override float DamageMultiplier
        {
            get
            {
                return CurrentMultiplier.EnemiesMunitionDamageMultiplier;
            }
        }

        public override float SpeedMultiplier
        {
            get
            {
                return CurrentMultiplier.EnemiesMunitionSpeedMultiplier;
            }
        }

        protected override int layerToHit
        {
            get
            {
                return ProjectLayers.Player;
            }
        }

        protected override void OnParticleCollision(GameObject HitTarget)
        {
            base.OnParticleCollision(HitTarget);

            if (HitTarget.layer == layerToHit)
            {
                Player player = HitTarget.GetComponent<Player>();

                if (player == null)
                {
                    Debug.Log("WeaponParticleEnemy.cs: You seem to have assigned a " +
                        "player layer to a game object with no player componenet.");

                    return;
                }

                player.TakeHit(Damage);
            }
        }
    }
}