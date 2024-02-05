using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Spawns an FX when a munition hits a target.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Munition/Munition Hit FX")]
    [RequireComponent(typeof(Munition))]
    public class MunitionHitFX : ArmamentFX
    {
        /// <summary>
        /// The sound effect played when a munition hits a layer it collides with.
        /// </summary>
        [Tooltip("The sound effect played when a munition hits a layer it collides with.")]
        [SerializeField]
        protected VolumetricAC hitSFX;

        /// <summary>
        /// The munition which when it hits its target it will spawn an FX.
        /// </summary>
        private Munition munition;

        protected override void CacheArmamentAndBindMethods()
        {
            munition = GetComponent<Munition>();

            if (munition == null)
            {
                Debug.Log("MunitionHitFX.cs: munition hit fx needs to be added " +
                    "to a munition, please make sure that you have a munition component.");

                return;
            }

            munition.OnHit += Spawn;

            if (hitSFX.AC != null)
            {
                munition.OnHit += PlaySFX;
            }
        }

        protected override void Spawn(System.EventArgs args)
        {
            if(FXPool.Instance == null)
            {
                return;
            }

            GameObject weaponFireFxGO = FXPool.Instance.Spawn(FxGo.name);

            if (weaponFireFxGO == null)
            {
                Debug.Log("MunitionHitFX.cs: munition named: " + name + "  was " +
                    "unable to spawn weapon fire fx go from weapon FX pool.");

                return;
            }

            weaponFireFxGO.transform.position = transform.position + offsetPosition;
            weaponFireFxGO.transform.localRotation = Quaternion.Euler(newRotation);
            weaponFireFxGO.transform.localScale = modifiedScale;
        }

        private void PlaySFX(System.EventArgs args)
        {
            SfxSource.Instance.PlayClip(hitSFX.AC, hitSFX.Volume);
        }
    }
}