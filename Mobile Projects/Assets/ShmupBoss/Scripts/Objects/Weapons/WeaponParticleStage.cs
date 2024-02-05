using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Contains all the information needed for a particle weapon, this stage can be used in an 
    /// array for a player weapon to simulate upgraded stages.
    /// </summary>
    [System.Serializable]
    public class WeaponParticleStage
    {
        /// <summary>
        /// Rate of particles fired per second, higher values means a faster firing rate.
        /// </summary>
        public float Rate;

        /// <summary>
        /// The damage each particle will make when hitting the opposing faction.
        /// </summary>
        public float Damage;

        public float Size;
        public float ColliderSize;
        public float Speed;

        /// <summary>
        /// This would rotate the particle around itself.
        /// </summary>
        public float RotationSpeed;

        /// <summary>
        /// Used to fire particles radially, when used please add a value to the arc angle spread or otherwise 
        /// particles will be fired on top of each other.
        /// </summary>
        public int BulletsNumber;

        /// <summary>
        /// Determines how spread out the radial particles are if the bullets number is bigger than 1.
        /// </summary>
        public float ArcAngleSpread;

        /// <summary>
        /// Offsets the fired radial particles away from the weapon center by the radius value.
        /// </summary>
        public float ArcRadius;

        /// <summary>
        /// The distance particles will travel before being eliminated, a distance of 0 means they will continue 
        /// traveling infinitely and you would need to use a particle destroyed to eliminate them instead.
        /// </summary>
        public float Distance;

        /// <summary>
        /// After the agent with the weapon which uses this particle stage is despawned, the weapon needs to stay 
        /// active for a time. If the weapon is deactivated immediately; all of its particles will disapper 
        /// instantly, this time determines for how long will the particle weapon stay active after its agent 
        /// has been despawned.
        /// </summary>
        public float Overtime;

        /// <summary>
        /// Would make the particle follow the rotation of the weapon which is firing them otherwise they 
        /// will be unaffected.
        /// </summary>
        public bool IsFollowingRotation;

        /// <summary>
        /// The material fired particles will have. This practically holds the sprite which will be used 
        /// for the particles.
        /// </summary>
        public UnityEngine.Object BulletMaterial;

        /// <summary>
        /// The sound effect made when a weapon fires a particle.
        /// </summary>
        public UnityEngine.Object FiringSFX;

        public float SfxVolume;

        /// <summary>
        /// This time would force a wait period between making firing sound effects, this is usually needed when 
        /// you have a weapon with a very high rate of fire which would make the sound effects overlap and become 
        /// incomprehensible.
        /// </summary>
        public float SfxCoolDownTime;

        /// <summary>
        /// If added, this material will be added as a trail for every fired particle.
        /// </summary>
        public UnityEngine.Object TrailMaterial;

        /// <summary>
        /// Determines how long the particle trail will be.
        /// </summary>
        public float TrailTime;

        public float TrailWidth;

        /// <summary>
        /// The curve that will hold how the particles move over time, can be used to change particles 
        /// movement from a straight line into a curved line.
        /// </summary>
        public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 0));

        public float CurveRange;
    }
}