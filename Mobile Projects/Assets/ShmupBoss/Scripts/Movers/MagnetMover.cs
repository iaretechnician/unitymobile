using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Mover which will move towards whatever target is set in the tracker in an incremental
    /// magent like fashion. It will basically be attracted to the target and move towards it
    /// in a constant speed.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Movers/Magnet Mover")]
    public class MagnetMover : MoverTracking
    {
        /// <summary>
        /// The speed which the mover will use when within the target magnet radius.
        /// </summary>
        [Tooltip("The speed which the mover will use when within the target magnet radius.")]
        [SerializeField]
        private float magnetSpeed;

        /// <summary>
        /// When the mover enters this target radius, it will move using the magnet speed and will 
        /// start moving towards the target.
        /// </summary>
        [SerializeField]
        [Tooltip("When the mover enters this target radius, it will move using the magnet speed " +
            "and will start moving towards the target.")]
        private float magnetRadius;

        /// <summary>
        /// The radius in which the mover will move closer to its target.
        /// </summary>
        protected bool isWithinMagnetRadius;

        protected override void Update()
        {
            base.Update();
            UpdateSpeedAndIfIsWithinMagentRadius();
        }

        protected virtual void UpdateSpeedAndIfIsWithinMagentRadius()
        {
            if (tracker != null)
            {
                if (tracker.IsTargetFound)
                {
                    if (tracker.DistanceToTarget <= magnetRadius)
                    {
                        isWithinMagnetRadius = true;
                        CurrentSpeed = magnetSpeed;
                    }
                    else
                    {
                        isWithinMagnetRadius = false;
                        CurrentSpeed = speed;
                    }
                }
                else
                {
                    isWithinMagnetRadius = false;
                }
            }
        }

        protected override void FindCurrentDirection()
        {
            if (tracker.IsTargetFound && isWithinMagnetRadius)
            {
                CurrentDirection =  tracker.DirectionToTarget;
            }
            else
            {
                CurrentDirection = FacingDirections.NonPlayer;
            }
        }

        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            GizmosExtension.DrawCircle(transform.position, magnetRadius);
        }
    }
}