using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Game objects which will be instantiated by the occurence of an enemy event.
    /// </summary>
    [System.Serializable]
    public class FxInstantiatedByEnemyEvent : FxInstantiatedByAgentEvent
    {
        /// <summary>
        /// The enemy event that will cause the instantiation of the game object.
        /// </summary>
        [Tooltip("The enemy event that will cause the instantiation of the game object.")]
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