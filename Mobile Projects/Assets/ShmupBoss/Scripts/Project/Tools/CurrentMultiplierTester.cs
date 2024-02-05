using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// This component is only used for debugging and to better understand how the multiplier works 
    /// and has no other function.<br></br>
    /// It will display the current static multiplier data so that you are able to know what multiplier 
    /// data variables are currently used.
    /// </summary>
    public class CurrentMultiplierTester : MonoBehaviour
    {
        [SB_HelpBox("This tool is just to show the currently used multiplier data, this is helpful for debugging " +
            "or for understanding how the multiplier is working in an infinite level.")]
        [Tooltip("This will display the data when Unity is in the selected execution order.")]
        public ExecutionHook ShowDataWhen;

        [SB_HelpBox("Do no adjust, just observe the data change after hitting play.")]
        [Header("Displayed Data")]
        [Tooltip("This is the currently used difficulty level either from the multiplier or the " +
            "game manager settings.")]
        public int DifficultyLevelUsed = -1;

        public int CurrentInfiniteLevelIndex = -1;

        [Space]
        public float EnemiesVitalsMultiplier;
        public float EnemiesSpeedMultiplier;
        public float EnemiesCollisionDamageMultiplier;
        public float ScoreMultiplier;
        public float CoinsMultiplier;

        [Space]
        public float EnemiesWeaponsRateMultiplier;
        public float EnemiesMunitionDamageMultiplier;
        public float EnemiesMunitionSpeedMultiplier;

        [Space]
        public float PlayerVitalsMultiplier;
        public float PlayerSpeedMultiplier;
        public float PlayerCollisionDamageMultiplier;

        [Space]
        public float PlayerWeaponsRateMultiplier;
        public float PlayerMunitionDamageMultiplier;
        public float PlayerMunitionSpeedMultiplier;

        private void Awake()
        {
            if(ShowDataWhen == ExecutionHook.Awake)
            {
                GetCurrentMultiplierData();              
            }
        }

        private void Start()
        {
            if (ShowDataWhen == ExecutionHook.Start)
            {
                GetCurrentMultiplierData();
            }
        }

        void Update()
        {
            if (ShowDataWhen == ExecutionHook.Update)
            {
                GetCurrentMultiplierData();
            }
        }

        /// <summary>
        /// The static multiplier class holds all the static variables which are used throughout the pack, 
        /// this method will get the multiplier static variables and save them so they can be displayed and you will be 
        /// able to find out what values are being currently used.
        /// </summary>
        private void GetCurrentMultiplierData()
        {
            if(Multiplier.Instance != null)
            {
                DifficultyLevelUsed = Multiplier.Instance.DifficultyLevel;
                CurrentInfiniteLevelIndex = Multiplier.Instance.CurrentInfiniteLevelIndex;
            }

            EnemiesVitalsMultiplier = CurrentMultiplier.EnemiesVitalsMultiplier;
            EnemiesSpeedMultiplier = CurrentMultiplier.EnemiesSpeedMultiplier;
            EnemiesCollisionDamageMultiplier = CurrentMultiplier.EnemiesCollisionDamageMultiplier;
            ScoreMultiplier = CurrentMultiplier.ScoreMultiplier;
            CoinsMultiplier = CurrentMultiplier.CoinsMultiplier;

            EnemiesWeaponsRateMultiplier = CurrentMultiplier.EnemiesWeaponsRateMultiplier;
            EnemiesMunitionDamageMultiplier = CurrentMultiplier.EnemiesMunitionDamageMultiplier;
            EnemiesMunitionSpeedMultiplier = CurrentMultiplier.EnemiesMunitionSpeedMultiplier;

            PlayerVitalsMultiplier = CurrentMultiplier.PlayerVitalsMultiplier;
            PlayerSpeedMultiplier = CurrentMultiplier.PlayerSpeedMultiplier;
            PlayerCollisionDamageMultiplier = CurrentMultiplier.PlayerCollisionDamageMultiplier;

            PlayerWeaponsRateMultiplier = CurrentMultiplier.PlayerWeaponsRateMultiplier;
            PlayerMunitionDamageMultiplier = CurrentMultiplier.PlayerMunitionDamageMultiplier;
            PlayerMunitionSpeedMultiplier = CurrentMultiplier.PlayerSpeedMultiplier;
        }
    }
}