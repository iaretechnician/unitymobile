using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Will swap the sprite image of a sprite renderer based on movement in four directions.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Movers/Components/Sprite Swapper By Four Direction")]
    public class SpriteSwapperByFourDirection : SpriteSwapperByDirection
    {
        /// <summary>
        /// The sprite that will be active when the mover is moving upward.
        /// </summary>
        [Tooltip("The sprite that will be active when the mover is moving upward.")]
        [SerializeField]
        protected FlippableSprite up;

        /// <summary>
        /// The sprite that will be active when the mover is moving downward.
        /// </summary>
        [Tooltip("The sprite that will be active when the mover is moving downward.")]
        [SerializeField]
        protected FlippableSprite down;

        /// <summary>
        /// The sprite that will be active when the mover is moving rightward.
        /// </summary>
        [Tooltip("The sprite that will be active when the mover is moving rightward.")]
        [SerializeField]
        protected FlippableSprite right;

        /// <summary>
        /// The sprite that will be active when the mover is moving leftward.
        /// </summary>
        [Tooltip("The sprite that will be active when the mover is moving leftward.")]
        [SerializeField]
        protected FlippableSprite left;

        /// <summary>
        /// The sprite that will be active when the mover is not moving.
        /// </summary>
        [Tooltip("The sprite that will be active when the mover is not moving")]
        protected FlippableSprite idle;

        protected override void CacheIdleSprite()
        {
            base.CacheIdleSprite();
            idle = new FlippableSprite(idleSprite);
        }

        protected virtual void Update()
        {
            if (!canSwapSprite)
            {
                return;
            }

            SwapSpriteByMoverDirection();
        }

        protected virtual void SwapSpriteByMoverDirection()
        {
            switch (mover.CurrentFourDirection)
            {
                case FourDirection.None:
                    {
                        SwapFlippableSprite(idle);
                        break;
                    }

                case FourDirection.Up:
                    {
                        SwapFlippableSprite(up);
                        break;
                    }

                case FourDirection.Down:
                    {
                        SwapFlippableSprite(down);
                        break;
                    }

                case FourDirection.Right:
                    {
                        SwapFlippableSprite(right);
                        break;
                    }

                case FourDirection.Left:
                    {
                        SwapFlippableSprite(left);
                        break;
                    }
            }
        }

        /// <summary>
        /// Swaps the sprite image with another.<br></br>
        /// A flippable sprite is just a sprite with an option to be flipped.
        /// </summary>
        /// <param name="flippableSprite">The new sprite image you wish to replace the old one with.</param>
        protected void SwapFlippableSprite(FlippableSprite flippableSprite)
        {
            if (spriteRenderer == null)
            {
                return;
            }

            if (flippableSprite.SpriteImage == null)
            {
                Debug.Log("SpriteSwapperByFourDirection.cs: One of your direction sprite swap components " +
                    "is missing a sprite.");

                return;
            }

            AssignFlippableSprite(flippableSprite);
        }

        /// <summary>
        /// Assigns the flippable sprite into the sprite image of the sprite renderer.
        /// </summary>
        /// <param name="flippableSprite"></param>
        protected void AssignFlippableSprite(FlippableSprite flippableSprite)
        {
            spriteRenderer.flipX = flippableSprite.IsFlippedHorizontally;
            spriteRenderer.flipY = flippableSprite.IsFlippedVertically;
            spriteRenderer.sprite = flippableSprite.SpriteImage;
        }
    }
}