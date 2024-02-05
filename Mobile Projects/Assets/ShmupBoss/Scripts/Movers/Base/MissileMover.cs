using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for missile movers, this includes missile following player or random enemy 
    /// or side spawnable missile mover.<br></br> 
    /// The main difference between a missile mover and a magnet mover is that a missile mover has a turn 
    /// speed which determines its direction to its target rather than simply getting closer to 
    /// regardless of its own direction.
    /// </summary>
    public abstract class MissileMover : MoverTracking
    {
        /// <summary>
        /// How fast a missile will turn towards its target.
        /// </summary>
        [Tooltip("How fast a missile will turn towards its target.")]
        public float TurnSpeed;

        /// <summary>
        /// Interpolated between the vector in the previous frame and current vector towards the target.
        /// </summary>
        protected Vector2 currentVector;

        /// <summary>
        /// This rotation is set when a missile has been initialized after being spawned. It is responsible for 
        /// the initial rotation the missile has.<br></br>
        /// Please note that this rotation is only set After a missile has been spawned, which in 
        /// practice means that for a single frame the missile fired will have the un-initialized 
        /// direction until it is changed.
        /// </summary>
        public Quaternion FiringRotation
        {
            set
            {
                Vector3 firingRotationInAngles = value.eulerAngles;
                float rotationAngle = firingRotationInAngles.z;

                currentVector = Math2D.DegreeToVector2(rotationAngle + EmitAngle).normalized;
                CurrentDirection = currentVector.normalized;
            }
        }

        /// <summary>
        /// The emit direction which is set when a missile is initialized translated into float angles.
        /// </summary>
        public float EmitAngle
        {
            get
            {
                return Math2D.VectorToDegree(EmitDirection);
            }
        }

        /// <summary>
        /// The emit direction represents the vector at which the missile is emitted out of the weapon.<br></br>
        /// It is different from the weapon rotation, this emit direction is needed when a weapon is emitting 
        /// missiles in a radial form or when using the spread option.<br></br> 
        /// This value is used in conjunction with the weapon rotation to find the initial direction vector of 
        /// the missiles.
        /// </summary>
        public Vector2 EmitDirection
        {
            get;
            set;
        }

        protected override void FindCurrentDirection()
        {
            if(tracker != null)
            {
                if (tracker.IsTargetFound)
                {
                    float t = TurnSpeed * ProjectConstants.TurnSpeedFactor * Time.deltaTime;
                    currentVector = Vector2.Lerp(currentVector, tracker.DirectionToTarget, t);
                }
            }

            CurrentDirection = currentVector.normalized;
        }
    }
}