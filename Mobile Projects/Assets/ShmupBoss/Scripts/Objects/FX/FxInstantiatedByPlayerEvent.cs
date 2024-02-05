using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Game objects which will be instantiated by the occurence of a player event.
    /// </summary>
    [System.Serializable]
    public class FxInstantiatedByPlayerEvent : FxInstantiatedByAgentEvent
    {
        /// <summary>
        /// The player event that will cause the instantiation of the game object.
        /// </summary>
        [Tooltip("The player event that will cause the instantiation of the game object.")]
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