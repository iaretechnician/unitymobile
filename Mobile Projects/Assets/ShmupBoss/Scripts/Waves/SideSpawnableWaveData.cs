using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Wave data which applies to all enemies with movers that spawn from the side, this includes 
    /// all types of movers with the exception of the waypoint mover and the curve mover.
    /// </summary>
    [CreateAssetMenu(menuName = "Shmup Boss/Waves/Side Spawnable Wave")]
    public class SideSpawnableWaveData : WaveData, IRandomPositionSpawnable
    {
        /// <summary>
        /// The side from which adversaries will be spawned from.
        /// </summary>
        [Tooltip("The side from which adversaries will be spawned from.")]
        [SerializeField]
        protected RectSideRandom spawnSide;
        public RectSideRandom SpawnSide
        {
            get
            {
                return spawnSide;
            }
        }

        /// <summary>
        /// This will offset the spawn position of the adversaries.<br></br>
        /// The bigger the value the further away from the side the adversary will be spawned.
        /// </summary>
        [Tooltip("This will offset the spawn position of the adversaries." + "\n" + 
            "The bigger the value the further away from the side the adversary will be spawned.")]
        [SerializeField]
        protected float spawnSideOffset;
        public float SpawnSideOffset
        {
            get
            {
                return spawnSideOffset;
            }
        }

        /// <summary>
        /// This will only be used if isRandomSpawnPosition is set to false, it will determine the location on 
        /// the spawn side, i.e. on the center or around the corners.
        /// </summary>
        [Tooltip("This will only be used if isRandomSpawnPosition is unchecked, it will determine the location " +
            "on the spawn side, i.e. on the center or around the corners.")]
        [Range(0, 1)]
        [SerializeField]
        protected float spawnPosition = 0.5f;
        public float SpawnPosition
        {
            get
            {
                return spawnPosition;
            }
        }

        /// <summary>
        /// If this is checked it will discard the value of the spawn position and use instead the min 
        /// and max values to determine the spawn position ratio on the edge.
        /// </summary>
        [Tooltip("If this is checked it will discard the value of the spawn position and use instead the " +
            "min and max values to determine the spawn position ratio on the edge.")]
        [SerializeField]
        protected bool isRandomSpawnPosition;
        public bool IsRandomSpawnPosition
        {
            get
            {
                return isRandomSpawnPosition;
            }
        }

        /// <summary>
        /// This will only be used if isRandomSpawnPosition is set to true.
        /// </summary>
        [Tooltip("This will only be used if isRandomSpawnPosition is checked.")]
        [Range(0, 1)]
        [SerializeField]
        protected float minRandomSpawnPosition;
        public float MinRandomSpawnPosition
        {
            get
            {
                return minRandomSpawnPosition;
            }
        }

        /// <summary>
        /// This will only be used if isRandomSpawnPosition is set to true.
        /// </summary>
        [Tooltip("This will only be used if isRandomSpawnPosition is checked.")]
        [Range(0, 1)]
        [SerializeField]
        protected float maxRandomSpawnPosition;
        public float MaxRandomSpawnPosition
        {
            get
            {
                return maxRandomSpawnPosition;
            }
        }

        /// <summary>
        /// The prefab or prefabs which will be spawned by this wave data. 
        /// These must have a mover type which is side spawnable.
        /// </summary>
        [Header("Prefabs To Spawn")]
        [Tooltip("The prefab or prefabs which will be spawned by this wave data. " +
            "These must have a mover type which is side spawnable.")]
        [SerializeField]
        protected Mover[] enemyPrefabs;
        public Mover[] EnemyPrefabs
        {
            get
            {
                return enemyPrefabs;
            }
        }
    }
}