using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Flips through a series of sprites based on the agent direction.<br></br>
    /// This works only on one movement axis at a time, horizontal or vertical.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Movers/Components/Gradual Sprite Swapper")]
    public class GradualSpriteSwapper : SpriteSwapperByDirection
    {
        /// <summary>
        /// How much time it takes to flip from one sprite frame to the next.
        /// </summary>
        [Tooltip("How much time it takes to flip from one sprite frame to the next.")]
        [SerializeField]
        private float timeBetweenFrames;


        /// <summary>
        /// The frames that will be swapped through when movement is to the left in a vertical level.
        /// </summary>
        [Header("Use With Vertical Levels")]
        [Space]
        [Tooltip("The frames that will be swapped through when movement is to the left in a vertical level.")]
        [SerializeField]
        private Sprite[] leftFrames;

        /// <summary>
        /// The frames that will be swapped through when movement is to the right in a vertical level.
        /// </summary>
        [Tooltip("The frames that will be swapped through when movement is to the right in a vertical level.")]
        [SerializeField]
        private Sprite[] rightFrames;

        /// <summary>
        /// The frames that will be swapped through when movement is upward in a horizontal level.
        /// </summary>
        [Header("Use With Horizontal Levels")]
        [Space]
        [Tooltip("The frames that will be swapped through when movement is upward a horizontal level.")]
        [SerializeField]
        private Sprite[] upFrames;

        /// <summary>
        /// The frames that will be swapped through when movement is downward in a horizontal level.
        /// </summary>
        [Tooltip("The frames that will be swapped through when movement is downward a horizontal level.")]
        [SerializeField]
        private Sprite[] downFrames;

        private int currentFrameIndex;
        private float frameSwapCounter;
        private bool isMissingFrames;

        private bool IsMissingLeftOrRightFrames
        {
            get
            {
                if (IsMissingSprites(leftFrames) || IsMissingSprites(rightFrames))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool IsMissingUpOrDownFrames
        {
            get
            {
                if (IsMissingSprites(upFrames) || IsMissingSprites(downFrames))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            CheckIfMissingFrames();

            if (isMissingFrames)
            {
                Debug.Log("GradualSpriteSwapper.cs: Some of your sprites in a gradual sprite swapper " +
                    "component are messing.");

                canSwapSprite = false;
            }
        }

        private void Update()
		{
            if (!canSwapSprite)
            {
                return;
            }

            if (Time.time < frameSwapCounter)
            {
                return;
            }

            UpdateCurrentFrameIndex();
            SwapFrame();
            ResetFrameSwapTimer();
        }

        private void CheckIfMissingFrames()
        {
            if (Level.IsVertical)
            {
                isMissingFrames = IsMissingLeftOrRightFrames;
            }
            else if (Level.IsHorizontal)
            {
                isMissingFrames = IsMissingUpOrDownFrames;
            }                      
        }

        private void UpdateCurrentFrameIndex()
        {
            if (Level.IsVertical)
            {
                UpdateCurrentFrameIndexForVerticalLevel();
            }
            else if (Level.IsHorizontal)
            {
                UpdateCurrentFrameIndexForHorizontalLevel();
            }
        }

        private void UpdateCurrentFrameIndexForVerticalLevel()
        {
            if (mover.CurrentHorizontalDirection == HorizontalDirection.Right)
            {
                if (currentFrameIndex < rightFrames.Length)
                {
                    currentFrameIndex++;
                }
            }

            if (mover.CurrentHorizontalDirection == HorizontalDirection.Left)
            {
                if (currentFrameIndex > -leftFrames.Length)
                {
                    currentFrameIndex--;
                }
            }

            if (mover.CurrentHorizontalDirection == HorizontalDirection.None)
            {
                if (currentFrameIndex > 0)
                {
                    currentFrameIndex--;
                }

                if (currentFrameIndex < 0)
                {
                    currentFrameIndex++;
                }
            }
        }

        private void UpdateCurrentFrameIndexForHorizontalLevel()
        {
            if (mover.CurrentVerticalDirection == VerticalDirection.Up)
            {
                if (currentFrameIndex < upFrames.Length)
                {
                    currentFrameIndex++;
                }
            }

            if (mover.CurrentVerticalDirection == VerticalDirection.Down)
            {
                if (currentFrameIndex > -downFrames.Length)
                {
                    currentFrameIndex--;
                }
            }

            if (mover.CurrentVerticalDirection == VerticalDirection.None)
            {
                if (currentFrameIndex > 0)
                {
                    currentFrameIndex--;
                }

                if (currentFrameIndex < 0)
                {
                    currentFrameIndex++;
                }
            }
        }

        private void SwapFrame()
        {
            if (currentFrameIndex == 0)
            {
                spriteRenderer.sprite = idleSprite;
            }

            if (Level.IsVertical)
            {
                SwapVerticalLevelFrames();
            }

            if (Level.IsHorizontal)
            {
                SwapHorizontalLevelFrames();
            }
        }

        void SwapVerticalLevelFrames()
        {
            if (currentFrameIndex > 0)
            {
                spriteRenderer.sprite = rightFrames[currentFrameIndex - 1];
            }

            if (currentFrameIndex < 0)
            {
                spriteRenderer.sprite = leftFrames[-(currentFrameIndex + 1)];
            }
        }

        private void SwapHorizontalLevelFrames()
        {
            if (currentFrameIndex > 0)
            {
                spriteRenderer.sprite = upFrames[currentFrameIndex - 1];
            }

            if (currentFrameIndex < 0)
            {
                spriteRenderer.sprite = downFrames[-(currentFrameIndex + 1)];
            }
        }

        private void ResetFrameSwapTimer()
        {
            frameSwapCounter = Time.time + timeBetweenFrames;
        }

        private static bool IsMissingSprites(Sprite[] frames)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                if (frames[i] == null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}