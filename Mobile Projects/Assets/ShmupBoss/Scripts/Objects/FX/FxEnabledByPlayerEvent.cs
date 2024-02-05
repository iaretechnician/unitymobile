using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Game object swhich will be enabled by the occurence of player events.
    /// </summary>
    [System.Serializable]
    public class FxEnabledByPlayerEvent : FxEnabledByAgentEvent
    {
        /// <summary>
        /// The player event that will enable the game object.
        /// </summary>
        [Tooltip("The player event that will enable the game object.")]
        [SerializeField]
        private PlayerEvent playerEvent;
        protected override AgentEvent agentEvent
        {
            get
            {
                return AgentEventTranslator.FromPlayerEvent(playerEvent);
            }
        }
    }
}