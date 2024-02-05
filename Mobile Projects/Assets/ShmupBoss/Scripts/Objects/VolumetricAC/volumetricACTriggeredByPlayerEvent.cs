using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Audio clip with volume controls (VolumetricAC) which is played when a player event is triggered.
    /// </summary>
    [System.Serializable]
    public class volumetricACTriggeredByPlayerEvent : volumetricACTriggeredByAgentEvent
    {
        /// <summary>
        /// The player event that will trigger the audio clip to be played.
        /// </summary>
        [Tooltip("The player event that will trigger the audio clip to be played.")]
        [SerializeField]
        private PlayerEvent playerEventTrigger;

        protected override AgentEvent AgentEventTrigger
        {
            get
            {
                return AgentEventTranslator.FromPlayerEvent(playerEventTrigger);
            }
        }
    }
}