using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Game objects which will be enabled by the occurence of enemy events.
    /// </summary>
    [System.Serializable]
    public class FxEnabledByEnemyEvent : FxEnabledByAgentEvent
    {
        /// <summary>
        /// The enemy event that will enable the game object.
        /// </summary>
        [Tooltip("The enemy event that will enable the game object.")]
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