using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// A static class which stores all the current static values for the global difficulty system 
    /// (i.e. multiplier.)<br></br>
    /// This data is saved by the multiplier component which uses scriptable object multiplier data (difficulty data.)
    /// This data is selected by the difficulty level which is set either at the multiplier component itself or 
    /// through the game manager settings (main menu.)<br></br>
    /// To test and see this data during the level you can add the CurrentMultiplierTester.cs to any object inside 
    /// your scene level. 
    /// </summary>
    public static class CurrentMultiplier
    {
        public static float EnemiesVitalsMultiplier = 1.0f;
        public static float EnemiesSpeedMultiplier = 1.0f;
        public static float EnemiesCollisionDamageMultiplier = 1.0f;
        public static float ScoreMultiplier = 1.0f;
        public static float CoinsMultiplier = 1.0f;

        public static float EnemiesWeaponsRateMultiplier = 1.0f;
        public static float EnemiesMunitionDamageMultiplier = 1.0f;
        public static float EnemiesMunitionSpeedMultiplier = 1.0f;

        public static float PlayerVitalsMultiplier = 1.0f;
        public static float PlayerSpeedMultiplier = 1.0f;
        public static float PlayerCollisionDamageMultiplier = 1.0f;

        public static float PlayerWeaponsRateMultiplier = 1.0f;
        public static float PlayerMunitionDamageMultiplier = 1.0f;
        public static float PlayerMunitionSpeedMultiplier = 1.0f;

        public static void SetData(MultiplierData multiplierData)
        {
            if(multiplierData == null)
            {
                Debug.Log("CurrentMultiplier.cs: trying to set a multiplier data which is null.");

                return;
            }

            EnemiesVitalsMultiplier = multiplierData.EnemiesVitalsMultiplier;
            EnemiesSpeedMultiplier = multiplierData.EnemiesSpeedMultiplier;
            EnemiesCollisionDamageMultiplier = multiplierData.EnemiesCollisionDamageMultiplier;
            ScoreMultiplier = multiplierData.ScoreMultiplier;
            CoinsMultiplier = multiplierData.CoinsMultiplier;

            EnemiesWeaponsRateMultiplier = multiplierData.EnemiesWeaponsRateMultiplier;
            EnemiesMunitionDamageMultiplier = multiplierData.EnemiesMunitionDamageMultiplier;
            EnemiesMunitionSpeedMultiplier = multiplierData.EnemiesMunitionSpeedMultiplier;

            PlayerVitalsMultiplier = multiplierData.PlayerVitalsMultiplier;
            PlayerSpeedMultiplier = multiplierData.PlayerSpeedMultiplier;
            PlayerCollisionDamageMultiplier = multiplierData.PlayerCollisionDamageMultiplier;

            PlayerWeaponsRateMultiplier = multiplierData.PlayerWeaponsRateMultiplier;
            PlayerMunitionDamageMultiplier = multiplierData.PlayerMunitionDamageMultiplier;
            PlayerMunitionSpeedMultiplier = multiplierData.PlayerMunitionSpeedMultiplier;
        }
    }
}