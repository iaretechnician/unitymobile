using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Audio clip with volume controls (VolumetricAC) which is played when an enemy event is triggered.
    /// </summary>
    [System.Serializable]
    public class volumetricACTriggeredByEnemyEvent : volumetricACTriggeredByAgentEvent
    {
        /// <summary>
        /// The enemy event that will trigger the audio clip to be played.
        /// </summary>
        [Tooltip("The enemy event that will trigger the audio clip to be played.")]
        [SerializeField]
        private EnemyEvent enemyEventTrigger;

        protected override AgentEvent AgentEventTrigger
        {
            get
            {
                return AgentEventTranslator.FromEnemyEvent(enemyEventTrigger);
            }
        }
    }
}