using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Enables a game object FX when a player event occurs.
    /// </summary>
    [RequireComponent(typeof(Player))]
    public class FxEnablerPlayer : FxEnabler
    {
        /// <summary>
        /// Array of game objects, the events that will enable them and if they will be disabled after time.
        /// </summary>
        [Tooltip("Array of game objects, the events that will enable them and if they will be disabled after time.")]
        [SerializeField]
        private FxEnabledByPlayerEvent[] fxsPlayer;
        protected override FxEnabledByAgentEvent[] fxs
        {
            get
            {
                return fxsPlayer;
            }
        }
    }
}