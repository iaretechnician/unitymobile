namespace ShmupBoss
{
    /// <summary>
    /// Saves, difficulty level, volume settings, auto fire option and the selected input method.
    /// </summary>
    [System.Serializable]
    public class GameSettings
    {
        /// <summary>
        /// This difficulty number is the index which will determine the multiplier data array element 
        /// used as a multiplier data.
        /// </summary>
        public int DifficultyLevel;

        public float MasterVolume = 1.0f;
        public float MusicVolume = 1.0f;
        public float SfxVolume = 1.0f;

        public bool IsAutoFiring = false;

        public InputMethod CurrentInputMehtod;
    }
}