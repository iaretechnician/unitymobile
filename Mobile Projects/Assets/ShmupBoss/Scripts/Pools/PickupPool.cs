using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Pools all the possible drop items the enemies spawn when eliminated.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Pools/Pickup Pool")]
    public class PickupPool : PoolBase
    {
        public static PickupPool Instance; 

        protected override int Layer
        {
            get
            {
                return ProjectLayers.Pickups;
            }
        }

        protected void Awake()
        {
            Instance = this;            
            Initializer.Instance.SubscribeToStage(Pool, 2);
        }

        protected override void CreateHierarchy()
        {
            PoolHierarchy = new GameObject("----- Pooled Pickups -----").transform;
        }

        protected override void Pool()
        {
            CreateHierarchy();
            IndexPickupsFromEnemyPool();
        }

        private void IndexPickupsFromEnemyPool()
        {
            if (EnemyPool.Instance == null)
            {
                return;
            }

            if (EnemyPool.Instance.EnemyByName.Count == 0)
            {
                return;
            }

            foreach (var kvp in EnemyPool.Instance.EnemyByName)
            {
                Enemy enemy = kvp.Value;

                Drop[] enemyDrops = enemy.Drops;

                for (int i = 0; i < enemyDrops.Length; i++)
                {
                    if (enemyDrops[i].PickupPrefab == null)
                    {
                        Debug.Log("PickupPool.cs: Enemy named: " + enemy.name + " contains a " +
                            "drop slot item numbered: " + i + " which is empty.");
                        
                        continue;
                    }

                    string pickupName = enemyDrops[i].PickupPrefab.name;
                    GameObject pickupGo = enemyDrops[i].PickupPrefab.gameObject;
                    Pickup pickup = pickupGo.GetComponent<Pickup>();
                    
                    if (pickup == null)
                    {
                        Debug.Log("PickupPool.cs: Inside of Enemy named: " + enemy.name + " You have somehow " +
                            " placed a pickup prefab named: " + pickupName + " which isn't a pickup? Please " +
                            "do make sure that your pickup item has a pickup script added to it.");
                      
                        continue;
                    }
              
                    int poolLimit = pickup.PoolLimit;

                    CreateAndIndexCloneAndQueue(pickupName, pickupGo, poolLimit);
                }
            }
        }

        /// <summary>
        /// Creates clones of the pickup object and indexes them in a dictionary so they can be extracted when 
        /// the pool needs to expand. It also creates and propagates the game object queue which consitutes 
        /// the pool. 
        /// </summary>
        /// <param name="pickupName">The name of the pickup you wish to pool.</param>
        /// <param name="pickupGo">The gameobject of the pickup you wish to pool.</param>
        /// <param name="poolLimit">How many clones do you want to create and add to the game objects queue.</param>
        private void CreateAndIndexCloneAndQueue(string pickupName, GameObject pickupGo, int poolLimit)
        {
            if (!GoCloneByName.ContainsKey(pickupName))
            {
                GameObject pickupGoClone = CreateClone(pickupName, pickupGo);
                GoCloneByName.Add(pickupName, pickupGoClone);

                CreateAndIndexQueue(pickupName, pickupGo, poolLimit);
            }
        }

        private void CreateAndIndexQueue(string pickupName, GameObject pickupGo, int poolLimit)
        {
            Queue<GameObject> uniquePickupPool = new Queue<GameObject>();

            for (int j = 0; j < poolLimit; j++)
            {
                GameObject PickupInst = CreateClone(pickupName, pickupGo);
                uniquePickupPool.Enqueue(PickupInst);
            }

            if (!GoQueueByName.ContainsKey(pickupName))
            {
                GoQueueByName.Add(pickupName, uniquePickupPool);
            }
        }
    }
}