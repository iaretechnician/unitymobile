using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for game objects (effects) that will be spawned by the occurence of agent events.
    /// </summary>
    public abstract class FxSpawnableByAgentEvent
    {
        /// <summary>
        /// The game object that will be spawned from the FX pool when an agent event occurs.
        /// </summary>
        [Tooltip("The game object that will be spawned from the FX pool when an agent event occurs." + "\n\n" + 
           "This game object must have an FX Eliminator on it so that it can be despawned.")]
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
        /// Hides this effect in the scene when the preview button is pressed.<br></br>
        /// Note: do not add an "#if UNITY_EDITOR" to this variable and associated property, or else 
        /// you would get an error when building the game.
        /// </summary>
        [Tooltip("Hides this effect in the scene when the preview button is pressed.")]
        [SerializeField]
        protected bool hideFromPreview;
        public bool HideFromPreview
        {
            get
            {
                return hideFromPreview;
            }
        }

        /// <summary>
        /// Changes the scale of the spawned FX.
        /// </summary>
        [Tooltip("Changes the scale of the spawned FX.")]
        [SerializeField]
        protected Vector3 modifyScale = Vector3.one;
        public Vector3 ModifyScale
        {
            get
            {
                return modifyScale;
            }
        }

        /// <summary>
        /// This will define how the position of the spawned object is found.
        /// </summary>
        [Tooltip("This will define how the position of the spawned object is found.")]
        [SerializeField]
        protected StartPositionOption startPositionOption;
        public StartPositionOption _StartPositionOption
        {
            get
            {
                return startPositionOption;
            }
        }


        /// <summary>
        /// The position offset of the spawned object.<br></br>
        /// This will be used only if the start position option is by agent.
        /// </summary>
        [Space]
        [Header("By Agent Settings")]
        [Tooltip("The position offset of the spawned object. This will be used only if the " +
            "start position option is by agent.")]
        [SerializeField]
        protected Vector3 agentPositionOffset;
        public Vector3 AgentPositionOffset
        {
            get
            {
                return agentPositionOffset;
            }
        }

        /// <summary>
        /// Determines on the X axis where the spawned object will be located in relation to the play field.<br></br>
        /// This will be used only if the start position option is by play field.
        /// </summary>
        [Space]
        [Header("By Play Field Settings")]
        [Tooltip("Determines on the X axis where the spawned object will be located in relation to the play " +
            "field. This will be used only if the start position option is by play field.")]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        protected float spawnXPositionRatio = 0.5f;
        public float SpawnXPositionRatio
        {
            get
            {
                return spawnXPositionRatio;
            }
        }

        /// <summary>
        /// Determines on the Y axis where the spawned object will be located in relation to the play field.<br></br>
        /// This will be used only if the start position option is by play field.
        /// </summary>
        [Tooltip("Determines on the Y axis where the spawned object will be located in relation to the play " +
            "field. This will be used only if the start position option is by play field.")]
        [SerializeField]
        [Range(0.0f, 1.0f)]
        protected float spawnYPositionRatio = 0.5f;
        public float SpawnYPositionRatio
        {
            get
            {
                return spawnYPositionRatio;
            }
        }

        /// <summary>
        /// Determines the Z depth (position) of the spawned object.<br></br>
        /// This will be used only if the start position option is by play field.
        /// </summary>
        [Tooltip("Determines the Z depth (position) of the spawned object. " +
            "This will be used only if the start position option is by play field.")]
        [SerializeField]
        [Range(0, ProjectConstants.DepthIndexLimit)]
        protected int depthIndex;
        public int DepthIndex
        {
            get
            {
                return depthIndex;
            }
        }

        /// <summary>
        /// The position of the spawned object.<br></br>
        /// This will be used only if the start position option is by input.
        /// </summary>
        [Space]
        [Header("By Input Settings")]
        [Tooltip("The position of the spawned object. This will be used only if the " +
            "start position option is by input.")]
        [SerializeField]
        protected Vector3 inputPosition;
        public Vector3 InputPosition
        {
            get
            {
                return inputPosition;
            }
        }

        /// <summary>
        /// The resultant scale afer taking consideration the original scale and the modified values.
        /// </summary>
        private Vector3 modifiedScale;

        /// <summary>
        /// The agent event that will cause the spawn of the FX, this is set by the player or enemy and 
        /// is translated from player or enemy events into agent events.
        /// </summary>
        protected abstract AgentEvent agentEvent
        {
            get;
        }

        /// <summary>
        /// The agent that will trigger the avent that will spawn the game object.
        /// </summary>
        protected AgentCollidable agent;

        /// <summary>
        /// Injects the agent whose events will spawn the game object (FX), finds the modified scale and 
        /// subscribes the spawn to the agent.
        /// </summary>
        /// <param name="agentCollidable">The agent whose event will control spawning the effect.</param>
        public void Initialize(AgentCollidable agentCollidable)
        {
            agent = agentCollidable;
            
            Vector3 originalScale = FxGo.transform.localScale;
            modifiedScale = Vector3.Scale(originalScale, ModifyScale);

            agentCollidable.Subscribe(Spawn, agentEvent);
        }

        protected virtual void Spawn(System.EventArgs args)
        {
            if (FXPool.Instance != null)
            {
                GameObject go = FXPool.Instance.Spawn(FxGo.name);
                go.transform.localScale = modifiedScale;
                go.transform.rotation = Quaternion.identity;

                switch (startPositionOption)
                {
                    case StartPositionOption.ByAgent:
                        {
                            go.transform.position = agent.transform.position + agentPositionOffset;
                            break;
                        }

                    case StartPositionOption.ByPlayField:
                        {
                            if (PlayField.Instance == null)
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

        public void SetModifyScaleToDefault()
        {
            modifyScale = Vector3.one;
        }
    }
}