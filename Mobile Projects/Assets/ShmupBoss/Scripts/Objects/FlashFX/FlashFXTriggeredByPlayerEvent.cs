using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// flash effects that are triggered by a player event for a set time.
    /// </summary>
    [System.Serializable]
    public class FlashFXTriggeredByPlayerEvent : FlashFXTriggeredByAgentEvent
    {
        /// <summary>
        /// The player event that will trigger the flash color effect.
        /// </summary>
        [Tooltip("The player event that will trigger the flash color effect.")]
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