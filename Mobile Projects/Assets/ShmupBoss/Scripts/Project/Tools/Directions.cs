using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// A collection of methods for converting between direction enumerators, vectors and angles.
    /// </summary>
    public static class Directions
    {
        #region TwoDirections Methods
        /// <summary>
        /// Converts a 2D vector into either the up or down directions.
        /// </summary>
        /// <param name="direction">The Vector2 you wish to convert.</param>
        /// <returns>The vertical direction enumeration.</returns>
        public static VerticalDirection VectorToVerticalDirection(Vector2 direction)
        {
            if (direction.y == 0.0f)
            {
                return VerticalDirection.None;
            }

            if (direction.y > 0.0f)
            {
                return VerticalDirection.Up;
            }
            else
            {
                return VerticalDirection.Down;
            }
        }

        /// <summary>
        /// Converts a 2D vector into either the right or left directions.
        /// </summary>
        /// <param name="direction">The Vector2 you wish to convert.</param>
        /// <returns>The horizontal direction enumeration.</returns>
        public static HorizontalDirection VectorToHorizontalDirection(Vector2 direction)
        {
            if (direction.x == 0.0f)
            {
                return HorizontalDirection.None;
            }

            if (direction.x > 0.0f)
            {
                return HorizontalDirection.Right;
            }
            else
            {
                return HorizontalDirection.Left;
            }
        }
        #endregion

        #region FourDirection Methods
        /// <summary>
        /// Converts a 2D vector into either the up, right, down or left directions.
        /// </summary>
        /// <param name="direction">The Vector2 you wish to convert.</param>
        /// <returns>One of the four directions enumeration.</returns>
        public static FourDirection VectorToFourDirection(Vector2 direction)
        {
            if (direction == Vector2.zero)
            {
                return FourDirection.None;
            }

            float angle = Math2D.VectorToDegree(direction);

            if (angle >= 45f && angle < 135f)
            {
                return FourDirection.Up;
            }

            if (angle >= 135f && angle < 225f)
            {
                return FourDirection.Left;
            }

            if (angle >= 225f && angle < 315f)
            {
                return FourDirection.Down;
            }

            return FourDirection.Right;
        }

        /// <summary>
        /// Converts a four direction enumeration into a vector2.
        /// </summary>
        /// <param name="direction">The four direction enumeration you wish to convert.</param>
        /// <returns>The converted Vector2 direction.</returns>
        public static Vector2 FourDirectionToVector2(FourDirection direction)
        {
            switch (direction)
            {
                case FourDirection.Up:
                    {
                        return Vector2.up;
                    }

                case FourDirection.Down:
                    {
                        return Vector2.down;
                    }

                case FourDirection.Left:
                    {
                        return Vector2.left;
                    }

                case FourDirection.Right:
                    {
                        return Vector2.right;
                    }

                default:
                    {
                        return Vector2.zero;
                    }
            }
        }

        /// <summary>
        /// Converts a four direction enumeration into an angle in degrees.
        /// </summary>
        /// <param name="direction">The four direction enumeration you wish to convert.</param>
        /// <returns>The converted angle in degrees.</returns>
        public static float FourDirectionToDegrees(FourDirection direction)
        {
            switch (direction)
            {

                case FourDirection.Down:
                    return 270;

                case FourDirection.Up:
                    return 90;

                case FourDirection.Left:
                    return 180;

                case FourDirection.Right:
                    return 0;

            }

            return 0;
        }
        #endregion

        #region Eight Direction Methods
        /// <summary>
        /// Converts a 2D vector into an eight direction enumeration.
        /// </summary>
        /// <param name="direction">The vector2 that you wish to convert.</param>
        /// <returns>One of the eight directions enumerations.</returns>
        public static EightDirection VectorToEightDirection(Vector2 direction)
        {
            if (direction == Vector2.zero)
            {
                return EightDirection.None;
            }

            float angle = Math2D.VectorToDegree(direction);

            if (angle >= 22.5f && angle < 67.5f)
            {
                return EightDirection.UpRight;
            }

            if (angle >= 67.5f && angle < 112.5f)
            {
                return EightDirection.Up;
            }

            if (angle >= 112.5f && angle < 157.5f)
            {
                return EightDirection.UpLeft;
            }

            if (angle >= 157.5f && angle < 202.5f)
            {
                return EightDirection.Left;
            }

            if (angle >= 202.5f && angle < 247.5f)
            {
                return EightDirection.DownLeft;
            }

            if (angle >= 247.5f && angle < 292.5f)
            {
                return EightDirection.Down;
            }

            if (angle >= 292.5f && angle < 337.5f)
            {
                return EightDirection.DownRight;
            }

            return EightDirection.Right;
        }

        /// <summary>
        /// Converts an eight direction enumeration into a vector2.
        /// </summary>
        /// <param name="direction">The eight direction enumeration that you wish to convert.</param>
        /// <returns>The converted vector2.</returns>
        public static Vector2 EightDirectionToVector2(EightDirection direction)
        {
            switch (direction)
            {
                case EightDirection.Up:
                    {
                        return Vector2.up;
                    }

                case EightDirection.Down:
                    {
                        return Vector2.down;
                    }

                case EightDirection.Left:
                    {
                        return Vector2.left;
                    }

                case EightDirection.Right:
                    {
                        return Vector2.right;
                    }

                case EightDirection.UpRight:
                    {
                        return new Vector2(0.5f, 0.5f);
                    }

                case EightDirection.DownRight:
                    {
                        return new Vector2(0.5f, -0.5f);
                    }

                case EightDirection.DownLeft:
                    {
                        return new Vector2(-0.5f, -0.5f);
                    }

                case EightDirection.UpLeft:
                    {
                        return new Vector2(-0.5f, 0.5f);
                    }

                default:
                    {
                        return Vector2.zero;
                    }
            }
        }

        /// <summary>
        /// Converts an eight direction enumeration into an angle in degrees.
        /// </summary>
        /// <param name="direction">The direction that you wish to convert.</param>
        /// <returns>The converted angle in degrees.</returns>
        public static float EightDirectionToDegrees(EightDirection direction)
        {
            switch (direction)
            {
                case EightDirection.Down:
                    {
                        return 270;
                    }

                case EightDirection.Up:
                    {
                        return 90;
                    }

                case EightDirection.Left:
                    {
                        return 180;
                    }

                case EightDirection.Right:
                    {
                        return 0;
                    }

                case EightDirection.UpRight:
                    {
                        return 45;
                    }

                case EightDirection.UpLeft:
                    {
                        return 135;
                    }


                case EightDirection.DownLeft:
                    {
                        return 225;
                    }


                case EightDirection.DownRight:
                    {
                        return 315;
                    }
            }

            return 0;
        }
        #endregion
    }
}