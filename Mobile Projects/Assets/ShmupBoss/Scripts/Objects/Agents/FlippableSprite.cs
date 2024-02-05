using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// A sprite image that have the option to be flipped horizontally or vertically.
    /// </summary>
    [System.Serializable]
    public struct FlippableSprite
    {
        [SerializeField]
        private Sprite sprite;
        public Sprite SpriteImage
        {
            get
            {
                return sprite;
            }
        }

        [Tooltip("The sprite will be flipped vertically if this is checked.")]
        [SerializeField]
        private bool isFlippedVertically;
        public bool IsFlippedVertically
        {
            get
            {
                return isFlippedVertically;
            }
        }

        [Tooltip("The sprite will be flipped horizontally if this is checked.")]
        [SerializeField]
        private bool isFlippedHorizontally;
        public bool IsFlippedHorizontally
        {
            get
            {
                return isFlippedHorizontally;
            }
        }

        public FlippableSprite(Sprite newSprite, bool flipVertically = false, bool flipHorizontally = false)
        {
            sprite = newSprite;
            isFlippedVertically = flipVertically;
            isFlippedHorizontally = flipHorizontally;
        }
    }

}
