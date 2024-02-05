using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// This mover is for a missile fired from an enemy and will seek the player using the player tracker.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Weapons/Munition/Enemy Missile Mover")]
    public class MissileMoverFollowingPlayer : MissileMover
    {
        public override float CurrentSpeed
        {
            get
            {
                return currentSpeed * CurrentMultiplier.EnemiesMunitionSpeedMultiplier;
            }
            protected set
            {
                currentSpeed = value;
            }
        }

        public override float EditRotation(float angle)
        {
            return Math2D.VectorToDegree(CurrentDirection) + FacingAngles.NonPlayer - 90.0f;
        }
    }
}