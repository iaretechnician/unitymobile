using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Moves in straight lines towards pre-specified points with options to stop at the points or loop.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Movers/Waypoint Mover")]
    public class WaypointMover : Mover
    {
        /// <summary>
        /// How the mover behaves when it has reached the last point.
        /// </summary>
        [Tooltip("How the mover behaves when it has reached the last point.")]
        [SerializeField]
        private WaypointMoverMode mode;

        /// <summary>
        /// If in mode you have choosen continue loop or loop back and forth, the number you input here 
        /// will exclude the points before it from any looping.
        /// </summary>
        [Tooltip("If in mode you have choosen continue loop or loop back and forth, the number you input " +
            "here will exclude the points before it from any looping.")]
        [SerializeField]
        private int loopBackToPoint;

        /// <summary>
        /// The points which the mover will pass through and for how long will it stop at each one.
        /// </summary>
        [Tooltip("The points which the mover will pass through and for how long will it stop at each one.")]
        [SerializeField]
        private Waypoint2D[] waypoints;

        /// <summary>
        /// Array of the waypoints that this mover goes through.
        /// </summary>
        /// <param name="i">The index of the waypoint in the array of waypoint this mover goes through.</param>
        /// <returns>The indexed waypoint this mover goes through.</returns>
        public Waypoint2D this[int i]
        {
            get
            {
                return waypoints[i];
            }
            set
            {
                waypoints[i] = value;
            }
        }

        /// <summary>
        /// The Z position this mover will have and which is set by the finite/infinite spawner.
        /// </summary>
        public float ZPos
        {
            get;
            set;
        }

        private int currentWaypointsIndex;
        private float nextTimeToMove;

        private bool isMovingForward = true;

        private bool canUpdateIndexForward
        {
            get
            {
                if (currentWaypointsIndex + 1 < waypoints.Length)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool canUpdateIndexBackward
        {
            get
            {
                if (currentWaypointsIndex > loopBackToPoint)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private bool isWaiting
        {
            get
            {
                if (nextTimeToMove > Time.time)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

#if UNITY_EDITOR
        [Header("Gizmos")]
        [Tooltip("The size of the waypoints you see in the editor.")]
        public float HandleSize = 0.5f;
#endif

        private void OnEnable()
        {
            if (!Level.IsFinishedLoading)
            {
                return;
            }

            if (waypoints.Length < 1)
            {
                return;
            }

            isMovingForward = true;
            currentWaypointsIndex = 0;

            SetPositionToFirstPoint();
            SetNextTimeToMove(currentWaypointsIndex);

            currentWaypointsIndex++;
        }

        private void SetPositionToFirstPoint()
        {
            transform.position = Math2D.Vector2ToVector3(waypoints[0].Position, ZPos);
        }

        private void SetNextTimeToMove(int i)
        {
            nextTimeToMove = Time.time + waypoints[i].WaitTime;
        }

        protected override void FindCurrentDirection()
        {
            if(isWaiting)
            {
                CurrentDirection = Vector2.zero;
                return;
            }

            Vector3 displacementToNextPoint = (Vector2)(waypoints[currentWaypointsIndex].Position3D - transform.position);

            if (displacementToNextPoint.sqrMagnitude < ProjectConstants.ReacehdPositionSqrThreshold)
            {
                switch (mode)
                {
                    case WaypointMoverMode.Stop:
                        if (canUpdateIndexForward)
                        {
                            UpdateIndexForward();
                        }
                        else
                        {                            
                            CurrentDirection = Vector2.zero;
                            return;
                        }
                        break;

                    case WaypointMoverMode.ContinueLoop:
                        ContinueLoop();
                        break;

                    case WaypointMoverMode.LoopBackAndForth:
                        MoveBackAndForth();
                        break;
                }
            }

            CurrentDirection = displacementToNextPoint.normalized;
        }

        private void ContinueLoop()
        {
            if (canUpdateIndexForward)
            {
                UpdateIndexForward();
            }
            else
            {
                LoopBackToPoint();
            }
        }

        private void LoopBackToPoint()
        {
            SetNextTimeToMove(currentWaypointsIndex);
            currentWaypointsIndex = loopBackToPoint;
        }

        private void MoveBackAndForth()
        {
            if (isMovingForward)
            {
                if (canUpdateIndexForward)
                {
                    UpdateIndexForward();
                }
                else
                {
                    UpdateIndexBackward();
                    isMovingForward = false;
                }
            }
            else
            {
                if (canUpdateIndexBackward)
                {
                    UpdateIndexBackward();
                }
                else
                {
                    UpdateIndexForward();
                    isMovingForward = true;
                }
            }
        }

        private void UpdateIndexForward()
        {
            SetNextTimeToMove(currentWaypointsIndex);
            currentWaypointsIndex++;          
        }

        private void UpdateIndexBackward()
        {
            SetNextTimeToMove(currentWaypointsIndex);
            currentWaypointsIndex--;
        }
    }
}