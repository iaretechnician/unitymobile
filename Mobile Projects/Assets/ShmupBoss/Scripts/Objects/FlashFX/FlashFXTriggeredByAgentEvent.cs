using System.Collections;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for the flash effects that are triggered by an agent event for a set time.
    /// </summary>
    public abstract class FlashFXTriggeredByAgentEvent
    {
        /// <summary>
        /// The color the flash effect will change the material/sprite to. Pay careful attention to the alpha value.
        /// </summary>
        [Tooltip("The color the flash effect will change the material/sprite to. " +
            "Pay careful attention to the alpha value.")]
        [SerializeField]
        protected Color color;

        /// <summary>
        /// For how long this flash will last.
        /// </summary>
        [Tooltip("For how long this flash will last.")]
        [SerializeField]
        protected float flashTime;

        /// <summary>
        /// The agent event that will trigger the flash, this is set by the player or enemy and 
        /// is translated from player or enemy events into agent events.
        /// </summary>
        protected abstract AgentEvent agentEvent
        {
            get;
        }

        /// <summary>
        /// The instance of the object with the flash effect script used on (FlashFXPlayer/FlashFXEnemy), 
        /// this is used to start the flash effect coroutine.<br></br>
        /// (This class is not a MonoBehaviour and cannot start coroutines.)
        /// </summary>
        protected FlashFX flashFXInstance;

        /// <summary>
        /// The renderers which materials will change when an event triggers them to. This array is injected
        /// by the initialize method.
        /// </summary>
        protected Renderer[] targetRenderers;

        /// <summary>
        /// True if the effect is currently flashing.
        /// </summary>
        protected bool isFlashing;

        /// <summary>
        /// Injects (saves) the flash fx instance that will play the effect and the renderers that will change color 
        /// when the effect has been started and subscribes the start flash method to the agent and the event that 
        /// triggers it.
        /// </summary>
        /// <param name="flashFX">The instance which will play the effect coroutine.</param>
        /// <param name="renderers">Array of renderers that will change color.</param>
        /// <param name="agent">The agent (player/enemy) whose events will trigger the color change.</param>
        public void Initialize(FlashFX flashFX, Renderer[] renderers, AgentCollidable agent)
        {
            flashFXInstance = flashFX;
            targetRenderers = renderers;

            if (flashFXInstance == null)
            {
                return;
            }

            if (targetRenderers == null)
            {
                return;
            }

            agent.Subscribe(StartFlash, agentEvent);
        }

        private void StartFlash(System.EventArgs args)
        {
            if (flashFXInstance == null)
            {
                return;
            }

            // If the instance is already flashing, this will stop the existing flash so 
            // that a new flash and a new timer can start. This also means that only one flash
            // of any kind is allowed at once. Having multiple flashes at the same time operating
            // would overly complicate the script (I would have to find a color average, etc..) and it 
            // would be quite a rare occasion when you would need two flashes operating at the same time.
            if (isFlashing)
            {
                StopFlash();
            }

            if (flashFXInstance.gameObject.activeSelf)
            {
                flashFXInstance.StartCoroutine(Flash());
            }
        }

        public void StopFlash()
        {
            flashFXInstance.StopAllCoroutines();
        }

        /// <summary>
        /// Changes the renderer color in accordance to the flash effect color, which increases linearly 
        /// from no color effect to full color effect at the end of the flash time.
        /// </summary>
        /// <returns></returns>
        private IEnumerator Flash()
        {
            if(targetRenderers == null)
            {
                yield return null;
            }

            float currentTime = Time.time;
            float flashStartTime = currentTime;
            float flashEndTime = flashStartTime + flashTime;

            isFlashing = true;

            while (flashEndTime >= currentTime)
            {
                SetRendererColor(SampleFlashColor((currentTime - flashStartTime) / flashTime));

                yield return null;

                currentTime = Time.time;
            }

            SetRendererColor(Color.white);
            isFlashing = false;
        }

        /// <summary>
        /// Returns white color if the percentage is zero (which will not change any renderers and have no effect), 
        /// and the full color if the percentage is 1 (100%) (which will multiply with the material/sprite color 
        /// and change it) and of course any linear values in between.
        /// </summary>
        /// <param name="percentage">The percentage of the current flash effect color.</param>
        /// <returns>The sampled flash color that will affect the renderer</returns>
        private Color SampleFlashColor(float percentage)
        {
            float t = 1.0f - percentage;
            
            return Color.Lerp(new Color(1, 1, 1, 1), color, t);
        }

        private void SetRendererColor(Color color)
        {
            foreach(Renderer targetRenderer in targetRenderers)
            {
                SpriteRenderer spriteRenderer = targetRenderer as SpriteRenderer;

                if (spriteRenderer != null)
                {
                    spriteRenderer.color = color;
                }
                else
                {
                    targetRenderer.material.color = color;
                }
            }                
        }
    }
}