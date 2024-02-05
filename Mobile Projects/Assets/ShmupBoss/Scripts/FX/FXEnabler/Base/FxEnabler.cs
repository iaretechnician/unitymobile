using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for the components that enable a game object FX when an agent event occurs.
    /// </summary>
    public abstract class FxEnabler : MonoBehaviour
    {
        protected abstract FxEnabledByAgentEvent[] fxs
        {
            get;
        }

        /// <summary>
        /// The agent that will enable the FX when a selected event occurs.
        /// </summary>
        protected AgentCollidable agent;

        protected virtual void Awake()
        {
            agent = GetComponent<AgentCollidable>();

            for (int i = 0; i < fxs.Length; i++)
            {
                fxs[i].Initialize(this, agent);
            }
        }

        protected virtual void OnDisable()
        {
            StopAllCoroutines();
            DisableAllFXs();
        }

        protected virtual void DisableAllFXs()
        {
            foreach (FxEnabledByAgentEvent fx in fxs)
            {
                fx.Decomission();
            }
        }
    }
}