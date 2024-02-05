using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for the components that instantiate a game object FX when an agent event occurs.
    /// </summary>
    public abstract class FxInstantiator : MonoBehaviour
    {
        protected abstract FxInstantiatedByAgentEvent[] fxs
        {
            get;
        }

        /// <summary>
        /// The agent that will instantiate the FX when a selected event occurs.
        /// </summary>
        private AgentCollidable agent;

        protected void Awake()
        {
            agent = GetComponent<AgentCollidable>();

            for (int i = 0; i < fxs.Length; i++)
            {
                fxs[i].Initialize(agent);
            }
        }
    }
}