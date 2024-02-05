using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Will move the object along a bezier curve path that you have created.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Movers/Curve Mover")]
    public class CurveMover : MoverRotatableOnZ
    {
        /// <summary>
        /// The bezier curve path that the mover will use.
        /// </summary>
        [HideInInspector]
        public Path CurvePath;

        private float distanceTraveled;

        public float ZPos
        {
            get;
            set;
        }

#if UNITY_EDITOR
        [SB_HelpBox("To add a point to the curve, hold Shift and press the first mouse button inside the scene view.")]
        [Tooltip("How big will the control points appear to be in the scene editor.")]
        public float HandleSize = 0.5f;
#endif

        protected override void Awake()
        {
            base.Awake();

            if (CurvePath != null)
            {
                CurvePath.PrepareCurvePoints();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            distanceTraveled = 0.0f;
        }

        protected override void FindCurrentDirection()
        {
            CurrentDirection = CurvePath.FindTangent(distanceTraveled);
        }

        protected override void Move()
        {
            FindDistanceTraveled();

            DisplacementInLastFrame = CurrentDirection * CurrentSpeed * Time.deltaTime;

            Vector2 positionOnCurve = CurvePath.FindPosition(distanceTraveled);
            transform.position = new Vector3(positionOnCurve.x, positionOnCurve.y, ZPos);
        }

        private void FindDistanceTraveled()
        {
            distanceTraveled += CurrentSpeed * Time.deltaTime;

            if (distanceTraveled > CurvePath.CurvePoints[CurvePath.CurvePoints.Count - 1].Dist)
            {
                distanceTraveled = 0.0f;
                EnemyPool.Instance.Despawn(gameObject);
            }
        }
    }
}