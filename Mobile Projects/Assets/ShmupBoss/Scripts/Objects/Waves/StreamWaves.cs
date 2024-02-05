using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Holds infrormation for spawning enemies (adversaries) in an infinite level.<br></br>
    /// Each stream wave holds an array of waves and information on which sub levels to spawn them.
    /// </summary>
    [System.Serializable]
    public class StreamWaves
    {
        /// <summary>
        /// The waves which will be spawned during this stream.
        /// </summary>
        [Tooltip("The waves which will be spawned during this stream.")]
        public Wave[] Waves;

        /// <summary>
        /// The number of enemies that will be increased after each time this stream has spawned, 
        /// this does not need to be 1 or bigger and fractions can be used.
        /// </summary>
        [Tooltip("The number of enemies that will be increased after each time this stream has spawned, " +
            "this does not need to be 1 or bigger and fractions can be used.")]
        public float IncreasedAmount;

        /// <summary>
        /// An infinite level is made up of an infinite number of sub levels.<br></br>
        /// The start level, determines at what sub level a stream wave will start.
        /// </summary>
        [Tooltip("An infinite level is made up of an infinite number of sub levels. " +
            "The start level, determines at what sub level a stream wave will start.")]
        public int StartLevel;

        /// <summary>
        /// An infinite level is made up of an infinite number of sub levels.<br></br>
        /// The end level, determines at what sub level a stream wave will start.
        /// </summary>
        [Tooltip("An infinite level is made up of an infinite number of sub levels. " +
            "The end level, determines at what sub level a stream wave will start.")]
        public int EndLevel;

        /// <summary>
        /// In addition to the start and end level, the every nth level will determine the 
        /// frequency of spawning this stream. A frequency of 1 means the stream will spawn 
        /// in every level between the start and end levels, a frequency of 2 means it will 
        /// spawn once, skip the next level, then spawn again, and so on.
        /// </summary>
        [Tooltip("In addition to the start and end level, the every nth level will determine" +
            " the frequency of spawning this stream. A frequency of 1 means the stream will " +
            "spawn in every level between the start and end levels, a frequency of 2 means " +
            "it will spawn once, skip the next level, then spawn again, and so on.")]
        public int EveryNthLevel;

        /// <summary>
        /// This is used to determine the amount to increase which is related to 
        /// how many times a stream has spawned.
        /// </summary>
        public int HowManyTimesSpawned
        {
            get;
            set;
        }
    }
}