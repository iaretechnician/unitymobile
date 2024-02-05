using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Spawns effects from the FX pool when an enemy event occurs.
    /// </summary>
    [AddComponentMenu("Shmup Boss/FX/Enemy/FX Spawner Enemy")]
    [RequireComponent(typeof(Enemy))]
    public class FxSpawnerEnemy : FxSpawner
    {
        /// <summary>
        /// Array of game objects (effects) which will be spawned when an enemy event occurs.
        /// </summary>
        [Tooltip("Array of game objects (effects) which will be spawned when an enemy event occurs.")]
        [SerializeField]
        private FxSpawnableByEnemyEvent[] fxs;
        public override FxSpawnableByAgentEvent[] FXs
        {
            get
            {
                return fxs;
            }
        }
    }
}