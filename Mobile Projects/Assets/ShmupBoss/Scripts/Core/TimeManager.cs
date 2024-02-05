using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Contains methods for pausing/unpausing the game and changing the default game speed (time scale).<br></br>
    /// </summary>
    public static class TimeManager
    {
        private static float defaultTimeScale = 1.0f;

        /// <summary>
        /// Changing this value will change the game speed (time scale.)
        /// </summary>
        public static float DefaultTimeScale
        {
            get 
            { 
                return defaultTimeScale; 
            }
            set 
            { 
                defaultTimeScale = value;
            }
        }

        /// <summary>
        /// Pauses time in the game.
        /// </summary>
        public static void Pause()
        {
            Time.timeScale = 0;
        }

        /// <summary>
        /// Unpauses time in the game by setting the time scale to the default time scale value.
        /// </summary>
        public static void UnPause()
        {
            Time.timeScale = DefaultTimeScale;
        }
    }
}