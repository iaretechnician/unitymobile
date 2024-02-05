using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Produces a colored flash effect on the player material or sprite when events occur.
    /// </summary>
    [AddComponentMenu("Shmup Boss/FX/Player/Flash FX Player")]
    [RequireComponent(typeof(Player))]
    public class FlashFXPlayer : FlashFX
    {
        /// <summary>
        /// List of flash effects, the time they are active and the events that trigger them.
        /// </summary>
        [SerializeField]
        private FlashFXTriggeredByPlayerEvent[] flashEffects;

        protected override void InitializeFlashEffects()
        {
            for (int i = 0; i < flashEffects.Length; i++)
            {
                flashEffects[i].Initialize(this, targetRenderers, agent);
            }
        }
    }
}