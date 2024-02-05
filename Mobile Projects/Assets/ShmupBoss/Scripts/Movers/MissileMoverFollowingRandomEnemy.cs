using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// This mover is for a missile fired from the player and will seek any random enemy using 
    /// the random enemy tracker.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Munition/Player Missile Mover")]
    public class MissileMoverFollowingRandomEnemy : MissileMover
    {
        public override float CurrentSpeed
        {
            get
            {
                return currentSpeed * CurrentMultiplier.PlayerMunitionSpeedMultiplier;
            }
            protected set
            {
                currentSpeed = value;
            }
        }

        public override float FacingAngle
        {
            get
            {
                return FacingAngles.Player;
            }
        }

        protected override void CacheTracker()
        {
            tracker = GetComponent<TrackerRandomEnemy>();

            if (tracker == null)
            {
                tracker = gameObject.AddComponent<TrackerRandomEnemy>();
            }
        }

        public override float EditRotation(float angle)
        {
            return Math2D.VectorToDegree(CurrentDirection) + FacingAngles.Player - 90.0f;
        }
    }
}