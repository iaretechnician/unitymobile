using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// This is only used as part of a boss and must be nested under a boss hierarchy.<br></br>
    /// This class adds the option of showing a destroyed version and most importantly will be deactivated 
    /// instead of despawned from the enemy pool, since the boss is the one that will be spawned/despawned 
    /// from the pool and not its sub phases.<br></br>
    /// It also has the important function of notifying the boss when it's eliminated.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Agents/Boss Sub Phase")]
    public class BossSubPhase : Enemy
    {
        /// <summary>
        /// The boss this sub phase is related to, this boss will activate/deactivate this sub phase.
        /// </summary>
        [Tooltip("The boss this sub phase is related to, this boss will activate/deactivate this sub phase.")]
        [Header("Boss Sub Phase Settings")]
        [SerializeField]
        private Boss boss;
        
        /// <summary>
        /// This game object will be set active when this sub phase is eliminated to be a representation
        /// of a destroyed version of this sub phase.
        /// </summary>
        [Tooltip("This game object will be set active when this sub phase is eliminated to be a " +
            "representation of a destroyed version of this sub phase.")]
        [SerializeField]
        private GameObject destroyedVersion;
        public GameObject DestroyedVersion
        {
            get
            {
                return destroyedVersion;
            }
        }

        /// <summary>
        /// If this is true, it will cause the boss to completely despawn when this sub phase is out of the
        /// despawning field.
        /// </summary>
        [Tooltip("If this is true, it will cause the boss to completely despawn when this sub phase is out of " +
            "the despawning field.")]
        [SerializeField]
        private bool isDespawningBossOnExit;

        /// <summary>
        /// This phase index is used in knowing which phase it belongs to when this sub phase is eliminated, 
        /// and this will assist in knowing the current phase index of the boss and if the boss needs 
        /// to be eliminated.
        /// </summary>
        public int BossPhaseIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Deactivates the sub phase, replaces any particles weapons, shows the destroyed version and very importantly 
        /// it also notifies the boss of it's elimination which will lead to the boss elimination if all conditions have been met.
        /// </summary>
        protected override void Despawn()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (boss == null)
            {
                Debug.Log("BossSubPhase.cs: boss sub phase named: " + name + 
                    " is missing its boss reference. did you forget to assign a boss to it.");

                return;
            }
            
            boss.NotifyOfSubPhaseElimination(new BossSubPhaseArgs(BossPhaseIndex));
            
            if (isUsingParticleWeapons)
            {
                ReplaceParticleWeapons();
            }

            if (DestroyedVersion != null)
            {
                DestroyedVersion.gameObject.SetActive(true);
            }

            gameObject.SetActive(false);

            boss.EliminateIfConditionsAreMet();
        }

        /// <summary>
        /// This is to cause the elimination of the boss when this sub phase exits the despawning field.
        /// </summary>
        /// <param name="collision">The boss sub phase collider.</param>
        protected override void OnTriggerExit2D(Collider2D collision)
        {
            if (!isDespawningBossOnExit)
            {
                return;
            }
            
            if (collision.gameObject.layer == ProjectLayers.DespawningField)
            {
                if (!gameObject.activeSelf)
                {
                    return;
                }

                if (boss == null)
                {
                    Debug.Log("BossSubPhase.cs: boss sub phase named: " + name +
                        " is missing its boss reference. did you forget to assign a boss to it.");

                    return;
                }

                boss.EliminateRegardless();
            }
        }
    }
}