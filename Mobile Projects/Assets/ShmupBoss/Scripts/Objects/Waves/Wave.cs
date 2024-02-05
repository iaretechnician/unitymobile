using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Contains the wave data and the time between it and the next wave.
    /// </summary>
    [System.Serializable]
    public class Wave
    {
        /// <summary>
        /// The wave data that will be spawned.
        /// </summary>
        [Tooltip("The wave data that will be spawned.")]
        public WaveData waveData;

        /// <summary>
        /// The wait time for spawning the next wave.
        /// </summary>
        [Tooltip("The wait time for spawning the next wave.")]
        public float TimeForNextWave;

        public Wave (WaveData data, float timeForNextWave)
        {
            waveData = data;
            TimeForNextWave = timeForNextWave;
        }
    }
}