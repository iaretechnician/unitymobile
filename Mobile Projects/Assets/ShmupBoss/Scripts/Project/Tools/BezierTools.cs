using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Tools for evaluating a bezier curve (path).<br></br>
    /// This type of curve is currently used with the curve mover.
    /// </summary>
    public static class BezierTools
    {
        public static Vector2 EvaluateCubic(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
        {
            Vector2 p0 = EvaluateQuadratic(a, b, c, t);
            Vector2 p1 = EvaluateQuadratic(b, c, d, t);

            return Vector2.Lerp(p0, p1, t);
        }

        public static Vector2 EvaluateQuadratic(Vector2 a, Vector2 b, Vector2 c, float t)
        {
            Vector2 p0 = Vector2.Lerp(a, b, t);
            Vector2 p1 = Vector2.Lerp(b, c, t);

            return Vector2.Lerp(p0, p1, t);
        }
    }
}