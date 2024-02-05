using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// A 2D point with tangent information and distance along a bezier curve.<br></br>
    /// A bezier curve (path) is made of bezier curve points, these points differ from control points 
    /// that they always lie on the curve while control points on the other hand could represnt tangent 
    /// control points which do not lie on the curve.
    /// </summary>
    [System.Serializable]
    public class BezierCurvePoint
    {
        /// <summary>
        /// The 2D position of the vertex on the curve.
        /// </summary>
        public Vector2 Pos
        {
            get;
            set;
        }

        /// <summary>
        /// The curve tangent on the point this vertex is located on.
        /// </summary>
        public Vector2 Tangent
        {
            get;
            set;
        }

        /// <summary>
        /// The path distance from its start to the point this vertex is located on.
        /// </summary>
        public float Dist
        {
            get;
            set;
        }

        public BezierCurvePoint(Vector2 pos)
        {
            Pos = pos;
        }
    }
}