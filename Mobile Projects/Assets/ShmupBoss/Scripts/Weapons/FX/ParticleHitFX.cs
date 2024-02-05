using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// This will make particles spawned by a particle weapon spawn an FX when they hit a target.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Components/Particle Hit FX")]
    [RequireComponent(typeof(WeaponParticle))]
    public class ParticleHitFX : ArmamentFX
    {
        /// <summary>
        /// If true, the spawned FX will have an angle related to the angle which it hit the target with.
        /// </summary>
        [Tooltip("When checked, the spawned FX will have an angle related to the angle which it hit the " +
            "target with.")]
        [SerializeField]
        private bool isUsingHitAngleRotation;

        /// <summary>
        /// Only used if isUsingHitAngleRotation is set to true, this will add or increment degrees to 
        /// the resulting spawned FX rotation which is the result of the hit angle.
        /// </summary>
        [Tooltip("Only used if isUsingHitAngleRotation is checked, this will add or increment degrees to " +
            "the resulting spawned FX rotation which is the result of the hit angle.")]
        [SerializeField]
        private float angleOffest;

        /// <summary>
        /// The sound effect played when a particle hits a layer it collides with.
        /// </summary>
        [Tooltip("The sound effect played when a particle hits a layer it collides with.")]
        [SerializeField]
        protected VolumetricAC hitSFX;

        /// <summary>
        /// The particle weapon which when its particle hit a layer they collide with will spawn a hit FX.
        /// </summary>
        private WeaponParticle weaponParticle;

        protected override void CacheArmamentAndBindMethods()
        {
            weaponParticle = GetComponent<WeaponParticle>();

            if (weaponParticle == null)
            {
                Debug.Log("ParticleHitFX.cs: particle hit fx needs to be added " +
                    "to a particle weapon, please make sure that you have a particle weapon component.");

                return;
            }

            weaponParticle.OnParticleHit += Spawn;

            if (hitSFX.AC != null)
            {
                weaponParticle.OnParticleHit += PlaySFX;
            }
        }

        protected override void Spawn(System.EventArgs args)
        {
            ParticleHitArgs particleHitArgs = (ParticleHitArgs)args;

            if(particleHitArgs == null)
            {
                return;
            }

            if (FXPool.Instance == null)
            {
                return;
            }

            GameObject weaponFireFxGO = FXPool.Instance.Spawn(FxGo.name);

            if (weaponFireFxGO == null)
            {
                Debug.Log("ParticleHitFX.cs: unable to spawn weapon fire fx go from weapon FX pool.");

                return;
            }

            weaponFireFxGO.transform.position = new Vector3(particleHitArgs.HitPosition.x, particleHitArgs.HitPosition.y, particleHitArgs.HitPosition.z) + offsetPosition;
            if (isUsingHitAngleRotation)
            {
                weaponFireFxGO.transform.rotation = Quaternion.Euler(0.0f, 0.0f, particleHitArgs.HitAngle + angleOffest);
            }
            else
            {
                weaponFireFxGO.transform.localRotation = Quaternion.Euler(newRotation);
            }

            weaponFireFxGO.transform.localScale = modifiedScale;
        }

        private void PlaySFX(System.EventArgs args)
        {
            SfxSource.Instance.PlayClip(hitSFX.AC, hitSFX.Volume);
        }
    }
}