using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Enables a game object FX when an enemy event occurs.
    /// </summary>
    [RequireComponent(typeof(Enemy))]
    public class FxEnablerEnemy : FxEnabler
    {
        /// <summary>
        /// Array of game objects, the events that will enable them and if they will be disabled after time.
        /// </summary>
        [Tooltip("Array of game objects, the events that will enable them and if they will be disabled after time.")]
        [SerializeField]
        private FxEnabledByEnemyEvent[] fxsEnemy;
        protected override FxEnabledByAgentEvent[] fxs
        {
            get
            {
                return fxsEnemy;
            }
        }
    }
}