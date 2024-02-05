using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Holds the values used to change agents vitals, movers speed, weapons and munition rate damage and speed.
    /// </summary>
    [CreateAssetMenu(menuName = "Shmup Boss/Multiplier Data")]
    public class MultiplierData : ScriptableObject
    {
        [Header("Enemies Multiplier")]
        public float EnemiesVitalsMultiplier = 1.0f;
        public float EnemiesSpeedMultiplier = 1.0f;
        public float EnemiesCollisionDamageMultiplier = 1.0f;
        public float ScoreMultiplier = 1.0f;
        public float CoinsMultiplier = 1.0f;

        [Header("Enemy Weapons Multiplier")]
        public float EnemiesWeaponsRateMultiplier = 1.0f;
        public float EnemiesMunitionDamageMultiplier = 1.0f;       
        public float EnemiesMunitionSpeedMultiplier = 1.0f;

        [Header("Player Multiplier")]
        public float PlayerVitalsMultiplier = 1.0f;
        public float PlayerSpeedMultiplier = 1.0f;
        public float PlayerCollisionDamageMultiplier = 1.0f;

        [Header("Player Weapons Multiplier")]
        public float PlayerWeaponsRateMultiplier = 1.0f;
        public float PlayerMunitionDamageMultiplier = 1.0f;
        public float PlayerMunitionSpeedMultiplier = 1.0f;

        /// <summary>
        /// An operator override to allow multiplication of this data with an integer, 
        /// typically this integer will be the level number in an infinite spawner level.
        /// </summary>
        /// <param name="data">The multiplier data you wish to multiply</param>
        /// <param name="multiplier">The integer you want to multiply with</param>
        /// <returns>The data after having been multiplied</returns>
        public static MultiplierData operator * (MultiplierData data, int multiplier)
        {
            MultiplierData result = CreateInstance<MultiplierData>();

            result.EnemiesVitalsMultiplier = data.EnemiesVitalsMultiplier * multiplier;
            result.EnemiesSpeedMultiplier = data.EnemiesSpeedMultiplier * multiplier;
            result.EnemiesCollisionDamageMultiplier = data.EnemiesCollisionDamageMultiplier * multiplier;

            result.ScoreMultiplier = data.ScoreMultiplier * multiplier;
            result.CoinsMultiplier = data.CoinsMultiplier * multiplier;

            result.EnemiesWeaponsRateMultiplier = data.EnemiesWeaponsRateMultiplier * multiplier;
            result.EnemiesMunitionDamageMultiplier = data.EnemiesMunitionDamageMultiplier * multiplier;
            result.EnemiesMunitionSpeedMultiplier = data.EnemiesMunitionSpeedMultiplier * multiplier;

            result.PlayerVitalsMultiplier = data.PlayerVitalsMultiplier * multiplier;
            result.PlayerSpeedMultiplier = data.PlayerSpeedMultiplier * multiplier;
            result.PlayerCollisionDamageMultiplier = data.PlayerCollisionDamageMultiplier * multiplier;

            result.PlayerWeaponsRateMultiplier = data.PlayerWeaponsRateMultiplier * multiplier;
            result.PlayerMunitionDamageMultiplier = data.PlayerMunitionDamageMultiplier * multiplier;
            result.PlayerMunitionSpeedMultiplier = data.PlayerMunitionSpeedMultiplier * multiplier;

            return result;
        }

        /// <summary>
        /// An operator override to allow addition of 2 multiplier datas, this is useful when you 
        /// want to take into account for example the original difficulty multiplier along with 
        /// the infinite incremental data of the infinite level.
        /// </summary>
        /// <param name="data1">First data to add</param>
        /// <param name="data2">Second data to add</param>
        /// <returns>The data after having been added</returns>
        public static MultiplierData operator + (MultiplierData data1, MultiplierData data2)
        {
            MultiplierData result = CreateInstance<MultiplierData>();

            result.EnemiesVitalsMultiplier = data1.EnemiesVitalsMultiplier + data2.EnemiesVitalsMultiplier;
            result.EnemiesSpeedMultiplier = data1.EnemiesSpeedMultiplier + data2.EnemiesSpeedMultiplier;
            result.EnemiesCollisionDamageMultiplier = data1.EnemiesCollisionDamageMultiplier + data2.EnemiesCollisionDamageMultiplier;

            result.ScoreMultiplier = data1.ScoreMultiplier + data2.ScoreMultiplier;
            result.CoinsMultiplier = data1.CoinsMultiplier + data2.CoinsMultiplier;

            result.EnemiesWeaponsRateMultiplier = data1.EnemiesWeaponsRateMultiplier + data2.EnemiesWeaponsRateMultiplier;
            result.EnemiesMunitionDamageMultiplier = data1.EnemiesMunitionDamageMultiplier + data2.EnemiesMunitionDamageMultiplier;
            result.EnemiesMunitionSpeedMultiplier = data1.EnemiesMunitionSpeedMultiplier + data2.EnemiesMunitionSpeedMultiplier;

            result.PlayerVitalsMultiplier = data1.PlayerVitalsMultiplier + data2.PlayerVitalsMultiplier;
            result.PlayerSpeedMultiplier = data1.PlayerSpeedMultiplier + data2.PlayerSpeedMultiplier;
            result.PlayerCollisionDamageMultiplier = data1.PlayerCollisionDamageMultiplier + data2.PlayerCollisionDamageMultiplier;

            result.PlayerWeaponsRateMultiplier = data1.PlayerWeaponsRateMultiplier + data2.PlayerWeaponsRateMultiplier;
            result.PlayerMunitionDamageMultiplier = data1.PlayerMunitionDamageMultiplier + data2.PlayerMunitionDamageMultiplier;
            result.PlayerMunitionSpeedMultiplier = data1.PlayerMunitionSpeedMultiplier + data2.PlayerMunitionSpeedMultiplier;

            return result;
        }
    }
}