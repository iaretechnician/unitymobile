using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Audio clip with volume controls (VolumetricAC) which is played when an agent event is triggered.
    /// </summary>
    [System.Serializable]
    public abstract class volumetricACTriggeredByAgentEvent
    {
        /// <summary>
        /// The audio clip with volume controls which will be played when a selected agent event occurs.
        /// </summary>
        [Tooltip("The audio clip with volume controls which will be played when a selected agent event occurs.")]
        [SerializeField]
        private VolumetricAC volumetricAC;

        /// <summary>
        /// The agent event that will trigger the clip to play. Typically translated from a player or enemy event.
        /// </summary>
        protected abstract AgentEvent AgentEventTrigger
        {
            get;
        }

        public virtual void Initialize(AgentCollidable agent)
        {
            if (volumetricAC.AC == null)
            {
                Debug.Log("volumetricACTriggeredByAgentEvent.cs: One of your volume audio clips which is " +
                    "triggered by a player or an enemy named: " + agent.name + " is missing its clip.");

                return;
            }

            agent.Subscribe(PlayClip, AgentEventTrigger);
        }

        public virtual void PlayClip(System.EventArgs args)
        {
            SfxSource.Instance.PlayClip(volumetricAC.AC, volumetricAC.Volume);
        }
    }
}