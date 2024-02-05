using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Pools enemies and bosses (adversaries), to find which adversaries to pool, this script goes into
    /// spawner and objects which are nested under the EnemyPool game object (ground enemies).
    /// </summary>
    [AddComponentMenu("Shmup Boss/Pools/Enemy Pool")]
    public class EnemyPool : PoolBase
    {
        public static EnemyPool Instance;

        /// <summary>
        /// A crucial event for ending the level. This event is invoked when all currently active enemies 
        /// in the scene have been despawned. <br></br>
        /// This event In combination with the number of waves currently in level in the spawner 
        /// script end the level and raise the on level end event in the level script.<br></br>
        /// This event is potentially raised several times during the level, but only ends the level 
        /// when the spawner's end level method has been added to it after spawning has finished.
        /// </summary>
        public event CoreDelegate OnAllEnemiesDespawned;

        /// <summary>
        /// This number is incremented whenever an enemy is spawned and decremented whenever despawned. 
        /// </summary>
        private int numberOfActiveEnemies;
        public int NumberOfActiveEnemies
        {
            get
            {
                return numberOfActiveEnemies;
            }
            private set
            {
                numberOfActiveEnemies = value;

                if(value == 0)
                {
                    RaiseOnAllEnemiesDespawned();
                }
            }
        }

    /// <summary>
    /// Caps the pooled adversary(enemy) count, if for example it is found that an enemy is going to be spawned 100 times through out the level, this pool limit will be used to cap that value.
    /// </summary>
    [SerializeField]
        protected int poolLimit;

        /// <summary>
        /// Accessed by other pool managers to know what pikcups/FX/munition to pool.<br></br>
        /// Boss sub phases are also indexed in this dictionary so that their pickups/FX/Munition are pooled.
        /// </summary>
        public Dictionary<string, Enemy> EnemyByName = new Dictionary<string, Enemy>();

        /// <summary>
        /// Used internally in order to pool the minimum required amount of enemies and to account  
        /// whether or not an enemy is repeated in the waves and how many are needed, this amount is 
        /// also capped by the pool limit.<br></br>
        /// The word adversary have been used because this not only pools enemies and enemy inherited
        /// classes but also any bosses which do not inherit from the enemy class.<br></br>
        /// Note: The boss sub phases on the other hand do inherit from the enemy class but are not pooled
        /// since they are attached to the boss and are never spawned directly.
        /// </summary>
        private Dictionary<string, GoCount> adversaryGoCountByName = new Dictionary<string, GoCount>();

        protected override int Layer
        {
            get
            {
                return ProjectLayers.Enemies;
            }
        }

        private void OnValidate()
        {
            if (poolLimit <= 0)
            {
                poolLimit = 10;
            }
        }

        /// <summary>
        /// Subscribes the pool method to the first stage of the initializer.
        /// </summary>
        protected void Awake()
        {
            Instance = this;
            Level.IsFinishedLoading = false;

            Initializer.Instance.SubscribeToStage(Pool, 1);
        }

        protected override void Pool()
        {
            CreateHierarchy();
            RenameGroundEnemies();
            IndexUniqueAdversaries();
            CreateClones();
            CreateGameObjectsQueues();
            CreatePlaceholders();
            DestroyUserPlacedEnemies();
        }

        protected override void CreateHierarchy()
        {
            PoolHierarchy = new GameObject("----- Pooled Adversaries -----").transform;
        }

        /// <summary>
        /// Because enemies are spawned/despawned by name, each enemy needs to have the same name.<br></br>
        /// When duplicating enemies inside Unity, the name will be changed by adding a number in parentheses 
        /// after the actual name. This method removes those parentheses and renames the enemy back to 
        /// its original name so that you do not have to do it manually.
        /// </summary>
        private void RenameGroundEnemies()
        {
            foreach(Transform tr in transform)
            {
                StringTools.RemoveParenthesesAfterLastSpace(tr.gameObject);
            }          
        }

        /// <summary>
        /// Goes through ground adversaries and adversaries spawned by a wave spawner to index them by name.<br></br>
        /// Names must be unique. if two duplicate names are found; they will be treated as the same adversary.<br></br>
        /// An adversary is a term I use to describe both enemies and bosses, since technically speaking a boss class
        /// does not inherit from an enemy class and does not fall under an enemy catagory.
        /// </summary>
        private void IndexUniqueAdversaries()
        {
            IndexGroundAdversaries();
            IndexFiniteSpawnerAdversaries();
            IndexInfiniteSpawnerAdversaries();
        }

        /// <summary>
        /// Goes through the heirarchy of the game object of this pool, and tries to
        /// index all enemies and bosses nested under it.
        /// </summary>
        private void IndexGroundAdversaries()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                IndexEnemiesAndBosses(child, 1);
            }
        }

        private void IndexFiniteSpawnerAdversaries()
        {
            if (FiniteSpawner.Instance == null)
            {
                return;
            }

            IndexWavesEnemiesAndBosses(FiniteSpawner.Instance.Waves);     
        }

        private void IndexInfiniteSpawnerAdversaries()
        {
            if (InfiniteSpawner.Instance == null)
            {
                return;
            }

            foreach (StreamWaves streamWaves in InfiniteSpawner.Instance.Streams)
            {
                IndexWavesEnemiesAndBosses(streamWaves.Waves);
            }
        }

        /// <summary>
        /// Extracts the wave data information and uses it to index enemies and bosses based on the wave data type
        /// (which is related to the adversary mover type.)
        /// </summary>
        /// <param name="waves">The waves array that is being inspected to know what adversaries it spawns and
        /// needs pooling.</param>
        private void IndexWavesEnemiesAndBosses(Wave[] waves)
        {
            foreach (Wave wave in waves)
            {
                if (wave.waveData == null)
                {
                    continue;
                }

                WaveData waveData = wave.waveData;
                int numberOfEnemies = wave.waveData.NumberOfEnemiesToSpawn;

                if(waveData is SideSpawnableWaveData sideSpawnableWaveData)
                {
                    for (int i = 0; i < sideSpawnableWaveData.EnemyPrefabs.Length; i++)
                    {
                        if (sideSpawnableWaveData.EnemyPrefabs[i] == null)
                        {
                            Debug.Log("EnemyPool.cs: side spawnable wave named: " + sideSpawnableWaveData.name +
                                " is missing its enemies in the enemy prefabs slots.");

                            continue;
                        }

                        GameObject go = sideSpawnableWaveData.EnemyPrefabs[i].gameObject;
                        IndexEnemiesAndBosses(go, numberOfEnemies);
                    }
                }
                else if(waveData is CurveWaveData curveWaveData)
                {
                    for (int i = 0; i < curveWaveData.EnemyPrefabs.Length; i++)
                    {
                        if(curveWaveData.EnemyPrefabs[i] == null)
                        {
                            Debug.Log("EnemyPool.cs: curve wave named: " + curveWaveData.name + 
                                " is missing its enemies in the enemy prefabs slots.");
                            
                            continue;
                        }
                        
                        GameObject go = curveWaveData.EnemyPrefabs[i].gameObject;
                        IndexEnemiesAndBosses(go, numberOfEnemies);
                    }
                }
                else if(waveData is WaypointWaveData waypointWaveData)
                {
                    for (int i = 0; i < waypointWaveData.EnemyPrefabs.Length; i++)
                    {
                        if (waypointWaveData.EnemyPrefabs[i] == null)
                        {
                            Debug.Log("EnemyPool.cs: waypoint wave named: " + waypointWaveData.name +
                                " is missing its enemies in the enemy prefabs slots.");

                            continue;
                        }

                        GameObject go = waypointWaveData.EnemyPrefabs[i].gameObject;
                        IndexEnemiesAndBosses(go, numberOfEnemies);
                    }
                }
            }
        }

        /// <summary>
        /// Will find whether a game object is an enemy or a boss and will index it accordingly.
        /// </summary>
        /// <param name="go">The game object you wish to index.</param>
        /// <param name="numberOfEnemies">This number is used in finding the maximum number of 
        /// enemies/bosses ever needed so that only the bare minimum is pooled.</param>
        private void IndexEnemiesAndBosses(GameObject go, int numberOfEnemies)
        {
            if (go == null)
            {
                Debug.Log("EnemyPool.cs: When trying to index wave enemies, one of your waves" +
                    " was discovered to have one of its game object slots empty.");

                return;
            }

            // This is just to make sure that your prefab is activated.
            // If it's not set to true here, and your prefab is inactive,
            // it might not process the first activation event on spawning.
            go.SetActive(true);

            if (go.GetComponent<Boss>() != null)
            {
                IndexGoCountsAndBossesSubPhases(go, numberOfEnemies);
            }
            else if (go.GetComponent<Enemy>() != null)
            {
                IndexGoCountsAndEnemies(go, numberOfEnemies);               
            }
            else
            {
                GoCount goCount = new GoCount(go, numberOfEnemies);
                IndexAdversaryGoCount(go.name, goCount);
            }
        }

        /// <summary>
        /// Will index/pool the boss and will also index the boss sub phases so that these sub phases 
        /// can be later accessed and munition/FX/pickups pooled.
        /// </summary>
        /// <param name="go">The boss game object which you wish to index.</param>
        /// <param name="numberOfEnemies">The count value for this game object to pool.</param>
        private void IndexGoCountsAndBossesSubPhases(GameObject go, int numberOfEnemies)
        {            
            Boss boss = go.GetComponent<Boss>();
            GoCount goCount = new GoCount(go, numberOfEnemies);

            // This will index the boss into the adversary go count, the boss is the game object which
            // will actually be spawned/despawned (pooled), boss sub phases are part of it.
            IndexAdversaryGoCount(go.name, goCount);

            // While the boss is what will be pooled and spawned/despawned; it is still needed to index 
            // the boss sub phases (which are technically enemy agents) so that munition/FX/pickups
            // can be later pooled as well.
            foreach (BossPhase bossPhase in boss.BossPhases)
            {
                Enemy[] enemiesInsideOfBossPhase = bossPhase.BossSubPhases;

                foreach (Enemy enemy in enemiesInsideOfBossPhase)
                {
                    enemy.gameObject.layer = ProjectLayers.Enemies;
                    
                    IndexEnemy(enemy.name, enemy);
                }
            }
        }

        /// <summary>
        /// Will index and pool the enemy, the adversary go count holds the list of adversaries that are pool and 
        /// later spawned/despawned. While the enemy by name list holds a list of enemies that is needed for pooling 
        /// other objects such munition, FX and pickups.
        /// </summary>
        /// <param name="go">The game object which has an enemy component and you wish to pool.</param>
        /// <param name="numberOfEnemies">The count value for this game object to pool.</param>
        private void IndexGoCountsAndEnemies(GameObject go, int numberOfEnemies)
        {
            Enemy enemy = go.GetComponent<Enemy>();
            GoCount goCount = new GoCount(go, numberOfEnemies);
           
            IndexAdversaryGoCount(go.name, goCount);
            IndexEnemy(enemy.name, enemy);
        }

        private void IndexEnemy(string enemyName, Enemy enemy)
        {
            if (!EnemyByName.ContainsKey(enemyName))
            {
                EnemyByName.Add(enemyName, enemy);
            }
        }

        /// <summary>
        /// Puts all adversaries in a go count dictionary, this go count dictionary keeps track of how many
        /// times an adverasary will be used so only the bare minimum is pooled
        /// </summary>
        /// <param name="name">The name of the adversary which will be used as a dictionary key.</param>
        /// <param name="goCount">The count value which represents how many copies are needed for it.</param>
        private void IndexAdversaryGoCount(string name, GoCount goCount)
        {           
            try
            {
                adversaryGoCountByName.Add(name, goCount);
            }
            catch (System.ArgumentException)
            {
                adversaryGoCountByName[name].Count += goCount.Count;
            }
        }

        /// <summary>
        /// Goes over the adversaries dictionary and creates a clone for each one, these 
        /// clones are later duplicated if the enemy pool needed to expand.
        /// </summary>
        private void CreateClones()
        {
            foreach (var kvp in adversaryGoCountByName)
            {
                GameObject EnemyInst = CreateClone(kvp.Key, kvp.Value.Prefab);
                GoCloneByName.Add(kvp.Key, EnemyInst);
            }
        }

        /// <summary>
        /// Creates the game objects which make up the enemy pool and are used to spawn/despawn adversaries.
        /// </summary>
        private void CreateGameObjectsQueues()
        {
            foreach (var kvp in adversaryGoCountByName)
            {
                int maxCount;

                if (kvp.Value.Count > poolLimit)
                {
                    maxCount = poolLimit;
                }
                else
                {
                    maxCount = kvp.Value.Count;
                }

                Queue<GameObject> uniqueEnemyPool = new Queue<GameObject>();

                for (int i = 0; i < maxCount; i++)
                {
                    GameObject EnemyInst = CreateClone(kvp.Key, kvp.Value.Prefab);
                    uniqueEnemyPool.Enqueue(EnemyInst);
                }

                GoQueueByName.Add(kvp.Key, uniqueEnemyPool);
            }
        }

        /// <summary>
        /// Creates placeholder game object which are nested under the treadmill (move with the background)
        /// and are used to spawn the ground enemies once they enter the ground enemies spawning field.
        /// </summary>
        private void CreatePlaceholders()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;

                if (!IsEnemyOrBoss(child))
                {
                    Debug.Log("EnemyPool.cs: Under the enemy pool hierarchy you have a nested a game object named: " +
                        child.name + " which is neither an enemy nor a boss, please only nest enemies or bosses under" +
                        " the enemy pool so that they can be pooled as ground adversaries.");
                    
                    continue;
                }

                GameObject PlaceholderInstance = CreateGroundAdversaryPlaceholder(child);

                if (!CanFindPlaceholdersTreadmillHierarchy())
                {
                    return;
                }

                PlaceholderInstance.transform.parent = Treadmill.Instance.PlaceholdersOnTreadmillHierarchy;
            }
        }

        /// <summary>
        /// Checks if game object is either an enemy or a boss (An adversary)
        /// </summary>
        /// <param name="go">The game object to check.</param>
        /// <returns>true if the game object is an enemy or a boss and flase if it isn't.</returns>
        private static bool IsEnemyOrBoss(GameObject go)
        {
            Enemy enemy = go.GetComponent<Enemy>();

            if (enemy != null)
            {
                return true;
            }

            Boss boss = go.GetComponent<Boss>();

            if (boss != null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates a game object with a collider and a rigid body (placeholder) that will spawn a ground adversary 
        /// (enemy/boss) when it enters the ground enemies spawning field.
        /// </summary>
        /// <param name="go">The adversary that will be transformed into a placeholder.</param>
        /// <returns>The newly created placeholder.</returns>
        private static GameObject CreateGroundAdversaryPlaceholder(GameObject go)
        {
            GameObject PlaceholderInstance = new GameObject(go.name);
            PlaceholderInstance.transform.position = go.transform.position;

            PlaceholderInstance.AddComponent<CircleCollider2D>().isTrigger = true;
            PlaceholderInstance.AddComponent<Rigidbody2D>().isKinematic = true;

            PlaceholderInstance.gameObject.layer = ProjectLayers.EnemyPlaceholders;
            return PlaceholderInstance;
        }

        private static bool CanFindPlaceholdersTreadmillHierarchy()
        {
            if (Treadmill.Instance == null)
            {
                Debug.Log("EnemyPool.cs: Was unable to find scrolling background instance, " +
                    "ground enemies placeholders were not created.");
                return false;
            }

            if (Treadmill.Instance.PlaceholdersOnTreadmillHierarchy == null)
            {
                Debug.Log("EnemyPool.cs: Was unable to find placeholders treadmill heirarchy, " +
                    "ground enemies placeholders were not created.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// After the user placed ground enemies have been indexed and placeholders have been created 
        /// for them to trigger spawning them from the pool, the user placed ground enemies are no longer 
        /// needed and this method "cleans" them from the scene.
        /// </summary>
        private void DestroyUserPlacedEnemies()
        {
            for (int i = (transform.childCount - 1); i >= 0; i--)
            {
                GameObject child = transform.GetChild(i).gameObject;
                Destroy(child);
            }
        }

        public override GameObject Spawn(string name)
        {
            Queue<GameObject> goQueue;

            try
            {
                goQueue = GoQueueByName[name];
            }
            catch (KeyNotFoundException)
            {
                Debug.Log("PoolBase.cs: The game object: " + name +
                    " that you are trying to spawn can't find it's GoQueue dictionary key.");

                return null;
            }

            if (goQueue.Count == 0)
            {
                ExpandPool(name);
            }

            GameObject go = goQueue.Dequeue();

            go.SetActive(true);

            NumberOfActiveEnemies++;

            return go;
        }

        public override void Despawn(GameObject go)
        {
            base.Despawn(go);

            NumberOfActiveEnemies--;

            // This override is needed incase the adversary was a ground enemy which was nested 
            // under the treadmill when spawned. this will put it back to its proper heirarchy.
            go.transform.parent = PoolHierarchy;
        }

        private void RaiseOnAllEnemiesDespawned()
        {            
            OnAllEnemiesDespawned?.Invoke(null);
        }
    }
}