using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for components that play sound effects when agent events occur
    /// </summary>
    /// <typeparam name="T">The type of audio clips that are triggered by either a player or an enemy.</typeparam>
    public abstract class Sfx<T> : MonoBehaviour where T: volumetricACTriggeredByAgentEvent
    {
        /// <summary>
        /// The agent which will trigger the audio clip to be played when its events occur.
        /// </summary>
        protected AgentCollidable agent;

        /// <summary>
        /// The list of audio clips which are triggered by an agent event and have volume controls (volumetric AC).
        /// </summary>
        public List<T> SoundEffects = new List<T>();

        protected virtual void Awake()
        {
            agent = GetComponent<AgentCollidable>();

            if (agent == null)
            {
                PrintAgentNullMessage();
                return;
            }

            foreach (volumetricACTriggeredByAgentEvent clip in SoundEffects)
            {
                clip.Initialize(agent);
            }
        }

        protected abstract void PrintAgentNullMessage();
    }
}