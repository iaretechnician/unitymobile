using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Spawns an FX (Game object) when a weapon fires.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Components/Weapon Fire FX")]
    public class WeaponFireFX : ArmamentFX
    {
        /// <summary>
        /// Will add a constrained point modifier to the spawned FX and link it to the weapon so that it 
        /// appears to be connected to the weapon and not stay in place after being spawned.
        /// </summary>
        [Tooltip("Will add a constrained point modifier to the spawned FX and link it to the weapon so that it " +
            "appears to be connected to the weapon and not stay in place after being spawned.")]
        [SerializeField]
        private bool isFXConstrainedToWeapon;

        /// <summary>
        /// The weapon which when fired will reesult in spawning an FX.
        /// </summary>
        private Weapon weapon;

        protected override void CacheArmamentAndBindMethods()
        {
            weapon = GetComponent<Weapon>();

            if (weapon == null)
            {
                Debug.Log("WeaponFireFX.cs: weapon fire fx needs to be added " +
                    "to a weapon, please make sure that you have a weapon component.");

                return;
            }

            weapon.OnFire += Spawn;
        }

        protected override void Spawn(System.EventArgs args)
        {
            if (FXPool.Instance == null)
            {
                return;
            }

            GameObject weaponFireFxGO = FXPool.Instance.Spawn(FxGo.name);

            if (weaponFireFxGO == null)
            {
                Debug.Log("WeaponFireFX.cs: unable to spawn weapon fire fx go from FX pool.");

                return;
            }

            weaponFireFxGO.transform.position = transform.position + offsetPosition;
            weaponFireFxGO.transform.localRotation = Quaternion.Euler(newRotation);
            weaponFireFxGO.transform.localScale = modifiedScale;

            CheckForFXPositionConstraint(weaponFireFxGO);
        }

        /// <summary>
        /// A constrained FX game object which will move with the spawner and not simply remain static after it has 
        /// been spawned, this is achieved by a ContrainedPoint script which links the object to its spawner transform.
        /// </summary>
        /// <param name="weaponFireFxGO">The FX game object which has been spawned.</param>
        private void CheckForFXPositionConstraint(GameObject weaponFireFxGO)
        {
            if (!isFXConstrainedToWeapon)
            {
                return;
            }

            ConstrainedPoint constrainedPoint = weaponFireFxGO.GetComponent<ConstrainedPoint>();

            if (constrainedPoint == null)
            {
                constrainedPoint = weaponFireFxGO.AddComponent<ConstrainedPoint>();
            }

            constrainedPoint.DisplacementFromTarget = offsetPosition;
            constrainedPoint.Target = transform;
        }
    }
}

