using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for game objects which will be instantiated by the occurence of agent events.
    /// </summary>
    public abstract class FxInstantiatedByAgentEvent
    {
        /// <summary>
        /// The game object that will be instantiated by the agent event.
        /// </summary>
        [Tooltip("The game object that will be instantiated by the agent event.")]
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
        /// The position the instantiated object will have right after instantiation.
        /// </summary>
        [Tooltip("The position the instantiated object will have right after instantiation.")]
        [SerializeField]
        protected StartPositionOption startPositionOption;

        /// <summary>
        /// The position offset of the instantiated object.<br></br>
        /// This will be used only if the start position option is by agent.
        /// </summary>
        [Space]
        [Header("By Agent Settings")]
        [Tooltip("The position offset of the instantiated object. This will be used only if the " +
            "start position option is by agent.")]
        [SerializeField]
        protected Vector3 agentPositionOffset;

        /// <summary>
        /// Determines on the X axis where the instantiated object will be located in relation to the play field.<br></br>
        /// This will be used only if the start position option is by play field.
        /// </summary>
        [Space]
        [Header("By Play Field Settings")]
        [Tooltip("Determines on the X axis where the instantiated object will be located in relation to the play " +
            "field. This will be used only if the start position option is by play field.")]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        protected float spawnXPositionRatio = 0.5f;

        /// <summary>
        /// Determines on the Y axis where the instantiated object will be located in relation to the play field.<br></br>
        /// This will be used only if the start position option is by play field.
        /// </summary>
        [Tooltip("Determines on the Y axis where the instantiated object will be located in relation to the play " +
            "field. This will be used only if the start position option is by play field.")]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        protected float spawnYPositionRatio = 0.5f;

        /// <summary>
        /// Determines the Z depth (position) of the instantiated object.<br></br>
        /// This will be used only if the start position option is by play field.
        /// </summary>
        [Tooltip("Determines the Z depth (position) of the instantiated object. " +
            "This will be used only if the start position option is by play field.")]
        [SerializeField]
        [Range(0, ProjectConstants.DepthIndexLimit)]
        protected int depthIndex;

        /// <summary>
        /// The position of the instantiated object.<br></br>
        /// This will be used only if the start position option is by input.
        /// </summary>
        [Space]
        [Header("By Input Settings")]
        [Tooltip("The position of the instantiated object. This will be used only if the " +
            "start position option is by input.")]
        [SerializeField]
        protected Vector3 inputPosition;

        /// <summary>
        /// The agent event that will cause the instantiation of the FX, this is set by the player or enemy and 
        /// is translated from player or enemy events into agent events.
        /// </summary>
        protected abstract AgentEvent agentEvent
        {
            get;
        }

        /// <summary>
        /// The agent that will trigger the avent that will instantiate the game object.
        /// </summary>
        protected AgentCollidable agent;

        public void Initialize(AgentCollidable agentCollidable)
        {

            agent = agentCollidable;
            agentCollidable.Subscribe(CreateClone, agentEvent);
        }

        /// <summary>
        /// Creates the instantiated object after the agent event had occured.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void CreateClone(System.EventArgs args)
        {
            GameObject go = GameObject.Instantiate(fxGo);

            switch (startPositionOption)
            {
                case StartPositionOption.ByAgent:
                    {
                        go.transform.position = agent.transform.position + agentPositionOffset;
                        break;
                    }

                case StartPositionOption.ByPlayField:
                    {
                        if(PlayField.Instance == null)
                        {
                            go.transform.position = agent.transform.position;
                            return;
                        }

                        float distanceFromXMin = PlayField.Instance.Boundries.width * spawnXPositionRatio;
                        float distanceFromYMin = PlayField.Instance.Boundries.height * spawnYPositionRatio;

                        float xPosition = PlayField.Instance.Boundries.xMin + distanceFromXMin;
                        float yPosition = PlayField.Instance.Boundries.yMin + distanceFromYMin;
                        float zPosition = Dimensions.FindZPositionInPlayField(depthIndex);

                        go.transform.position = new Vector3(xPosition, yPosition, zPosition);
                        break;
                    }

                case StartPositionOption.ByInput:
                    {
                        go.transform.position = inputPosition;
                        break;
                    }
            }

        }
    }
}