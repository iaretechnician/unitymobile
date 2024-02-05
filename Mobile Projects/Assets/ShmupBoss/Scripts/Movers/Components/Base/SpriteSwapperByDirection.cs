using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for components which will change a sprite based on the direction of movement.
    /// </summary>
    public abstract class SpriteSwapperByDirection : MonoBehaviour
    {
        /// <summary>
        /// The mover whose direction will be used to swap the sprite.
        /// </summary>
        [Tooltip("The mover whose direction will be used to swap the sprite.")]
        [SerializeField]
        protected Mover mover;

        /// <summary>
        /// Sprite renderer which will be swapped by the images you input.
        /// </summary>
        [Tooltip("Sprite renderer which will be swapped by the images you input.")]
        [SerializeField]
        protected SpriteRenderer spriteRenderer;

        /// <summary>
        /// Sprite which will be shown when the mover is not moving.
        /// </summary>
        protected Sprite idleSprite;

        protected bool canSwapSprite;

        protected virtual void Awake()
        {
            if(mover == null)
            {
                mover = GetComponent<Mover>();
            }
            
            if (mover == null)
            {
                Debug.Log("SpriteSwapperByDirection.cs: Trying to find a mover but can't find any.");

                return;
            }

            if(spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (spriteRenderer == null)
            {
                Debug.Log("SpriteSwapperByDirection.cs: " + name + "Cannot find a sprite renderer.");

                return;
            }

            canSwapSprite = true;
            CacheIdleSprite();
        }

        /// <summary>
        /// Uses the sprite in the sprite renderer as an idle sprite.
        /// </summary>
        protected virtual void CacheIdleSprite()
        {
            if(spriteRenderer.sprite == null)
            {
                Debug.Log("SpriteSwapperByDirection.cs: " + name + " Unable to find the idle sprite of the " +
                    "sprite renderer. Make sure to add an actual sprite to your sprite renderer.");

                canSwapSprite = false;
                return;
            }

            idleSprite = spriteRenderer.sprite;
        }
    }
}