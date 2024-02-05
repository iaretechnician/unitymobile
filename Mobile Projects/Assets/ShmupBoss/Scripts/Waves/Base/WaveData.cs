using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for all wave data, contains the common information of depth index, 
    /// number of spawned enemies and time between spawning them.
    /// </summary>
    public abstract class WaveData : ScriptableObject
    {
        /// <summary>
        /// How many enemies are spawned by this wave.
        /// </summary>
        [Header("Spawn Settings")]
        [Tooltip("How many enemies are spawned by this wave.")]
        public int NumberOfEnemiesToSpawn;

        /// <summary>
        /// If true, this will spawn the enemy prefabs randomly. If false it will spawn the enemy prefabs 
        /// in ordered sequence.
        /// </summary>
        [Tooltip("If true, this will spawn the enemy prefabs randomly. If false it will spawn the enemy " +
            "prefabs in an ordered sequence.")]
        [SerializeField]
        protected bool isSpawningEnemiesInRandomOrder;
        public bool IsSpawningEnemiesInRandomOrder
        {
            get
            {
                return isSpawningEnemiesInRandomOrder;
            }
        }

        /// <summary>
        /// The time between spawning each enemy in the wave, this is only active if isRandomTime is set to false.
        /// </summary>
        [Tooltip("The time between spawning each enemy in the wave, this is only active if isRandomTime " +
            "is set to false.")]
        public float TimeBetweenEnemies;

        /// <summary>
        /// If set to true, time between enemies will be discarded and the min and max time below will 
        /// be used to define a random time between spawning each enemy.
        /// </summary>
        [Tooltip("If checked, time between enemies will be discarded and the min and max time below will " +
            "be used to define a random time between spawning each enemy.")]
        [SerializeField]
        protected bool isRandomTimeBetweenEnemies;
        public bool IsRandomTimeBetweenEnemies
        {
            get
            {
                return isRandomTimeBetweenEnemies;
            }
        }

        /// <summary>
        /// Only active if isRandomTime is set to true, will determine the shortest time possible 
        /// between spawning enemies.
        /// </summary>
        [Tooltip("Only active if isRandomTime is checked, will determine the shortest time possible " +
            "between spawning enemies.")]
        [SerializeField]
        protected float minTimeBetweenEnemies;
        public float MinTimeBetweenEnemies
        {
            get
            {
                return minTimeBetweenEnemies;
            }
        }

        /// <summary>
        /// Only active if isRandomTime is set to true, will determine the largest time possible 
        /// between spawning enemies.
        /// </summary>
        [Tooltip("Only active if isRandomTime is checked, will determine the largest time possible " +
            "between spawning enemies.")]
        [SerializeField]
        protected float maxTimeBetweenEnemies;
        public float MaxTimeBetweenEnemies
        {
            get
            {
                return maxTimeBetweenEnemies;
            }
        }

        /// <summary>
        /// The Z depth layer index where an enemy will be spawned.<br></br>
        /// An index of 0 means the bottom most layer, higher numbers means the layers will go 
        /// on top, the distance between these layers is determined by the space between indices.
        /// </summary>
        [Header("Spawn Position Settings")]
        [Tooltip("The Z depth layer index where an enemy will be spawned." + "\n" + 
            "An index of 0 means the bottom most layer, higher numbers means the layers will go on top, " +
            "the distance between these layers is determined by the space between indices.")]
        [Range(0, ProjectConstants.DepthIndexLimit)]
        [SerializeField]
        protected int depthIndex;
        public int DepthIndex
        {
            get
            {
                return depthIndex;
            }
        }
    }
}