using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for the component that produces a colored flash effect on the agent material or sprite.
    /// </summary>
    public abstract class FlashFX : MonoBehaviour
    {      
        /// <summary>
        /// Renderers which materials will change when an event triggers them to.
        /// </summary>
        [Tooltip("Renderers which materials will change when an event triggers them to.")]
        [SerializeField]
        protected Renderer[] targetRenderers;

        /// <summary>
        /// The agent that will trigger the material color change when a selected event occurs.
        /// </summary>
        protected AgentCollidable agent;

        private void Awake()
        {
            agent = GetComponent<AgentCollidable>();

            if (agent == null)
            {
                Debug.Log("FlashFX.cs: " + name + "Can't find the agent that controls a FlashFX.");

                return;
            }

            // If you have forgotten to assign a target renderer, this will attempt to get the renderer 
            // component of the agent.
            if (targetRenderers == null || targetRenderers.Length <= 0)
            {
                targetRenderers = new Renderer[0];
                targetRenderers[0] = GetComponent<Renderer>();
            }

            if (targetRenderers[0] == null)
            {
                Debug.Log("FlashFX.cs: Can't find a renderer to change its color for agent named: " + agent.name);

                return;
            }

            InitializeFlashEffects();
        }

        private void OnDisable()
        {
            StopAllCoroutines();           
            ResetRenderersColor();
        }

        /// <summary>
        /// Assigns the flash effect to the target renderers and the agents/agent events that control them.
        /// </summary>
        protected abstract void InitializeFlashEffects();

        private void ResetRenderersColor()
        {
            foreach (Renderer targetRenderer in targetRenderers)
            {
                if(targetRenderer == null)
                {
                    continue;
                }

                // Checks if the renderer is a sprite renderer, if not it changes the material color.
                
                SpriteRenderer spriteRenderer = targetRenderer as SpriteRenderer;

                if (spriteRenderer != null)
                {
                    spriteRenderer.color = Color.white;
                }
                else
                {
                    targetRenderer.material.color = Color.white;
                }
            }
        }
    }
}