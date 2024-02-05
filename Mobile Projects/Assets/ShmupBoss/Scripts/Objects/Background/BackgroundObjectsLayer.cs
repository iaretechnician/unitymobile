using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Layer of game object that will be spawned as background objects used as 
    /// decoratives with options for randomized spawning and appearance.
    /// </summary>
    [System.Serializable]
    public class BackgroundObjectsLayer : IRandomPositionSpawnable
    {
        [Tooltip("This layer name is only used for organizational purposes in the editor.")]
        public string LayerName;
        
        /// <summary>
        /// Game objects that will be spawned as background objects in this layer.
        /// </summary>
        [Header("Prefabs To Spawn")]
        [Tooltip("Game objects that will be spawned as background objects in this layer.")]
        public GameObject[] Prefabs;

        /// <summary>
        /// If true, this will spawn the prefabs randomly, otherwise they will be spawned 
        /// by their order in the array.
        /// </summary>
        [Tooltip("If checked, this will spawn the prefabs randomly, otherwise they will be " +
            "spawned by their order in the array.")]
        public bool IsSpawningInRandomOrder;

        /// <summary>
        /// If true, this layer will continue to spawn objects non stop during the level and the 
        /// spawn count will not be considered.
        /// </summary>
        [Tooltip("If true, this layer will continue to spawn objects non stop during the level and " +
            "the spawn count will not be considered.")]
        public bool IsSpawningInfinitely;

        /// <summary>
        /// This number is only used if "IsSpawningInfinitely" is false, 
        /// this would limit the number of spawned ojects to this count.
        /// </summary>
        [Tooltip("This number is only used if IsSpawningInfinitely is unchecked, spawn count would limit " +
            "the number of spawned objects to this count.")]
        public int SpawnCount;

        /// <summary>
        /// How many are initially pooled.
        /// </summary>
        [Tooltip("How many are initially pooled.")]
        public int PoolLimit;

        [Header("Spawn Time Settings")]
        public float WaitTimeBeforeFirstSpawn;

        /// <summary>
        /// The time in-between spawning each background object in this layer. 
        /// This is only used if "IsRandomWaitTime" is false.
        /// </summary>
        [Tooltip("The time in-between spawning each background object in this layer. This is " +
            "only used if IsRandomWaitTime is unchecked.")]
        public float WaitTimeBetweenSpawns;

        /// <summary>
        /// If true, the layer uses the min/max random time instead of the wait time between spawns.
        /// </summary>
        [Tooltip("If checked, the layer uses the min/max random time instead of the wait time between spawns.")]
        public bool IsRandomWaitTime;

        /// <summary>
        /// Only used if "IsRandomWaitTime" is set to true, represents the minimum possible 
        /// time between spawning background objects in this layer 
        /// </summary>
        [Tooltip("Only used if IsRandomWaitTime is checked, represents the minimum possible time between " +
            "spawning background objects in this layer.")]
        public float MinRandomTime;

        /// <summary>
        /// Only used if "IsRandomWaitTime" is set to true, represents the maximum possible 
        /// time between spawning background objects in this layer 
        /// </summary>
        [Tooltip("Only used if IsRandomWaitTime is checked, represents the maximum possible time " +
            "between spawning background objects in this layer ")]
        public float MaxRandomTime;

        /// <summary>
        /// Only used if "IsRandomScale" is false, this will modify the scale of the spawned objects.
        /// </summary>
        [Header("Scale Settings")]
        [Tooltip("Only used if IsRandomScale is unchecked, this will modify the scale of the spawned objects.")]
        public Vector3 Scale = Vector3.one;

        /// <summary>
        /// If true, the layer uses the min/max random scale values instead of the scale field.
        /// </summary>
        [Tooltip("If true, the layer uses the min/max random scale values instead of the scale field.")]
        public bool IsRandomScale;

        /// <summary>
        /// Used if "IsRandomScale" is true and sets the minimum random scale for the spawned objects.
        /// </summary>
        [Tooltip("Used if IsRandomScale is checked and sets the minimum random scale for the spawned objects.")]
        public float MinRandomScale;

        /// <summary>
        /// Used if "IsRandomScale" is true and sets the maximum random scale for the spawned objects.
        /// </summary>
        [Tooltip("Used if IsRandomScale is checked and sets the maximum random scale for the spawned objects.")]
        public float MaxRandomScale;

        /// <summary>
        /// This depth index will determine the position of the layer on the Z axis.
        /// </summary>
        [Header("Spawn Position Settings")]
        [Tooltip("This depth index will determine the position of the layer on the Z axis.")]
        [Range(0, ProjectConstants.DepthIndexLimit)]
        [SerializeField]
        private int depthIndex;
        public int DepthIndex
        {
            get
            {
                return depthIndex;
            }
        }

        /// <summary>
        /// Which side are objects spawned from, their direction of movement will be selected automatically 
        /// based on their spawn side.
        /// </summary>
        [Tooltip("Which side are objects spawned from, their direction of movement will be selected automatically " +
            "based on their spawn side.")]
        [SerializeField]
        private RectSideRandom spawnSide;
        public RectSideRandom SpawnSide
        {
            get
            {
                return spawnSide;
            }
        }

        /// <summary>
        /// How far from the edge of the playfield are the background objects being spawned.
        /// </summary>
        [Tooltip("How far from the edge of the playfield are the background objects being spawned.")]
        [SerializeField]
        private float spawnSideOffset;
        public float SpawnSideOffset
        {
            get
            {
                return spawnSideOffset;
            }
        }

        /// <summary>
        /// The ratio of where on the spawn side will the object be spawned.<br></br>
        /// For example a value of 0.5 means that the object will be spawned from the center of the spawn side 
        /// while a value of 0 means it will be spawned at the first corner and 1 on the opposing corner.
        /// </summary>
        [Tooltip("The ratio of where on the spawn side will the object be spawned. For example a value of 0.5 means " +
            "that the object will be spawned from the center of the spawn side while a value of 0 means it will be " +
            "spawned at the first corner and 1 on the opposing corner.")]
        [Range(0.0f,1.0f)]
        [SerializeField]
        private float spawnPosition;
        public float SpawnPosition
        {
            get
            {
                return spawnPosition;
            }
        }

        /// <summary>
        /// If true, the spawn position will be discarded and the min and max random spawn position values 
        /// will be used to pick the position of spawning on the spawn side.
        /// </summary>
        [Tooltip("If checked, the spawn position will be discarded and the min and max random spawn position values " +
            "will be used to pick the position of spawning on the spawn side.")]
        [SerializeField]
        private bool isRandomSpawnPosition;
        public bool IsRandomSpawnPosition
        {
            get
            {
                return isRandomSpawnPosition;
            }
        }

        /// <summary>
        /// Only used if the "isRandomSpawnPosition" value is true.
        /// </summary>
        [Range(0.0f, 1.0f)]
        [Tooltip("Only used if the isRandomSpawnPosition is checked.")]
        [SerializeField]
        private float minRandomSpawnPosition;
        public float MinRandomSpawnPosition
        {
            get
            {
                return minRandomSpawnPosition;
            }
        }

        /// <summary>
        /// Only used if the "isRandomSpawnPosition" value is true.
        /// </summary>
        [Range(0.0f, 1.0f)]
        [Tooltip("Only used if the isRandomSpawnPosition is checked.")]
        [SerializeField]
        private float maxRandomSpawnPosition;
        public float MaxRandomSpawnPosition
        {
            get
            {
                return maxRandomSpawnPosition;
            }
        }

        /// <summary>
        /// The speed of the spawned background object which will have its direction set automatically 
        /// depending on the spawn side.<br></br>
        /// Note: this value is only used if "IsRandomSpeed" value is false.
        /// </summary>
        [Header("Speed Settings")]
        [Tooltip("The speed of the spawned background object which will have its direction set automatically " +
            "depending on the spawn side. Note: this value is only used if IsRandomSpeed is unchecked.")]
        public float Speed;

        /// <summary>
        /// If true, the speed value will be discarded and the min and random speeds used instead.
        /// </summary>
        [Tooltip("If checked, the speed value will be discarded and the min and random speeds used instead.")]
        public bool IsRandomSpeed;

        /// <summary>
        /// Only used if the "IsRandomSpeed" value is true.
        /// </summary>
        [Tooltip("Only used if the IsRandomSpeed is checked.")]
        public float MinRandomSpeed;

        /// <summary>
        /// Only used if the "IsRandomSpeed" value is true.
        /// </summary>
        [Tooltip("Only used if the IsRandomSpeed is checked.")]
        public float MaxRandomSpeed;

        [Header("Rotation Settings")]
        [Tooltip("This will change the starting rotation of the spawned background object")]
        public Vector3 NewRotation;

        /// <summary>
        /// The rotation speed of the spawned random background object on the X axis, this is 
        /// used only if "IsRandomRotationSpeed" is set to false.
        /// </summary>
        [Tooltip("The rotation speed of the spawned random background object on the X axis, this is " +
            "used only if IsRandomRotationSpeed is unchecked")]
        public float XRotationSpeed;

        /// <summary>
        /// The rotation speed of the spawned random background object on the Y axis, this is 
        /// used only if "IsRandomRotationSpeed" is set to false.
        /// </summary>
        [Tooltip("The rotation speed of the spawned random background object on the Y axis, this is " +
            "used only if IsRandomRotationSpeed is unchecked")]
        public float YRotationSpeed;

        /// <summary>
        /// The rotation speed of the spawned random background object on the Z axis, this is 
        /// used only if "IsRandomRotationSpeed" is set to false.
        /// </summary>
        [Tooltip("The rotation speed of the spawned random background object on the Z axis, this is " +
            "used only if IsRandomRotationSpeed is unchecked")]
        public float ZRotationSpeed;

        /// <summary>
        /// If set to true, the min and max random rotation speeds will be used instead of the rotation 
        /// speeds on the axis.
        /// </summary>
        [Tooltip("If set to true, the min and max random rotation speeds will be used instead of the " +
            "rotation speeds on the axis.")]
        public bool IsRandomRotationSpeed;

        /// <summary>
        /// Only used if the "IsRandomRotationSpeed" value is true.
        /// </summary>
        [Tooltip("Only used if the IsRandomRotationSpeed is checked.")]
        public float MinRandomRotationSpeed;

        /// <summary>
        /// Only used if the "IsRandomRotationSpeed" value is true.
        /// </summary>
        [Tooltip("Only used if the IsRandomRotationSpeed is checked.")]
        public float MaxRandomRotationSpeed;

        /// <summary>
        /// Transform will hold this layer's background objects. This is used for purely orgnizational purposes.
        /// </summary>
        public Transform BackgroundObjectsHeirarchy
        {
            get;
            set;
        }
    }
}