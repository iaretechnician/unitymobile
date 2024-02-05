using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Moves all the background layers in the scrolling background component.<br></br> 
    /// In addition to that it is also responsible for moving the ground units and the movement of 
    /// spawned background objects are dependent on the speed multiplier in this script.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Background/Treadmill")]
    [RequireComponent(typeof(ScrollingBackground))]
    public class Treadmill : MonoBehaviour
    {
        public static Treadmill Instance;
        
        /// <summary>
        /// These scrolling background layers will be moved by the treadmill.<br></br> 
        /// It is also very important to note that the first layer of the scrolling backgorund, 
        /// layer 0 determines the speed of ground units.
        /// </summary>
        private ScrollingBackground scrollingBackground;

        /// <summary>
        /// The cached background layers from the scrolling background component.
        /// </summary>
        private BackgroundLayer[] backgroundLayers;

        /// <summary>
        /// The speed of the very first layer of the scrolling backgorund, 
        /// this speed is also pivotal in moving the ground units.
        /// </summary>
        private float mainBackgroundSpeed = 1.0f;

        /// <summary>
        /// This multiplier will change the speed of anything that is dependent on the treadmill.<br></br>
        /// This include all of the scrolling bakground layers, the spawned background objects, and ground units.
        /// </summary>
        [Space]
        [Tooltip("This multiplier will change the speed of anything that is dependent on the treadmill. " +
            "This includes all of the scrolling background layers, the spawned background objects, and ground units.")]
        [SerializeField]
        private float speedMultiplier = 1.0f;
        public float SpeedMultiplier
        {
            get
            {
                return speedMultiplier;
            }
        }

        private bool canProcessBackgroundLayers = true;

        /// <summary>
        /// This transform is used for the placeholders which are representations of the ground units 
        /// which will be spawned when they enter the ground units spawning field.
        /// </summary>
        public Transform PlaceholdersOnTreadmillHierarchy
        {
            get;
            private set;
        }

        public Transform GroundEnemiesOnTreadmillHierarchy
        {
            get;
            private set;
        }

        private void Awake()
        {
            Instance = this;

            CacheComponents();
            CreateHierarchy();
        }

        private void CacheComponents()
        {
            canProcessBackgroundLayers = true;
            scrollingBackground = GetComponent<ScrollingBackground>();

            if(scrollingBackground == null)
            {
                Debug.Log("Treadmill.cs: was unable to find the scrolling background script, the treadmill " +
                    "script must be assigned to the same object which contains the scrolling background.");
                
                return;
            }

            backgroundLayers = scrollingBackground.BackgroundLayers;

            if (backgroundLayers == null || backgroundLayers.Length < 1)
            {
                canProcessBackgroundLayers = false;

                Debug.Log("Treadmill.cs: your scrolling background script needs to have the first " +
                    "layer set so that the treadmill can use its settings.");

                return;
            }

            mainBackgroundSpeed = backgroundLayers[0].Speed;

            Initializer.Instance.SubscribeToStage(FindIfAllLayersHaveTreadmillTransform, 5);
        }

        private void FindIfAllLayersHaveTreadmillTransform()
        {
            foreach (BackgroundLayer bgl in backgroundLayers)
            {
                if (bgl.TreadmillTransform == null)
                {
                    Debug.Log("Treadmill.cs: can't find the treadmill transform on one of your background layers.");

                    canProcessBackgroundLayers = false;

                    return;
                }
            }
        }

        private void CreateHierarchy()
        {
            PlaceholdersOnTreadmillHierarchy = new GameObject("----- Placeholders On Treadmill -----").transform;
            GroundEnemiesOnTreadmillHierarchy = new GameObject("----- Ground Enemies On Treadmill -----").transform;
        }

        private void Update()
        {
            MoveLayers();
        }

        /// <summary>
        /// Moves all the layers that are dependent on the treadmill.
        /// </summary>
        private void MoveLayers()
        {
            if (Level.IsVertical)
            {
                MoveBackgroundsVertically();
                MovePlaceholdersVertically();
                MoveGroundEnemiesVertically();
            }
            else if (Level.IsHorizontal)
            {
                MoverBackgroundsHorizontally();
                MovePlaceholdersHorizontally();
                MoveGroundEnemiesHorizontally();
            }
        }

        private void MoveBackgroundsVertically()
        {
            if (!canProcessBackgroundLayers)
            {
                return;
            }

            foreach (BackgroundLayer bgl in backgroundLayers)
            {                
                bgl.TreadmillTransform.Translate(Vector3.down * bgl.Speed * speedMultiplier * Time.deltaTime);

                if (bgl.TreadmillTransform.position.y < ProjectConstants.ResettingDistance)
                {
                    TransformTools.ResetParentPosition(bgl.TreadmillTransform);
                }
            }
        }

        private void MoverBackgroundsHorizontally()
        {
            if (!canProcessBackgroundLayers)
            {
                return;
            }

            foreach (BackgroundLayer bgl in backgroundLayers)
            {
                bgl.TreadmillTransform.Translate(Vector3.left * bgl.Speed * speedMultiplier * Time.deltaTime);

                if (bgl.TreadmillTransform.position.x < ProjectConstants.ResettingDistance)
                {
                    TransformTools.ResetParentPosition(bgl.TreadmillTransform);
                }
            }
        }

        private void MovePlaceholdersVertically()
        {
            PlaceholdersOnTreadmillHierarchy.Translate(Vector3.down * mainBackgroundSpeed * speedMultiplier * Time.deltaTime);

            if (PlaceholdersOnTreadmillHierarchy.position.y < ProjectConstants.ResettingDistance)
            {
                TransformTools.ResetParentPosition(PlaceholdersOnTreadmillHierarchy);
            }
        }

        private void MovePlaceholdersHorizontally()
        {
            PlaceholdersOnTreadmillHierarchy.Translate(Vector3.left * mainBackgroundSpeed * speedMultiplier * Time.deltaTime);

            if (PlaceholdersOnTreadmillHierarchy.position.x < ProjectConstants.ResettingDistance)
            {
                TransformTools.ResetParentPosition(PlaceholdersOnTreadmillHierarchy);
            }
        }

        private void MoveGroundEnemiesHorizontally()
        {
            GroundEnemiesOnTreadmillHierarchy.Translate(Vector3.left * mainBackgroundSpeed * speedMultiplier * Time.deltaTime);

            if (GroundEnemiesOnTreadmillHierarchy.position.x < ProjectConstants.ResettingDistance)
            {
                TransformTools.ResetParentPosition(GroundEnemiesOnTreadmillHierarchy);
            }
        }

        private void MoveGroundEnemiesVertically()
        {
            GroundEnemiesOnTreadmillHierarchy.Translate(Vector3.down * mainBackgroundSpeed * speedMultiplier * Time.deltaTime);

            if (GroundEnemiesOnTreadmillHierarchy.position.y < ProjectConstants.ResettingDistance)
            {
                TransformTools.ResetParentPosition(GroundEnemiesOnTreadmillHierarchy);
            }
        }

        public void ChangeSpeedMultiplier(float newMultiplier)
        {
            speedMultiplier = newMultiplier;
        }
    }
}