using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Contains all the information needed for a munition weapon, this stage can be used in an 
    /// array for a player weapon to simulate upgraded stages.
    /// </summary>
    [System.Serializable]
    public class WeaponMunitionStage
    {
        /// <summary>
        /// Rate of munition fired per second, higher values means a faster firing rate.
        /// </summary>
        [Tooltip("Rate of munition fired per second, higher values means a faster firing rate.")]
        public float Rate;

        /// <summary>
        /// The munition (bullet or missile) which will be fired by the weapon using this stage.
        /// </summary>
        [Tooltip("The munition (bullet or missile) which will be fired by the weapon using this stage.")]
        public Munition MunitionPrefab;

        /// <summary>
        /// Modifies the scale of the fired munition. Please make sure it is not set to zero.
        /// </summary>
        [Tooltip("Modifies the scale of the fired munition. Please make sure it is not set to zero.")]
        public Vector3 MunitionScale = Vector3.one;

        /// <summary>
        /// Forces the munition to be fired in a random radial manner, the bigger the angle 
        /// the more spread out the munition will be.
        /// </summary>
        [Tooltip("Forces the munition to be fired in a random radial manner, the bigger the angle " +
            "the more spread out the munition will be.")]
        public float ScatterAngle;

        /// <summary>
        /// Used to fire munition radially, when used please add a value to the arc angle spread or otherwise the 
        /// munition will be fired on top of each other.
        /// </summary>
        [Header("Radial Firing")]
        [Tooltip("Used to fire munition radially, when used please add a value to the arc angle spread or otherwise " +
            "the munition will be fired on top of each other.")]
        public int BulletsNumber;

        /// <summary>
        /// Determines how spread out the radial bullets are if the bullets number is bigger than 1.
        /// </summary>
        [Tooltip("Determines how spread out the radial bullets are if the bullets number is bigger than 1.")]
        public float ArcAngleSpread;

        /// <summary>
        /// The sound effect made when a weapon fires a munition.
        /// </summary>
        [Header("Sound")]
        [Tooltip("The sound effect made when a weapon fires a munition.")]
        public VolumetricAC FiringSFX;

        /// <summary>
        /// This time would force a wait period between making firing sound effects, this is usually needed when 
        /// you have a weapon with a very high rate of fire which would make the sound effects overlap and become 
        /// incomprehensible.
        /// </summary>
        [Tooltip("This time would force a wait period between making firing sound effects, this is usually needed " +
            "when you have a weapon with a very high rate of fire which would make the sound effects overlap and " +
            "become incomprehensible.")]
        public float FiringSfxCoolDownTime;
    }
}