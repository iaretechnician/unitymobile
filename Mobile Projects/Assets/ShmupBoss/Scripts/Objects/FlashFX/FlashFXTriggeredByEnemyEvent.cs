using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// flash effects that are triggered by an enemy event for a set time.
    /// </summary>
    [System.Serializable]
    public class FlashFXTriggeredByEnemyEvent : FlashFXTriggeredByAgentEvent
    {
        /// <summary>
        /// The enemy event that will trigger the flash color effect.
        /// </summary>
        [Tooltip("The enemy event that will trigger the flash color effect.")]
        [SerializeField]
        private EnemyEvent enemyEvent;
        protected override AgentEvent agentEvent
        {
            get
            {
                return AgentEventTranslator.FromEnemyEvent(enemyEvent);
            }
        }
    }
}