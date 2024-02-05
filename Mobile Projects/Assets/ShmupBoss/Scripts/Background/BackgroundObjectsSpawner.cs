using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Pools and spawns objects that will be used as background decorations with many parameters 
    /// to randomize how they are spawned and move.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Background/Background Objects Spawner")]
    public class BackgroundObjectsSpawner : PoolBase
    {
        public static BackgroundObjectsSpawner Instance;
        
        public BackgroundObjectsLayer[] RandomObjectsLayers;
        
        /// <summary>
        /// Background objects are nested under a numbered layer name to help orgnize things in the scene.<br></br> 
        /// This dictionary will index those layers transforms by the numbered name of the layer.
        /// </summary>
        private Dictionary<string, Transform> layerHierarchyByName = new Dictionary<string, Transform>();

        protected override int Layer
        {
            get
            {
                return ProjectLayers.BackgroundObjects;
            }
        }

        private void OnValidate()
        {
            if (RandomObjectsLayers == null || RandomObjectsLayers.Length <= 0)
            {
                return;
            }

            for (int i = 0; i < RandomObjectsLayers.Length; i++)
            {
                if (RandomObjectsLayers[i].PoolLimit <= 0)
                {
                    RandomObjectsLayers[i].PoolLimit = 1;
                }
                
                if (RandomObjectsLayers[i].Scale == Vector3.zero)
                {
                    RandomObjectsLayers[i].Scale = Vector3.one;
                }

                if (RandomObjectsLayers[i].MinRandomScale == 0.0f && RandomObjectsLayers[i].MaxRandomScale == 0.0f)
                {
                    RandomObjectsLayers[i].MinRandomScale = 1.0f;
                    RandomObjectsLayers[i].MaxRandomScale = 1.0f;
                }
            }
        }

        protected void Awake()
        {
            Instance = this;

            CreateHierarchy();
            Pool();

            Initializer.Instance.SubscribeToStage(SpawnBackgroundObjectsLayers, 4);
        }

        protected override void CreateHierarchy()
        {
            PoolHierarchy = new GameObject("----- Pooled Background Objects -----").transform;
        }

        private void SpawnBackgroundObjectsLayers()
        {
            for (int i = 0; i < RandomObjectsLayers.Length; i++)
            {
                string layerName = StringTools.CombineNameAndNumber(ProjectConstants.LayerPrefix, i);

                StartCoroutine(SpawnRandomObjectsLayer(RandomObjectsLayers[i], layerName));
            }
        }

        protected override void Pool()
        {
            for (int i = 0; i < RandomObjectsLayers.Length; i++)
            {
                string layerName = StringTools.CombineNameAndNumber(ProjectConstants.LayerPrefix, i);

                Transform RandomObjectsLayerHierarchy = CreateLayerHierarchy(i, layerName);

                layerHierarchyByName.Add(layerName, RandomObjectsLayerHierarchy);

                PoolLayer(RandomObjectsLayers[i], layerName);
            }
        }

        /// <summary>
        /// Creates and injects (saves) the layer hierarchy which all background objects related 
        /// to that layer will be nested under.
        /// </summary>
        /// <param name="index">The number of the layer in the array of background objects layer.</param>
        /// <param name="layerName">The prefix of the created layer hierarchy which the layer number 
        /// will succeed.</param>
        /// <returns></returns>
        private Transform CreateLayerHierarchy(int index, string layerName)
        {
            Transform RandomObjectsLayerHierarchy = new GameObject(layerName).transform;
            RandomObjectsLayerHierarchy.parent = PoolHierarchy;
            RandomObjectsLayers[index].BackgroundObjectsHeirarchy = RandomObjectsLayerHierarchy;
            return RandomObjectsLayerHierarchy;
        }

        private void PoolLayer(BackgroundObjectsLayer bol, string layerName)
        {            
            for (int i = 0; i < bol.Prefabs.Length; i++)
            {
                string prefabName = layerName + bol.Prefabs[i].name;

                Queue<GameObject> goQueue = new Queue<GameObject>();

                for (int j = 0; j < bol.PoolLimit; j++)
                {
                    GameObject go = Instantiate(bol.Prefabs[i]);
                    go.name = prefabName;
                    go.layer = Layer;                 
                    go.transform.parent = bol.BackgroundObjectsHeirarchy;
                    go.SetActive(false);
                    go.AddComponent<MoverRotator>();

                    if (j == 0)
                    {
                        AddToClonesDictionary(layerName, go, bol);
                    }

                    goQueue.Enqueue(go);
                }

                try
                {
                    GoQueueByName.Add(prefabName, goQueue);
                }
                catch (System.ArgumentException)
                {
                    Debug.Log("BackgroundObjectsSpawner.cs: You seem to have added the same" +
                        " object in a background objects spawner layer twice, double check your " +
                        "prefabs list and their names in your background objects spawner layers.");
                }
            }
        }

        private void AddToClonesDictionary(string layerName, GameObject go, BackgroundObjectsLayer bol)
        {
            GameObject cloneGO = Instantiate(go);
            cloneGO.name = go.name;

            Transform theParent = layerHierarchyByName[layerName];
            cloneGO.transform.parent = theParent;

            try
            {
                GoCloneByName.Add(go.name, cloneGO);
            }
            catch(System.ArgumentException)
            {
                Debug.Log("BackgroundObjectsSpawner.cs: You seem to have added the same" +
                    " object in a background objects spawner layer twice, double check" +
                    " your prefabs list in your background objects spawner layers.");
            }
        }

        private IEnumerator SpawnRandomObjectsLayer(BackgroundObjectsLayer rbol, string layerName)
        {
            yield return new WaitForSeconds(rbol.WaitTimeBeforeFirstSpawn);

            int prefabToSpawnIndex = -1;
            int numberOfSpawnedObjects = 0;

            while (rbol.IsSpawningInfinitely || numberOfSpawnedObjects <= rbol.SpawnCount)
            {              
                if (rbol.IsSpawningInRandomOrder)
                {
                    prefabToSpawnIndex = Random.Range(0, rbol.Prefabs.Length);
                }
                else
                {
                    if(prefabToSpawnIndex < rbol.Prefabs.Length-1)
                    {
                        prefabToSpawnIndex++;
                    }
                    else
                    {
                        prefabToSpawnIndex = 0;
                    }                   
                }

                string goToPullName = layerName + rbol.Prefabs[prefabToSpawnIndex].name;

                SpawnRandomObject(rbol, goToPullName);
                numberOfSpawnedObjects++;

                float waitTime;

                if (rbol.IsRandomWaitTime)
                {
                    waitTime = Random.Range(rbol.MinRandomTime, rbol.MaxRandomTime);
                }
                else
                {
                    waitTime = rbol.WaitTimeBetweenSpawns;
                }

                yield return new WaitForSeconds(waitTime);
            }           
        }

        private void SpawnRandomObject(BackgroundObjectsLayer rbol, string goName)
        {
            GameObject spawnedObject = Spawn(goName);

            RectSide spawnSide = SpawnSides.FindRectSideFromRectSideRandom(rbol.SpawnSide);
            spawnedObject.transform.position = SpawnSides.FindSpawnPosition(rbol, spawnSide);
            spawnedObject.transform.localRotation = Quaternion.Euler(rbol.NewRotation);

            MoverRotator moverRotator = spawnedObject.GetComponent<MoverRotator>();

            if (moverRotator == null)
            {
                Debug.Log("BackgroundObjectsSpawner.cs: Unable to find MoverRotator component for background object.");

                return;
            }

            moverRotator.SpawnSide = spawnSide;

            if (rbol.IsRandomSpeed)
            {
                moverRotator.Speed = Random.Range(rbol.MinRandomSpeed, rbol.MaxRandomSpeed);
            }
            else
            {
                moverRotator.Speed = rbol.Speed;
            }

            if (rbol.IsRandomRotationSpeed)
            {
                moverRotator.XRotationSpeed = Random.Range(rbol.MinRandomRotationSpeed, rbol.MaxRandomRotationSpeed);
                moverRotator.YRotationSpeed = Random.Range(rbol.MinRandomRotationSpeed, rbol.MaxRandomRotationSpeed);
                moverRotator.ZRotationSpeed = Random.Range(rbol.MinRandomRotationSpeed, rbol.MaxRandomRotationSpeed);
            }
            else
            {
                moverRotator.XRotationSpeed = rbol.XRotationSpeed;
                moverRotator.YRotationSpeed = rbol.YRotationSpeed;
                moverRotator.ZRotationSpeed = rbol.ZRotationSpeed;
            }

            if (!rbol.IsRandomScale)
            {
                spawnedObject.transform.localScale = rbol.Scale;
            }
            else
            {
                float randomScaleValue = Random.Range(rbol.MinRandomScale, rbol.MaxRandomScale);
                spawnedObject.transform.localScale = Vector3.one * randomScaleValue;
            }
        }

        /// <summary>
        /// Creates a clone that will be used when the pool needs to expand.<br></br>
        /// This method is different from the base class because the background objects spawner uses a numbered
        /// layer for orgnization in the scene which is embeded in the object name, this number is then extracted 
        /// to know under which layer the background object will be spawned.
        /// </summary>
        /// <param name="name">Name of the clone.</param>
        /// <param name="go">The game object to clone.</param>
        /// <returns></returns>
        protected override GameObject CreateClone(string name, GameObject go)
        {
            GameObject goClone = Instantiate(go);
            goClone.name = name;
            goClone.layer = Layer;

            string layerName = name.Remove(ProjectConstants.LayerPrefixLength);
            Transform theParent = layerHierarchyByName[layerName];
            goClone.transform.parent = theParent;

            goClone.SetActive(false);

            return goClone;
        }
    }
}