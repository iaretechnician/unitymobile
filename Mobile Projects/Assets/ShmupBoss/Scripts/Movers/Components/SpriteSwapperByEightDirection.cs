using UnityEngine;

namespace ShmupBoss 
{
    /// <summary>
    /// Will swap the sprite image of a sprite renderer based on movement in eight directions.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Movers/Components/Sprite Swapper By Eight Direction")]
    public class SpriteSwapperByEightDirection : SpriteSwapperByFourDirection
    {
        /// <summary>
        /// The sprite that will be active when the mover is moving towards the upper right corner.
        /// </summary>
        [Tooltip("The sprite that will be active when the mover is moving towards the upper right corner.")]
        [SerializeField]
        protected FlippableSprite UpRight;

        /// <summary>
        /// The sprite that will be active when the mover is moving towards the upper left corner.
        /// </summary>
        [Tooltip("The sprite that will be active when the mover is moving towards the upper left corner.")]
        [SerializeField]
        protected FlippableSprite UpLeft;

        /// <summary>
        /// The sprite that will be active when the mover is moving towards the lower right corner.
        /// </summary>
        [Tooltip("The sprite that will be active when the mover is moving towards the lower right corner.")]
        [SerializeField]
        protected FlippableSprite DownRight;

        /// <summary>
        /// The sprite that will be active when the mover is moving towards the lower left corner.
        /// </summary>
        [Tooltip("The sprite that will be active when the mover is moving towards the lower left corner.")]
        [SerializeField]
        protected FlippableSprite DownLeft;

        protected override void SwapSpriteByMoverDirection()
        {
            switch (mover.CurrentEightDirection)
            {
                case EightDirection.None:
                    {
                        SwapFlippableSprite(idle);
                        break;
                    }

                case EightDirection.Up:
                    {
                        SwapFlippableSprite(up);
                        break;
                    }

                case EightDirection.Down:
                    {
                        SwapFlippableSprite(down);
                        break;
                    }

                case EightDirection.Right:
                    {
                        SwapFlippableSprite(right);
                        break;
                    }

                case EightDirection.Left:
                    {
                        SwapFlippableSprite(left);
                        break;
                    }

                case EightDirection.UpRight:
                    {
                        SwapFlippableSprite(UpRight);
                        break;
                    }

                case EightDirection.UpLeft:
                    {
                        SwapFlippableSprite(UpLeft);
                        break;
                    }

                case EightDirection.DownRight:
                    {
                        SwapFlippableSprite(DownRight);
                        break;
                    }

                case EightDirection.DownLeft:
                    {
                        SwapFlippableSprite(DownLeft);
                        break;
                    }
            }
        }
    }
}