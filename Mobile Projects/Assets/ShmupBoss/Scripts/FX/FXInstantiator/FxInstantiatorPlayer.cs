using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Game objects which will be instantiated by the occurence of player events.
    /// </summary>
    [RequireComponent(typeof(Player))]
    public class FxInstantiatorPlayer : FxInstantiator
    {
        /// <summary>
        /// Array of game objects and the events that will instantiate them.
        /// </summary>
        [Tooltip("Array of game objects and the events that will instantiate them.")]
        [SerializeField]
        private FxInstantiatedByPlayerEvent[] fxsPlayer;
        protected override FxInstantiatedByAgentEvent[] fxs
        {
            get
            {
                return fxsPlayer;
            }
        }
    }
}