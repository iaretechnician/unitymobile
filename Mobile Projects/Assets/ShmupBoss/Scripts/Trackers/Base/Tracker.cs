using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base clase for trackers. Contains information about the tracked target position,
    /// the vector and distance to it. It also raises an event whenever a target is reached.
    /// </summary>
    public abstract class Tracker : MonoBehaviour
    {
        public event CoreDelegate OnReachingTarget;
        
        public virtual bool IsTargetFound
        {
            get;
            protected set;
        }

        public virtual Transform TargetTransform
        {
            get;
            protected set;
        }

        public Vector2 TargetPosition
        {
            get;
            protected set;
        }

        /// <summary>
        /// The vector from the game object with tracker this script to its target, 
        /// the distance can be calculated using this vector2.
        /// </summary>
        protected Vector2 VectorToTarget;

        /// <summary>
        /// The normalized version of the VectorToTarget which gives the pure
        /// direction to the target.
        /// </summary>
        public Vector2 DirectionToTarget
        {
            get
            {
                return VectorToTarget.normalized;
            }
        }

        public float DistanceToTarget
        {
            get;
            protected set;
        }

        /// <summary>
        /// This changing bool represents the value of the "IsWithinTargetThreshold" bool
        /// and raises an event whenever that value is changed to true or false.
        /// </summary>
        protected ChangingBool changingIsWithinTargetThresholdBool = new ChangingBool();

        /// <summary>
        /// Returns true whenever the game object with this tracker script has reached its 
        /// target and is within the target threshold and returns false when it's far from it's target.
        /// </summary>
        public bool IsWithinTargetThreshold
        {
            get
            {
                return changingIsWithinTargetThresholdBool.CurrentValue;
            }
            protected set
            {
                changingIsWithinTargetThresholdBool.CurrentValue = value;
            }
        }

        /// <summary>
        /// Adds the on reaching target event to the "IsWithinTargetThreshold" changing bool event.
        /// </summary>
        protected virtual void Awake()
        {
            changingIsWithinTargetThresholdBool.OnChangeToTrue += RaiseOnReachingTarget;
        }

        /// <summary>
        /// Updates the target coordinates.
        /// </summary>
        protected virtual void Update()
        {
            UpdateTargetCoordinates();
        }

        protected void UpdateTargetCoordinates()
        {
            if (TargetTransform == null)
            {
                return;
            }

            if (IsTargetFound)
            {
                TargetPosition = TargetTransform.position;
                VectorToTarget = TargetTransform.position - transform.position;
                DistanceToTarget = VectorToTarget.magnitude;

                if (DistanceToTarget <= ProjectConstants.ReachedPositionThreshold)
                {
                    IsWithinTargetThreshold = true;
                }
                else
                {
                    IsWithinTargetThreshold = false;
                }
            }
            else
            {
                Reset();
            }
        }

        /// <summary>
        /// Resets this script values.
        /// </summary>
        protected virtual void OnEnable()
        {
            Reset();
        }

        /// <summary>
        /// Resets this script values.
        /// </summary>
        protected virtual void OnDisable()
        {
            Reset();
        }

        protected void Reset()
        {
            TargetPosition = Vector2.zero;
            VectorToTarget = Vector2.zero;
            DistanceToTarget = Mathf.Infinity;
            IsWithinTargetThreshold = false;
        }

        protected void RaiseOnReachingTarget()
        {
            OnReachingTarget?.Invoke(null);
        }
    }
}