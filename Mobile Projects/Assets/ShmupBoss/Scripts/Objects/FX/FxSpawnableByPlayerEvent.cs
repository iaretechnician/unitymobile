using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Game object (Effect) that will be spawned by the occurence of player events.
    /// </summary>
    [System.Serializable]
    public class FxSpawnableByPlayerEvent : FxSpawnableByAgentEvent
    {
        /// <summary>
        /// The player event which will cause the game object (effect) to be spwaned.
        /// </summary>
        [Space]
        [Header("Spawn Trigger")]
        [Tooltip("The player event which will cause the game object (effect) to be spwaned.")]
        [SerializeField]
        private PlayerEvent playerEventTrigger;
        protected override AgentEvent agentEvent
        {
            get
            {
                return AgentEventTranslator.FromPlayerEvent(playerEventTrigger);
            }
        }
    }
}