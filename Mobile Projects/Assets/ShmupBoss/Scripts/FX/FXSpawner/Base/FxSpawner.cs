using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for spawning effects from the FX pool when an agent event occurs.
    /// </summary>
    public abstract class FxSpawner : MonoBehaviour
    {
        /// <summary>
        /// The agent whose event will trigger spawning the effects.
        /// </summary>
        protected AgentCollidable agentCollidable;

        /// <summary>
        /// The effects which will be spawned when selected events occur.
        /// </summary>
        public abstract FxSpawnableByAgentEvent[] FXs
        {
            get;
        }

        /// <summary>
        /// The hierarchy that will be used to nest any preview items.<br></br>
        /// This has both attributes of serialize and hide in inspector so that the reference to it 
        /// is not lost when you play the scene (serialize/deserialize)
        /// </summary>
        [HideInInspector]
        [SerializeField]
        protected Transform previewFXsHierarchy;

        protected virtual void OnValidate()
        {
            if(FXs == null)
            {
                return;
            }
            
            if(FXs.Length == 0)
            {
                return;
            }

            for (int i = 0; i < FXs.Length; i++)
            {
                if(FXs[i].ModifyScale == Vector3.zero)
                {
                    FXs[i].SetModifyScaleToDefault();
                }
            }
        }

        protected virtual void Awake()
        {
            DestroyPreviewFXs();

            agentCollidable = GetComponent<AgentCollidable>();

            if(agentCollidable == null)
            {
                Debug.Log("FxSpawner.cs: " + name + " can't find an agent to reference, an FxSpawner " +
                    "component needs to be added to an agent.");

                return;
            }

            foreach (FxSpawnableByAgentEvent fx in FXs)
            {
                fx.Initialize(agentCollidable);
            }
        }

        protected void DestroyPreviewFXs()
        {
            if (previewFXsHierarchy != null)
            {
                Destroy(previewFXsHierarchy.gameObject);
            }
        }

#if UNITY_EDITOR
        public virtual void CreatePreviewFXsInEditor()
        {
            previewFXsHierarchy = new GameObject("----- Preview FXs -----").transform;
            previewFXsHierarchy.gameObject.AddComponent<Disposable>();

            for (int i = 0; i < FXs.Length; i++)
            {
                if (FXs[i].HideFromPreview)
                {
                    continue;
                }

                if (FXs[i].FxGo == null)
                {
                    Debug.Log("The fx game object you are trying to preview is empty, " +
                        "please double check that your FX game object is referenced.");

                    continue;
                }

                GameObject previewFX = Instantiate(FXs[i].FxGo, transform.position, Quaternion.identity, previewFXsHierarchy);

                Vector3 originalScale = FXs[i].FxGo.transform.localScale;
                previewFX.transform.localScale = Vector3.Scale(originalScale, FXs[i].ModifyScale);

                switch (FXs[i]._StartPositionOption)
                {
                    case StartPositionOption.ByAgent:
                        {
                            previewFX.transform.position += FXs[i].AgentPositionOffset;
                            break;
                        }

                    case StartPositionOption.ByPlayField:
                        {
                            if (PlayField.Instance == null)
                            {
                                Debug.Log("FXSpawner.cs: unable yet to position the " +
                                    "spawnable FX inside the game field because no isntance of the play field" +
                                    " has been saved. please play the level at least once then try again.");

                                return;
                            }

                            float distanceFromXMin = PlayField.Instance.Boundries.width * FXs[i].SpawnXPositionRatio;
                            float distanceFromYMin = PlayField.Instance.Boundries.height * FXs[i].SpawnYPositionRatio;

                            float xPosition = PlayField.Instance.Boundries.xMin + distanceFromXMin;
                            float yPosition = PlayField.Instance.Boundries.yMin + distanceFromYMin;
                            float zPosition = Dimensions.FindZPositionInPlayField(FXs[i].DepthIndex);

                            previewFX.transform.position = new Vector3(xPosition, yPosition, zPosition);
                            break;
                        }

                    case StartPositionOption.ByInput:
                        {
                            previewFX.transform.position = FXs[i].InputPosition;
                            break;
                        }
                }
            }
        }

        public void DestroyPreviewFXsInEditor()
        {
            if (previewFXsHierarchy != null)
            {
                DestroyImmediate(previewFXsHierarchy.gameObject);
            }
        }
#endif
    }
}