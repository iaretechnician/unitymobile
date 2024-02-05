using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Produces a colored flash effect on the enemy material or sprite when events occur.
    /// </summary>
    [AddComponentMenu("Shmup Boss/FX/Enemy/Flash FX Enemy")]
    [RequireComponent(typeof(Enemy))]
    public class FlashFXEnemy : FlashFX
    {
        /// <summary>
        /// List of flash effects, the time they are active and the events that trigger them.
        /// </summary>
        [SerializeField]
        private FlashFXTriggeredByEnemyEvent[] flashEffects;

        protected override void InitializeFlashEffects()
        {
            for (int i = 0; i < flashEffects.Length; i++)
            {
                flashEffects[i].Initialize(this, targetRenderers, agent);
            }
        }
    }
}