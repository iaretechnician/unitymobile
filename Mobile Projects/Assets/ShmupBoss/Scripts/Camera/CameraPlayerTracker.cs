using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// When the camera is zoomed in, the camera player tracker script will move the camera to follow 
    /// the player and it will also activate the camera shake if an agent calls for it.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Camera/Camera Player Tracker")]
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(LevelCamera))]
    public class CameraPlayerTracker : MonoBehaviour
    {
        /// <summary>
        /// This time determines how long it will take for the camera to follow the player into its new position. 
        /// A shorter time means it will follow it instantly, a longer time means it will lag behind and move slowly 
        /// towards the player.<br></br>
        /// This will also define how long it takes for the camera to get back to focusing on the player after 
        /// shaking, it is better to avoid larger values and to stick to values less than 1.
        /// </summary>
        [Header("Camera Tracking Player Settings")]
        [Tooltip("This time determines how long it will take for the camera to follow the player into its" +
            " new position. A shorter time means it will follow it instantly, a longer time means it will " +
            "lag behind and move slowly towards the player. This will also define how long it takes for " +
            "the camera to get back to focusing on the player after shaking, it is better to avoid larger values " +
            "and to stick to values less than 1.")]
        [SerializeField]
        private float trackingTransitionTime;

        /// <summary>
        /// How long does the camera shake last.
        /// </summary>
        [Header("Shake Settings")]
        [Tooltip("How long does the camera shake last.")]
        [SerializeField]
        private float shakeDuration;

        /// <summary>
        /// When the camera shakes, it will move from side to side. The shake distance is how much the camera
        /// will move when it shakes.<br></br>
        /// Please note that this distance is capped by the actual camera zoom. If the camera is zoomed in 
        /// only slightly, then there will be no distance for it to shake.<br></br>
        /// You need to be carful when assigning the shak distance, not to assign a value which is too big 
        /// which would give you unpredictable results, use value in accordance with your camera movement zone.
        /// </summary>
        [Tooltip("When the camera shakes, it will move from side to side. The shake distance is how much the " +
            "camera will move when it shakes. Please note that this distance is capped by the actual camera " +
            "zoom. If the camera is zoomed in only slightly, then there will be no distance for it to shake." +
            "You need to be careful when assigning the shak distance, not to assign a value which is too big " +
            "which would give you unpredictable results, use values in accordance with your camera movement zone.")]
        [SerializeField]
        private float shakeDistance;

        /// <summary>
        /// How fast the camera moves when shaking.
        /// </summary>
        [SerializeField]
        private float shakeSpeed;

        private Camera cam;
        private LevelCamera levelCamera;

        /// <summary>
        /// True if the camera is currently shaking.
        /// </summary>
        private bool isShaking;

        /// <summary>
        /// This is used as a trigger to change the camera shake direction after it has reached the limit distance.
        /// </summary>
        private bool flipShakeDirectionTrigger;

        /// <summary>
        /// The time counter for how long the camera has been shaking.
        /// </summary>
        private float shakeCounter;

        float xShakeDistance;
        float yShakeDistance;
        private Vector2 shakeVector2;
        private bool canAccessPlayerComp;

        private void Awake()
        {
            CacheCompsAndCounter();
            Initializer.Instance.SubscribeToStage(FindIfCanAccessPlayerComp, 6);
        }

        private void CacheCompsAndCounter()
        {
            cam = GetComponent<Camera>();
            levelCamera = GetComponent<LevelCamera>();

            shakeCounter = shakeDuration;
        }

        private void FindIfCanAccessPlayerComp()
        {
            if (Level.Instance == null || Level.Instance.PlayerComp == null)
            {
                canAccessPlayerComp = false;

                Debug.Log("CameraPlayerTracker.cs: was unable to access the player comp in the player instance, " +
                    "double check that you have a level instance loaded properly with a player comp.");

                return;
            }

            canAccessPlayerComp = true;
        }

        void Update()
        {
            MoveCamera();
        }

        /// <summary>
        /// Moves the camera taking into consideration tracking the player and any shake movement.
        /// </summary>
        private void MoveCamera()
        {
            if (isShaking)
            {
                FindShakeVector();
            }

            FollowPlayer();
            ClampPosition();
            Shake();
            ClampPosition();
        }

        /// <summary>
        /// Calculates the current camera shake vector, this vector is used to move the camera from side to side, 
        /// if the level is vertical it will move the camera left/right, if it is horizontal it will move the 
        /// camera up/down.
        /// </summary>
        private void FindShakeVector()
        {
            shakeCounter -= Time.deltaTime;

            if (shakeCounter <= 0.0f)
            {
                ResetShakeValues();

                return;
            }

            if (Level.IsVertical)
            {
                CalculateXShakeDistance();
            }
            else if (Level.IsHorizontal)
            {
                CalculateYShakeDistance();
            }

            shakeVector2 = new Vector2(xShakeDistance, yShakeDistance);
        }

        private void ResetShakeValues()
        {
            isShaking = false;
            xShakeDistance = 0.0f;
            yShakeDistance = 0.0f;

            shakeVector2 = Vector2.zero;
        }

        private void CalculateXShakeDistance()
        {
            if (xShakeDistance < shakeDistance && !flipShakeDirectionTrigger)
            {
                xShakeDistance += Time.deltaTime * shakeSpeed;
            }
            else
            {
                if (xShakeDistance > -shakeDistance)
                {
                    xShakeDistance -= Time.deltaTime * shakeSpeed;
                    flipShakeDirectionTrigger = true;
                }
                else
                {
                    flipShakeDirectionTrigger = false;
                }
            }
        }

        private void CalculateYShakeDistance()
        {
            if (yShakeDistance < shakeDistance && !flipShakeDirectionTrigger)
            {
                yShakeDistance += Time.deltaTime * shakeSpeed;
            }
            else
            {
                if (yShakeDistance > -shakeDistance)
                {
                    yShakeDistance -= Time.deltaTime * shakeSpeed;
                    flipShakeDirectionTrigger = true;
                }
                else
                {
                    flipShakeDirectionTrigger = false;
                }
            }
        }

        /// <summary>
        /// Moves the camera to focus on the player, this is dependent on the tracking transition time.
        /// </summary>
        private void FollowPlayer()
        {
            if (!canAccessPlayerComp)
            {
                return;
            }

            Vector3 PlayerCurrentPosition = Level.Instance.PlayerComp.transform.position;
            Vector3 cameraCurrentPosition = transform.position;

            float changeFactor;

            if (trackingTransitionTime <= 0.0f)
            {
                changeFactor = 1.0f;
            }
            else
            {
                changeFactor = Time.deltaTime / trackingTransitionTime;
            }
            
            Vector3 interpolatedPosition = Vector3.Lerp(cameraCurrentPosition, PlayerCurrentPosition, changeFactor);

            transform.position = new Vector3(interpolatedPosition.x,
                interpolatedPosition.y,
                levelCamera.ZPositionIn2DLevel);
        }

        /// <summary>
        /// Adds the shake vector value to the camera position.
        /// </summary>
        private void Shake()
        {
            transform.position = new Vector3(transform.position.x + shakeVector2.x,
                transform.position.y + shakeVector2.y,
                levelCamera.ZPositionIn2DLevel);
        }

        /// <summary>
        /// Clamps the position of the camera so that it doesn't go out of the viewfield.
        /// </summary>
        private void ClampPosition()
        {
            Rect cameraShakeMovementZone = FindCameraMovementZone();

            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, cameraShakeMovementZone.xMin, cameraShakeMovementZone.xMax),
                Mathf.Clamp(transform.position.y, cameraShakeMovementZone.yMin, cameraShakeMovementZone.yMax),
                levelCamera.ZPositionIn2DLevel);
        }

        /// <summary>
        /// Finds the rectangle which the camera can move in without going out of the viewfield.
        /// </summary>
        /// <returns>The rectangle which the camera can move in without going out of the viewfield.</returns>
        private Rect FindCameraMovementZone()
        {
            Vector2 cameraSize = new Vector2(cam.orthographicSize * cam.aspect, cam.orthographicSize);

            return new Rect(
                levelCamera.Viewfield.position.x + cameraSize.x,
                levelCamera.Viewfield.position.y + cameraSize.y,
                levelCamera.Viewfield.size.x - cameraSize.x * 2,
                levelCamera.Viewfield.size.y - cameraSize.y * 2);
        }

        public void StartShaking()
        {
            isShaking = true;
            shakeCounter = shakeDuration;
        }
    }
}