using System.Collections.Generic;
using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Creates layers of backgrounds which are stacked in accordance to their Z depth index, 
    /// each with a different speed and ending.<br></br>
    /// The very first background of the first layer potentially determines the dimensions which are used 
    /// for finding different fields such as the viewfield or playfield.<br></br>
    /// Please note also that in selecting backgrounds for your layer, you cannot choose anything with 
    /// identical names or it will be considered to be the same background.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Background/Scrolling Background")]
    public class ScrollingBackground : PoolBase
    {
        public static ScrollingBackground Instance;

        /// <summary>
        /// How many backgrounds are visible simultaneously when the game is played, this value is dependent 
        /// on your viewfield and background dimensions.<br></br>
        /// If still confused what this value does, just play the scene then change the number and play again.
        /// </summary>
        [Tooltip("How many backgrounds are visible simultaneously when the game is played, the value you should input " +
            "is dependent on your viewfield and background dimensions. If still confused what this value does, " +
            "just play the scene then change the number and play again.")]
        [SerializeField]
        private int activeBackgroundsLimit = 3;

        /// <summary>
        /// The background layers that make up the level, please make sure that your main layer, is the first 
        /// layer. This first layer speed will determine the speed of ground units and its size will also determine
        /// the backgorund field which depending on your settings could dteremine your playfield, viewfield and 
        /// other fields.<br></br>
        /// Please note that you cannot use two background which have the same name or otherwise they will be considered 
        /// as the same one.
        /// </summary>
        [Space]
        [Tooltip("The background layers that make up the level, please make sure that your main layer is the " +
            "first layer. This first layer speed will determine the speed of ground units and its size will also determine the " +
            "background field which depending on your settings could determine your playfield, viewfield and other fields. Please " +
            "note that you cannot use two backgrounds which have the same name or otherwise they will be considered as the same one.")]
        [SerializeField]
        private BackgroundLayer[] backgroundLayers;
        public BackgroundLayer[] BackgroundLayers
        {
            get
            {
                return backgroundLayers;
            }
        }

        protected override int Layer
        {
            get
            {
                return ProjectLayers.Backgrounds;
            }
        }

        /// <summary>
        /// Categorizes backgrounds by name and how many times they 
        /// are repeated to know how many times to pool them.<br></br>
        /// Please note that you cannot use two backgrounds with the same name, 
        /// otherwise they will be recognized as the same one.
        /// </summary>
        private Dictionary<string, GoCount> backgroundGoCountByName = new Dictionary<string, GoCount>();

        /// <summary>
        /// Categorizes the background layers by name, this name is generated from the layer number by adding 
        /// a 2 digit number to the word layer, for this reason no more than 99 layers are allowed.
        /// </summary>
        private Dictionary<string, BackgroundLayer> backgroundLayerByName = new Dictionary<string, BackgroundLayer>();               

        [HideInInspector]
        [SerializeField]
        private Transform previewBackgrounds;

        public Transform BackgroundsOnTreadmillHeirarchy
        {
            get;
            private set;
        }
        
        /// <summary>
        /// The width of the first background in the first background layer.
        /// </summary>
        public float MainBackgroundPrefabWidth
        {
            get;
            private set;
        }

        /// <summary>
        /// The height of the first background in the first background layer.
        /// </summary>
        public float MainBackgroundPrefabHeight
        {
            get;
            private set;
        }

        /// <summary>
        /// The dimensions of the first background in the first background layer.
        /// </summary>
        public Vector2 MainBackgroundPrefabDimensions
        {
            get;
            private set;
        }

        protected void Awake()
        {
            Instance = this;

            DestroyPreviewBackgrounds();
            CacheBackgroundDimensions();

            Initializer.Instance.SubscribeToStage(Pool, 2);
        }

        #region Validate
        private void OnValidate()
        {
            Instance = this;

            if (!CanProcessFirstBackground())
            {
                return;
            }

            VerifyActiveBackgroundsLimit();
            VerifyCountValueNotZero();
            CacheBackgroundDimensions();
            VerifyYouHaveEnoughBackgroundCountValues();

            // If the final image is repeated infinitely, make sure it has
            // enough count value that equals the active backgrounds limit.
            if (backgroundLayers[0].AfterSequenceEnds == SequenceEndOptions.LoopFinalImage)
            {
                VerifyLastBGCountValueIsEnough();
            }
        }

        private void VerifyActiveBackgroundsLimit()
        {
            if(activeBackgroundsLimit < ProjectConstants.MinActiveBackgroundsLimit)
            {
                activeBackgroundsLimit = ProjectConstants.MinActiveBackgroundsLimit;
            }
        }

        protected void VerifyCountValueNotZero()
        {
            for (int i = 0; i < backgroundLayers[0].Backgrounds.Length; i++)
            {
                if (backgroundLayers[0].Backgrounds[i].Count < 1)
                {
                    backgroundLayers[0].Backgrounds[i].Count = 1;
                }
            }
        }

        public void CacheBackgroundDimensions()
        {
            if (!CanProcessFirstBackground())
            {
                return;
            }

            MainBackgroundPrefabWidth = Dimensions.FindWidth(backgroundLayers[0].Backgrounds[0].Prefab);
            MainBackgroundPrefabHeight = Dimensions.FindHeight(backgroundLayers[0].Backgrounds[0].Prefab);

            MainBackgroundPrefabDimensions = new Vector2(MainBackgroundPrefabWidth, MainBackgroundPrefabHeight);
        }

        /// <summary>
        /// Makes sure the backgorunds and their count values will at least be sufficient to generate 
        /// backgrounds that can fulfil the limit of active backgrounds.
        /// </summary>
        private void VerifyYouHaveEnoughBackgroundCountValues()
        {
            foreach(BackgroundLayer bl in backgroundLayers)
            {
                int totalBackgroundCount = 0;

                for(int i =0; i < bl.Backgrounds.Length; i++)
                {
                    totalBackgroundCount += bl.Backgrounds[i].Count;
                }

                if(totalBackgroundCount < activeBackgroundsLimit)
                {
                    int missingBackgrounds = activeBackgroundsLimit - totalBackgroundCount;

                    bl.Backgrounds[bl.Backgrounds.Length - 1].Count += missingBackgrounds;
                }
            }
        }

        private void VerifyLastBGCountValueIsEnough()
        {
            int lastBackgroundIndex = backgroundLayers[0].Backgrounds.Length - 1;

            if (backgroundLayers[0].Backgrounds[lastBackgroundIndex].Count < activeBackgroundsLimit)
            {
                backgroundLayers[0].Backgrounds[lastBackgroundIndex].Count = activeBackgroundsLimit;
            }
        }
        #endregion

        protected override void Pool()
        {
            CreateHierarchy();
            IndexLayersAndBackgrounds();
            PoolBackgrounds();
            ActivateInitialBackgrounds();
        }

        protected override void CreateHierarchy()
        {
            PoolHierarchy = new GameObject("----- Pooled Backgrounds -----").transform;
            BackgroundsOnTreadmillHeirarchy = new GameObject("----- Backgrounds On Treadmill -----").transform;
        }

        private void IndexLayersAndBackgrounds()
        {
            for(int i = 0; i <  backgroundLayers.Length; i++)
            {
                string layerName = StringTools.CombineNameAndNumber(ProjectConstants.LayerPrefix, i);

                PrepareAndIndexLayers(i, layerName);
                IndexBackgrounds(i, layerName);
            }
        }

        private void PrepareAndIndexLayers(int layerIndex, string layerName)
        {
            backgroundLayers[layerIndex].PoolTransform = CreatePoolTransform(layerName);
            backgroundLayers[layerIndex].TreadmillTransform = CreateTransformOnTreadmill(layerIndex, layerName);
            backgroundLayers[layerIndex].BackgroundsSequence = CreateBackgroundsSequence(layerIndex, layerName);
            backgroundLayers[layerIndex].StopAtFinalImageCounter = activeBackgroundsLimit;
            backgroundLayers[layerIndex].CanContinueSpawning = true;

            backgroundLayerByName.Add(layerName, backgroundLayers[layerIndex]);
        }

        private Transform CreatePoolTransform(string layerName)
        {
            Transform layerPoolHierarchy = new GameObject(layerName).transform;
            layerPoolHierarchy.parent = PoolHierarchy;
            return layerPoolHierarchy;
        }

        private Transform CreateTransformOnTreadmill(int layerIndex, string layerName)
        {
            Transform layerOnTreadmillHierarchy = new GameObject(layerName).transform;
            layerOnTreadmillHierarchy.position = Vector3.back * (backgroundLayers[layerIndex].DepthIndex * Level.SpaceBetweenIndices);
            layerOnTreadmillHierarchy.parent = BackgroundsOnTreadmillHeirarchy;
            return layerOnTreadmillHierarchy;
        }

        private List<string> CreateBackgroundsSequence(int layerIndex, string layerName)
        {
            GoCount[] backgrounds = backgroundLayers[layerIndex].Backgrounds;

            List<string> backgroundsSequence = new List<string>();

            for (int i = 0; i < backgrounds.Length; i++)
            {
                if (backgrounds[i].Prefab == null)
                {
                    Debug.Log("Background entry number: " + i + " in layer number: " + layerIndex + ". is missing its prefab.");
                    continue;
                }

                for (int j = 0; j < backgrounds[i].Count; j++)
                {
                    string backgroundName = layerName + backgrounds[i].Prefab.name;
                    backgroundsSequence.Add(backgroundName);
                }
            }

            return backgroundsSequence;
        }

        private void IndexBackgrounds(int layerIndex, string layerName)
        {
            for (int i = 0; i < backgroundLayers[layerIndex].Backgrounds.Length; i++)
            {
                string newBackgroundName = layerName + backgroundLayers[layerIndex].Backgrounds[i].Prefab.name;

                try
                {
                    backgroundGoCountByName.Add(newBackgroundName, backgroundLayers[layerIndex].Backgrounds[i]);
                }
                catch (System.ArgumentException)
                {
                    backgroundGoCountByName[newBackgroundName].Count += backgroundLayers[layerIndex].Backgrounds[i].Count;
                }
            }
        }

        private void PoolBackgrounds()
        {
            foreach(var kvp in backgroundGoCountByName)
            {
                int maxCount;

                if (kvp.Value.Count > activeBackgroundsLimit)
                {
                    maxCount = activeBackgroundsLimit;
                }
                else
                {
                    maxCount = kvp.Value.Count;
                }

                string layerName = kvp.Key.Remove(ProjectConstants.LayerPrefixLength);

                if (!backgroundLayerByName.ContainsKey(layerName))
                {
                    Debug.Log("ScrollingBackground.cs: Can't pool backgrounds in queue, unable to find layer: "
                        + layerName);

                    return;
                }

                BackgroundLayer backgroundLayer = backgroundLayerByName[layerName];

                Queue<GameObject> GameObjectPool = new Queue<GameObject>();

                for (int i = 0; i < maxCount; i++)
                {
                    GameObject go = Instantiate(kvp.Value.Prefab, backgroundLayer.PoolTransform);
                    go.layer = Layer;
                    go.name = kvp.Key;
                    go.SetActive(false);
                    GameObjectPool.Enqueue(go);

                    if(i == 0)
                    {
                        // Adds to clones dictionary
                        GameObject goClone = Instantiate(kvp.Value.Prefab, backgroundLayer.PoolTransform);
                        goClone.layer = Layer;
                        goClone.name = kvp.Key;
                        goClone.SetActive(false);
                        GameObjectPool.Enqueue(goClone);
                        GoCloneByName.Add(go.name, goClone);
                    }
                }

                GoQueueByName.Add(kvp.Key, GameObjectPool);
            }
        }

        private void ActivateInitialBackgrounds()
        {
            foreach(var kvp in backgroundLayerByName)
            {
                BackgroundLayer backgroundLayer = kvp.Value;
                List<string> backgroundsSequence = new List<string>();
                backgroundsSequence = backgroundLayer.BackgroundsSequence;               

                for (int i = 0; i < activeBackgroundsLimit; i++)
                {
                    GameObject ActiveBackground = Spawn(backgroundsSequence[i]);

                    if (Level.IsVertical)
                    {
                        ActiveBackground.transform.position = new Vector3(
                            0.0f,
                            i * MainBackgroundPrefabHeight,
                            backgroundLayer.TreadmillTransform.position.z);
                    }
                    else if (Level.IsHorizontal)
                    {
                        ActiveBackground.transform.position = new Vector3(
                            i * MainBackgroundPrefabWidth,
                            0.0f,
                            backgroundLayer.TreadmillTransform.position.z);
                    }

                    kvp.Value.LastBackground = ActiveBackground;
                    backgroundLayer.CurrentSequenceIndex++;
                }
            }           
        }

        public void ReplaceBackground(GameObject go)
        {
            if (go.layer == Layer)
            {
                float zPos = go.transform.position.z;

                Despawn(go);

                string layerName = StringTools.GetLayerName(go.name);
                BackgroundLayer backgroundLayer = backgroundLayerByName[layerName];

                BackgroundLayerEndAction(backgroundLayer);

                if (!backgroundLayer.CanContinueSpawning)
                {
                    return;
                }

                GameObject PulledBG = Spawn(backgroundLayer.BackgroundsSequence[backgroundLayer.CurrentSequenceIndex]);

                if (Level.IsVertical)
                {
                    PulledBG.transform.position = new Vector3(0f, (backgroundLayer.LastBackground.transform.position.y + MainBackgroundPrefabHeight), zPos);
                }
                else if (Level.IsHorizontal)
                {
                    PulledBG.transform.position = new Vector3((backgroundLayer.LastBackground.transform.position.x + MainBackgroundPrefabWidth), 0f, zPos);
                }

                backgroundLayer.LastBackground = PulledBG;
                backgroundLayer.CurrentSequenceIndex++;
            }
        }

        private void BackgroundLayerEndAction(BackgroundLayer backgroundLayer)
        {
            if (backgroundLayer.CurrentSequenceIndex >= backgroundLayer.BackgroundsSequence.Count)
            {
                switch (backgroundLayer.AfterSequenceEnds)
                {
                    case SequenceEndOptions.LoopFinalImage:
                        {
                            backgroundLayer.CurrentSequenceIndex = backgroundLayer.BackgroundsSequence.Count - 1;
                            break;
                        }

                    case SequenceEndOptions.LoopSequence:
                        {
                            backgroundLayer.CurrentSequenceIndex = 0;
                            break;
                        }

                    case SequenceEndOptions.StopAtFinalImage:
                        {
                            backgroundLayer.CanContinueSpawning = false;

                            if (backgroundLayer.StopAtFinalImageCounter > 2)
                            {
                                backgroundLayer.StopAtFinalImageCounter--;
                            }
                            else
                            {
                                backgroundLayer.Speed = 0.0f;
                            }

                            break;
                        }

                    case SequenceEndOptions.FinishSequence:
                        {
                            backgroundLayer.CanContinueSpawning = false;
                            break;
                        }
                }
            }
        }

        public override GameObject Spawn(string goName)
        {
            if (!GoQueueByName.ContainsKey(goName))
            {
                Debug.Log("Background: " + goName + " that you are trying to pull from the background pool cannot be found.");
                return null;
            }

            GameObject pulledGO = GoQueueByName[goName].Dequeue();
            pulledGO.SetActive(true);

            string layerName = goName.Remove(ProjectConstants.LayerPrefixLength);
            BackgroundLayer backgroundLayer = backgroundLayerByName[layerName];
            pulledGO.transform.parent = backgroundLayer.TreadmillTransform;

            return pulledGO;
        }

        public override void Despawn(GameObject go)
        {
            go.SetActive(false);

            try
            {
                GoQueueByName[go.name].Enqueue(go);
            }
            catch (KeyNotFoundException)
            {
                Debug.Log("The background: " + go.name +
                    " you are trying to despawn can't find its GOQueue dictionary key.");
            }

            string layerName = go.name.Remove(ProjectConstants.LayerPrefixLength);

            if (!backgroundLayerByName.ContainsKey(layerName))
            {
                Debug.Log("Can't find the background layer, " + go.name + " belongs to, unable to find layer:  " + layerName);
                return;
            }

            BackgroundLayer backgroundLayer = backgroundLayerByName[layerName];

            Transform poolSubHierarchy = backgroundLayer.PoolTransform;

            if (poolSubHierarchy != null)
            {
                go.transform.parent = poolSubHierarchy;
            }
        }

        private bool CanProcessFirstBackground()
        {
            if(backgroundLayers == null)
            {
                return false;
            }
            
            if (backgroundLayers.Length < 1)
            {
                return false;
            }

            if (backgroundLayers[0].Backgrounds.Length < 1)
            {
                return false;
            }

            if (backgroundLayers[0].Backgrounds[0].Prefab == null)
            {
                return false;
            }

            return true;
        }

        public void CreatePreviewBackgrounds()
        {
            previewBackgrounds = new GameObject("----- Preview Backgrounds -----").transform;
            previewBackgrounds.parent = transform;

            for (int i = 0; i < backgroundLayers.Length; i++)
            {
                int backgroundIndex = 0;

                for (int j = 0; j < backgroundLayers[i].Backgrounds.Length; j++)
                {
                    for (int k = 0; k < backgroundLayers[i].Backgrounds[j].Count; k++)
                    {
                        if (backgroundLayers[i].Backgrounds[j].Prefab == null)
                        {
                            Debug.Log("ScrollingBackground.cs: one or more of your background entries is missing " +
                                "its background prefab.");

                            return;
                        }

                        GameObject previewBG = Instantiate(backgroundLayers[i].Backgrounds[j].Prefab);

                        if (Level.IsVertical)
                        {
                            float zDepth = backgroundLayers[i].DepthIndex * -Level.SpaceBetweenIndices;

                            previewBG.transform.position = new Vector3(0f, (backgroundIndex * MainBackgroundPrefabHeight), zDepth);
                        }
                        else if (Level.IsHorizontal)
                        {
                            float zDepth = backgroundLayers[i].DepthIndex * -Level.SpaceBetweenIndices;

                            previewBG.transform.position = new Vector3((backgroundIndex * MainBackgroundPrefabWidth), 0f, zDepth);
                        }
                        previewBG.transform.parent = previewBackgrounds.transform;

                        backgroundIndex++;
                    }
                }
            }
        }

        public void DestroyPreviewBackgroundsInEditor()
        {
            if (previewBackgrounds != null)
            {
                DestroyImmediate(previewBackgrounds.gameObject);
            }
        }

        public void DestroyPreviewBackgrounds()
        {
            if (previewBackgrounds != null)
            {
                Destroy(previewBackgrounds.gameObject);
            }
        }
    }
}