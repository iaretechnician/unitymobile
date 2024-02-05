using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Contains tools for finding various dimensions such as width, height and position or 
    /// translating from one unit to another.
    /// </summary>
    public static class Dimensions
    {
        /// <summary>
        /// Finds the width of a game object which contains either a sprite renderer or a mesh renderer.
        /// </summary>
        /// <param name="go">The game object with a sprite renderer or a mesh renderer which you wish to find 
        /// its width.</param>
        /// <returns>The width value.</returns>
        public static float FindWidth(GameObject go)
        {
            if (go.GetComponent<SpriteRenderer>() != null)
            {
                SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
                return spriteRenderer.bounds.size.x;
            }
            else if (go.GetComponent<MeshRenderer>() != null)
            {
                MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                return meshRenderer.bounds.size.x;
            }
            else
            {
                return CannotFindSize(go);
            }
        }

        /// <summary>
        /// Finds the height of a game object which contains either a sprite renderer or a mesh renderer.
        /// </summary>
        /// <param name="go">The game object with a sprite renderer or a mesh renderer which you wish to find 
        /// its height.</param>
        /// <returns>The height value.</returns>
        public static float FindHeight(GameObject go)
        {
            if (go.GetComponent<SpriteRenderer>() != null)
            {
                SpriteRenderer spriteRenderer = go.GetComponent<SpriteRenderer>();
                return spriteRenderer.bounds.size.y;
            }
            else if (go.GetComponent<MeshRenderer>() != null)
            {
                MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
                return meshRenderer.bounds.size.y;
            }
            else
            {
                return CannotFindSize(go);
            }
        }

        private static float CannotFindSize(GameObject go)
        {
            Debug.Log("Dimensions.cs: game object named: " + go.name + " which you are trying to find " +
                "its size is neither a mesh nor a sprite. The FindWidth/Height method cannot function.");

            return 0.0f;
        }

        /// <summary>
        /// Converts a vector2 into a rect using the dimensions of the vector.
        /// </summary>
        /// <param name="size">The size of the rect you wish to create.</param>
        /// <returns>The rect created using the vector dimensions.</returns>
        public static Rect Vector2ToRect(Vector2 size)
        {
            Vector2 rectBottomLeftCorner = new Vector2((size.x * -0.5f), (size.y * -0.5f));
            Rect rect = new Rect(rectBottomLeftCorner, size);

            return rect;
        }

        public static float FindZPositionInPlayField(int depthIndex)
        {
            if (Level.Is2D)
            {
                return (-depthIndex * Level.SpaceBetweenIndices);
            }

            return 0.0f;
        }

        /// <summary>
        /// Translates the vertical aspect ratio enum into a float value so that it can be used
        /// by the camera and any other componenets.
        /// </summary>
        /// <param name="aspect">The vertical aspect ratio enum.</param>
        /// <returns>The translated float value of that enum.</returns>
        public static float VerticalAspectRatioToFloat(VerticalAspectRatio aspect, float inputVerticalAspectRatio)
        {
            switch (aspect)
            {
                case VerticalAspectRatio.Aspect3By4:
                    return 3.0f / 4.0f;

                case VerticalAspectRatio.Aspect4By5:
                    return 4.0f / 5.0f;

                case VerticalAspectRatio.Aspect9By16:
                    return 9.0f / 16.0f;

                case VerticalAspectRatio.Aspect10By16:
                    return 10.0f / 16.0f;

                case VerticalAspectRatio.Input:
                    return inputVerticalAspectRatio;

                default:
                    return 1.0f;
            }
        }
    }
}