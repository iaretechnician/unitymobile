using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Game object (Effect) that will be spawned by the occurence of enemy events.
    /// </summary>
    [System.Serializable]
    public class FxSpawnableByEnemyEvent : FxSpawnableByAgentEvent
    {
        /// <summary>
        /// The enemy event which will cause the game object (effect) to be spwaned.
        /// </summary>
        [Space]
        [Header("Spawn Trigger")]
        [Tooltip("The enemy event which will cause the game object (effect) to be spwaned.")]
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