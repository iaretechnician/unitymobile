using System.Collections;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for the bullets and missiles. Munition is pooled by the munition pool and are spawned by 
    /// munition weapons.
    /// </summary>
    public abstract class Munition : MonoBehaviour
    {
        public event CoreDelegate OnHit;

        /// <summary>
        /// How many instances of this munition are pooled.
        /// </summary>
        [Tooltip("How many instances of this munition are pooled, try to estimate the maximum number of " +
            "munition of this type that will be on screen at the exact same time.")]
        [SerializeField]
        protected int poolLimit;
        public int PoolLimit
        {
            get
            {
                return poolLimit;
            }
        }

        /// <summary>
        /// If true, this would make the munition rotation not influenced by any rotation of the 
        /// weapon or agent firing it.
        /// </summary>
        [Tooltip("If checked, this would make the munition rotation not influenced by any rotation of " +
            "the weapon or agent firing it.")]
        [SerializeField]
        protected bool isIndependentOfWeaponRotation;

        [Tooltip("The damage caused when this munition hits an opposing faction.")]
        [SerializeField]
        protected float damage;
        public float Damage
        {
            get
            {
                if(gameObject.layer == ProjectLayers.PlayerMunition)
                {
                    return damage * CurrentMultiplier.PlayerMunitionDamageMultiplier;
                }
                else
                {
                    return damage * CurrentMultiplier.EnemiesMunitionDamageMultiplier;
                }
                               
            }
        }

        /// <summary>
        /// For how long will this munition stay active after having been fired.
        /// </summary>
        [Tooltip("For how long will this munition stay active after having been fired." + "\n" +
            "This is particularly important in the case of missiles, which might not go out of the despawning " +
            "field to be despawned, and might instead wander around for very long times in the play field " +
            "unless despawned through a life time." + "\n\n" + "A value of zero means this munition has no lifetime " +
            "and will live infinitely, or as long as the game lasts!")]
        [SerializeField]
        protected float lifetime;

        protected virtual void OnValidate()
        {
            if(poolLimit <= 0)
            {
                poolLimit = 10;
            }
        }

        /// <summary>
        /// Starts the disable after time coroutine.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (lifetime > 0.0f)
            {
                StartCoroutine(DisableAfterTime());
            }
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Prepares the munition initial position, direction and rotation.
        /// </summary>
        /// <param name="weaponPosition">Position which will be used as an initial position for the munition.</param>
        /// <param name="firingRotation">Typically the weapon rotation, or the rotation you want 
        /// to initialze the bullet with.</param>
        /// <param name="emitDirection">An additional direction is needed to add effects like radial firing.</param>
        public abstract void Initialize(Vector3 weaponPosition, Quaternion firingRotation, Vector2 emitDirection);

        private IEnumerator DisableAfterTime()
        {
            yield return new WaitForSeconds(lifetime);

            Despawn();
        }

        protected void Despawn()
        {
            // This is to initiate any methods added to the OnHit event such as a hit FX explosion when 
            // the munition is disabled after time.
            if (Level.IsFinishedLoading)
            {
                RaiseOnHit();
            }

            if (gameObject.layer == ProjectLayers.EnemyMunition)
            {
                if(MunitionPoolEnemy.Instance == null)
                {
                    Debug.Log("Munition.cs: trying to despawn a munition using its lifetime" +
                        " but couldn't find the munition pool enemy instance for it.");

                    gameObject.SetActive(false);
                    return;
                }

                MunitionPoolEnemy.Instance.Despawn(gameObject);
            }
            else if (gameObject.layer == ProjectLayers.PlayerMunition)
            {
                if (MunitionPoolPlayer.Instance == null)
                {
                    Debug.Log("Munition.cs: trying to despawn a munition using its lifetime" +
                        " but couldn't find the munition pool player instance for it.");

                    gameObject.SetActive(false);
                    return;
                }

                MunitionPoolPlayer.Instance.Despawn(gameObject);
            }
            else
            {
                Debug.Log("Munition.cs: A munition somehow does not have the right layer it should have " +
                    ", have you forgotten a bullet or a missile active by mistake in the scene?");

                gameObject.SetActive(false);
                return;
            }
        }

        public void NotifyOfOnHit()
        {
            if (Level.IsFinishedLoading)
            {
                RaiseOnHit();
            }
        }

        protected void RaiseOnHit()
        {
            OnHit?.Invoke(null);
        }
    }
}