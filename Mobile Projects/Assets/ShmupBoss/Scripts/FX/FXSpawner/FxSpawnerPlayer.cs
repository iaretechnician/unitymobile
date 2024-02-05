using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Spawns effects from the FX pool when a player event occurs.
    /// </summary>
    [AddComponentMenu("Shmup Boss/FX/Player/FX Spawner Player")]
    [RequireComponent(typeof(Player))]
    public class FxSpawnerPlayer : FxSpawner
    {
        /// <summary>
        /// Array of game objects (effects) which will be spawned when a player event occurs.
        /// </summary>
        [Tooltip("Array of game objects (effects) which will be spawned when a player event occurs.")]
        [SerializeField]
        private FxSpawnableByPlayerEvent[] fxs;
        public override FxSpawnableByAgentEvent[] FXs
        {
            get
            {
                return fxs;
            }
        }
    }
}