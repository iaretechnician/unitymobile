using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Game objects which will be instantiated by the occurence of enemy events.
    /// </summary>
    [RequireComponent(typeof(Enemy))]
    public class FxInstantiatorEnemy : FxInstantiator
    {
        /// <summary>
        /// Array of game objects and the events that will instantiate them.
        /// </summary>
        [Tooltip("Array of game objects and the events that will instantiate them.")]
        [SerializeField]
        private FxInstantiatedByEnemyEvent[] fxsEnemy;
        protected override FxInstantiatedByAgentEvent[] fxs
        {
            get
            {
                return fxsEnemy;
            }
        }
    }
}