using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Tools for drawing gizmos in the editor such as circles or rects.
    /// </summary>
    public static class GizmosExtension
    {
        #region Rect 
        /// <summary>
        /// Draws a wire cube with a depth of 0 (A rect) given rect dimensions.
        /// </summary>
        /// <param name="rect">The rect dimensions.</param>
        /// <param name="color">The color of the drawn rect.</param>
        public static void DrawRect(Rect rect, Color color)
        {
            Gizmos.color = color;
            Vector3 gizmoSize = new Vector3(rect.width, rect.height, 0.0f);
            Gizmos.DrawWireCube(rect.center, gizmoSize);
        }

        /// <summary>
        /// Draws a wire cube with a depth of 0 (A rect) given a vector2.
        /// </summary>
        /// <param name="vector2">The size of the rect you wish to draw.</param>
        /// <param name="color">The color of the drawn rect.</param>
        public static void DrawRect(Vector2 vector2, Color color)
        {
            Gizmos.color = color;
            Vector3 gizmoSize = new Vector3(vector2.x, vector2.y, 0.0f);
            Gizmos.DrawWireCube(Vector3.zero, gizmoSize);
        }

        /// <summary>
        /// Draws a wire cube with a depth of 0 (A rect) given vector2 dimensions and a center.
        /// </summary>
        /// <param name="vector2">The size of the rect you wish to draw.</param>
        /// <param name="center">The center of the rect you wish to draw.</param>
        /// <param name="color">The color of the drawn rect.</param>
        public static void DrawRect(Vector2 vector2, Vector2 center, Color color)
        {
            Gizmos.color = color;
            Vector3 gizmoSize = new Vector3(vector2.x, vector2.y, 0.0f);
            Vector3 gizmoCenter = new Vector3(center.x, center.y, 0.0f);
            Gizmos.DrawWireCube(gizmoCenter, gizmoSize);
        }
        #endregion

        #region Circle 
        /// <summary>
        /// Draws a circle with a given radius in the XY plane at Z = 0.
        /// </summary>
        /// <param name="center">Center of the circle in world space.</param>
        /// <param name="radius">Radius of the circle in world units.</param>
        public static void DrawCircle(Vector2 center, float radius)
        {
            DrawCircle(center, radius, 0, 3f);
        }

        /// <summary>
        /// Draws a circle with a given radius in the XY plane at Z = depth.
        /// </summary>
        /// <param name="center">Center of the circle in world space.</param>
        /// <param name="radius">Radius of the circle in world units.</param>
        /// <param name="depth">Circle Position on the Z Axis.</param>
        public static void DrawCircle(Vector2 center, float radius, float depth)
        {
            DrawCircle(center, radius, depth, 3f);
        }

        /// <summary>
        /// Draws a circle with a given radius in the XY plane at Z = depth.
        /// </summary>
        /// <param name="center">Center of the circle in world space.</param>
        /// <param name="radius">Radius of the circle in world unit.</param>
        /// <param name="depth">Circle Position on the Z Axis.</param>
        /// <param name="deltaAngle">Circle gets more accurate the lower the deltaAngle is.</param>
        public static void DrawCircle(Vector2 center, float radius, float depth, float deltaAngle)
        {

            int SegmentNum = Mathf.CeilToInt(360f / deltaAngle);

            float RadialDegree = deltaAngle * Mathf.Deg2Rad;

            Vector3 Center = new Vector3(center.x, center.y, 0);

            for (int i = 0; i < SegmentNum; i++)
            {
                Vector3 From;
                Vector3 To;
                if (i == SegmentNum - 1)
                {
                    From = new Vector3(
                        Mathf.Cos(RadialDegree * i) * radius,
                        Mathf.Sin(RadialDegree * i) * radius,
                        depth) + Center;
                    To = new Vector3(
                        radius,
                        0,
                        depth) + Center;
                }
                else
                {
                    From = new Vector3(
                        Mathf.Cos(RadialDegree * i) * radius,
                        Mathf.Sin(RadialDegree * i) * radius,
                        depth) + Center;
                    To = new Vector3(
                        Mathf.Cos(RadialDegree * (i + 1)) * radius,
                        Mathf.Sin(RadialDegree * (i + 1)) * radius,
                        depth) + Center;
                }
                Gizmos.DrawLine(From, To);
            }
        }
        #endregion
    }
}