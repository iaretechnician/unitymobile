using System.Collections;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for a game object which will be enabled by the occurence of an agent event.
    /// </summary>
    public abstract class FxEnabledByAgentEvent
    {
        /// <summary>
        /// The game object that will be enabled by the agent event.
        /// </summary>
        [Tooltip("The game object that will be enabled by the agent event.")]
        [SerializeField]
        protected GameObject fxGo;
        public GameObject FxGo
        {
            get
            {
                return fxGo;
            }
        }

        /// <summary>
        /// The agent event that will cause the enabling of the FX, this is set by the player or enemy and 
        /// is translated from player or enemy events into agent events.
        /// </summary>
        protected abstract AgentEvent agentEvent
        {
            get;
        }

        /// <summary>
        /// If value is bigger than zero, this will disable the enabled game object after the set time.
        /// </summary>
        [Tooltip("If value is bigger than zero, this will disable the enabled game object after the set time.")]
        [SerializeField]
        protected float disableAfterTime;

        /// <summary>
        /// The agent that will trigger the avent that will enable the game object.
        /// </summary>
        protected AgentCollidable agent;

        /// <summary>
        /// The instance of the componenet of the FxEnabler (Player/enemy) that will start the 
        /// coroutine of disabling the enabled game object after time.<br></br>
        /// This class is not MonoBehaviour and cannot start a coroutine, 
        /// this is why the fx enabler instance is needed.
        /// </summary>
        protected FxEnabler fxEnablerInstance;

        /// <summary>
        /// Injects (saves) the fx enabler instance that will be responsible for starting the coroutines 
        /// and the agent that will raise the events which will trigger enabling the game object.
        /// </summary>
        /// <param name="fxEnabler">The instance that will be start the coroutine for disabling the 
        /// game object after time</param>
        /// <param name="agentCollidable">The agent that will raise the events that will trigger 
        /// enabling the game object.</param>
        public void Initialize(FxEnabler fxEnabler, AgentCollidable agentCollidable)
        {
            fxEnablerInstance = fxEnabler;

            agent = agentCollidable;
            agentCollidable.Subscribe(Enable, agentEvent);
        }


        /// <summary>
        /// Enables the game object (FX) and if disable after time is needed, will start the coroutine 
        /// for disabling it as well.
        /// </summary>
        /// <param name="args">Empty system argument, this is only needed because the agent 
        /// subscribe method requires it.</param>
        protected virtual void Enable(System.EventArgs args)
        {
            if (!fxGo.activeSelf)
            {
                fxGo.SetActive(true);

                if (disableAfterTime > 0.0f)
                {
                    fxEnablerInstance.StartCoroutine(DisableAfterTime());
                }
            }
        }

        public void Decomission()
        {
            fxGo.SetActive(false);
        }

        protected virtual IEnumerator DisableAfterTime()
        {
            yield return new WaitForSeconds(disableAfterTime);

            fxGo.SetActive(false);

            yield return null;
        }
    }
}