using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Default directions for agents. In a vertical level, a player is typically
    /// facing Up while enemies down. In a horizontal Level a Player is typically facing
    /// right while enemies left.
    /// </summary>
    public static class FacingDirections
    {
        /// <summary>
        /// The facing direction of the player, in a vertical level it's up, in a horizontal level it's right.
        /// </summary>
        public static Vector2 Player
        {
            get
            {
                if (Level.IsVertical)
                {
                    return Vector3.up;
                }
                else if (Level.IsHorizontal)
                {
                    return Vector3.right;
                }

                return Vector3.zero;
            }
        }

        /// <summary>
        /// The facing direction enemies, in a vertical level it's down, in a horizontal level it's left.
        /// </summary>
        public static Vector2 NonPlayer
        {
            get
            {
                if (Level.IsVertical)
                {
                    return Vector3.down;
                }
                else if (Level.IsHorizontal)
                {
                    return Vector3.left;
                }

                return Vector3.zero;
            }
        }
    }
}