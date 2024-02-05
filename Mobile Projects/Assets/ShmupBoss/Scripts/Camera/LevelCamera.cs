using UnityEngine;
using UnityEditor;

namespace ShmupBoss
{
    /// <summary>
    /// Controls the camera size and offsets it to align it with the background or input, 
    /// it also controls the aspect and zoom values and holds the viewfield rect which is used 
    /// by other fields.<br></br>
    /// This script does contain numerous instances where it checks for a 2D level which might
    /// seem unnecessary, but it is done in preparation for a possible inclusion of a 3D level.  
    /// </summary>
    [AddComponentMenu("Shmup Boss/Camera/Level Camera")]
    [RequireComponent(typeof(Camera))]
    public class LevelCamera : MonoBehaviour
    {
        public static LevelCamera Instance;
        
        /// <summary>
        /// If a viewfield is calculated by the background or by input.
        /// </summary>
        [Tooltip("If a viewfield is calculated by the background or by input.")]
        [Header("Viewfield Source")]
        [SerializeField]
        private ViewfieldSource viewfieldSource;

        /// <summary>
        /// If you have selected by input in the viewfield source, 
        /// this input will control the camera viewfield.
        /// </summary>
        [Header("If Source By Input")]
        [Tooltip("If you have selected by input in the viewfield source, " +
            "this input will control the camera viewfield.")]
        [SerializeField]
        private Vector2 inputFieldSize;

        /// <summary>
        /// The input field translated from vector to rect.
        /// </summary>
        private Rect inputField;

        /// <summary>
        /// The rect the camera can view, this is calculated either by the backgorund (scrolling background) 
        /// or by the input and is used by many other scripts that generate different fields 
        /// which might be dependent on this viewfield.
        /// </summary>
        public Rect Viewfield
        {
            get;
            private set;
        }

        /// <summary>
        /// Use values over 1 to zoom in, if you want to have camera shake effect, 
        /// or camera tracking, you must have some zoom value over 1.0f.
        /// </summary>
        [Tooltip("Use values over 1 to zoom in, if you want to have camera shake effect, " +
            "or camera tracking, you must have some zoom value over 1")]
        [Space]
        [Header("Camera Zoom")]
        [SerializeField]
        private float zoomFactor = 1.0f;

        /// <summary>
        /// This aspect ratio will be forced on the level if you are building a game for desktop and the 
        /// level is vertical, otherwise this aspect ration will not be used.<br></br> 
        /// In mobile or a horizontal level the camera fills the level automatically.
        /// </summary>
        [Space]
        [Header("IF Desktop Build & Vertical Level")]
        [Tooltip("This aspect ratio will be forced on the level if you are building a game for desktop and the level" +
            " is vertical, otherwise this aspect ration will not be used. In mobile or a horizontal level the " +
            "camera fills the level automatically.")]
        [SerializeField]
        private VerticalAspectRatio verticalAspectRatio;
        public VerticalAspectRatio _VerticalAspectRatio
        {
            get
            {
                return verticalAspectRatio;
            }
        }

        [Tooltip("This value will be used only if it's a vertical desktop build and you have selected " +
            "input as a vertical aspect ratio.")]
        [SerializeField]
        private float inputVerticalAspectRatio = 1.0f;

        /// <summary>
        /// The vertical aspect ratio but in floating value instead of an enum.
        /// </summary>
        public float VerticalAspect
        {
            get
            {
                return Dimensions.VerticalAspectRatioToFloat(verticalAspectRatio, inputVerticalAspectRatio);
            }
        }

        private float originalCameraAspect;

        public float ZPositionIn2DLevel 
        {
            get
            {
                if(Level.Instance != null)
                {
                    return -Level.Instance.SpaceBetween2DDepthIndices * ProjectConstants.DepthIndexLimit;
                }
                else
                {
                    return -ProjectConstants.DefaultSpaceBetween2DDepthIndices * ProjectConstants.DepthIndexLimit;
                }
            }
        }

        private float cameraOrthographicSize;
        private Camera cam;
        private ScrollingBackground scrollingBackground;
        private CameraPlayerTracker cameraPlayerTracker;

#if UNITY_EDITOR
        [Space]
        [Header("Gizoms")]
        [SerializeField]
        protected bool isDrawingInputField;
#endif

        /// <summary>
        /// Subscribes initializing the camera to the first stage of the initializer.<br></br>
        /// The camera needs to be initialized at the very beginning because 
        /// many other scripts are potentially dependent on its viewfield.
        /// </summary>
        private void Awake()
        {
            Instance = this;

            Initializer.Instance.SubscribeToStage(Initialize, 1);
        }

        private void OnValidate()
        {
            if(zoomFactor <= Mathf.Epsilon)
            {
                zoomFactor = 1.0f;
            }

            if(inputVerticalAspectRatio <= Mathf.Epsilon)
            {
                inputVerticalAspectRatio = 1.0f;
            }
        }

        public void Initialize()
        {
            CacheComponents();

            if (scrollingBackground == null)
            {
                return;
            }

            SetCameraType();
            SetAspectRatio();
            FindInputField();
            FitCameraSize();
            FindSpaceBetweenIndices();
            ShiftCameraPosition();
            FindCompleteCameraViewfield();
        }

        private void FindInputField()
        {
            inputField = new Rect((inputFieldSize.x * -0.5f), (inputFieldSize.y * -0.5f), inputFieldSize.x, inputFieldSize.y);
        }

        private void CacheComponents()
        {
            cam = GetComponent<Camera>();

            cam.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
            originalCameraAspect = cam.aspect;

            scrollingBackground = ScrollingBackground.Instance;

            if (scrollingBackground == null)
            {
                scrollingBackground = FindObjectOfType<ScrollingBackground>();
            }

            cameraPlayerTracker = GetComponent<CameraPlayerTracker>();
        }

        private void SetCameraType()
        {
            if (Level.Is2D)
            {
                cam.orthographic = true;
            }
        }

        /// <summary>
        /// Depending if building for desktop/mobile and if vertical/horizontal, 
        /// this method sets the aspect ration and any needed screen parameters.
        /// </summary>
        private void SetAspectRatio()
        {
#if !UNITY_IOS && !UNITY_ANDROID
            if (Level.IsVertical && verticalAspectRatio != VerticalAspectRatio.None)
            {
                ForceVertical(verticalAspectRatio);
            }
#else
            //if the device is a portable device, the Screen rotation should be locked to Level View Type
            if (Level.IsVertical)
		    {
                Screen.orientation = ScreenOrientation.Portrait;
		        Screen.autorotateToLandscapeLeft = false;
		        Screen.autorotateToLandscapeRight = false;
		        Screen.autorotateToPortrait = true;
		        Screen.autorotateToPortraitUpsideDown = true;
		    }

		    if (Level.IsHorizontal)
		    {
                Screen.orientation = ScreenOrientation.Landscape;
		        Screen.autorotateToLandscapeLeft = true;
		        Screen.autorotateToLandscapeRight = true;
		        Screen.autorotateToPortrait = false;
		        Screen.autorotateToPortraitUpsideDown = false;
		    }
#endif
        }

        /// <summary>
        /// Changes the camera rect to whatever is needed to fit the view into the selected aspect ratio.
        /// </summary>
        /// <param name="aspect"></param>
        private void ForceVertical(VerticalAspectRatio aspect)
        {
            float originalToNewAspectRatio = (1.0f / originalCameraAspect) * VerticalAspect;

            Rect cameraRect = cam.rect;
            cameraRect.width = originalToNewAspectRatio;
            cameraRect.position = new Vector2(0.5f - (cameraRect.width * 0.5f), 0.0f);
            cam.rect = cameraRect;
        }

        /// <summary>
        /// Changes the camera size to fit the background or the input depending on your selection.
        /// </summary>
        private void FitCameraSize()
        {
            // Unity's orthographic size is half the camera's view height, the aspect is width divided by height.
            // To find the new camera size based on a background width/height, we will need to divide the half 
            // width of the background by the camera aspect for the vertical version, and to simply use the half 
            // background height for the horizontal version.
            if(viewfieldSource == ViewfieldSource.BackgroundDimensions)
            {
                if (Level.Is2D)
                {
                    if (Level.IsVertical)
                    {
                        cameraOrthographicSize = (scrollingBackground.MainBackgroundPrefabWidth * 0.5f) / cam.aspect;
                    }
                    else if (Level.IsHorizontal)
                    {
                        cameraOrthographicSize = scrollingBackground.MainBackgroundPrefabHeight * 0.5f;
                    }
                }
            }
            else if(viewfieldSource == ViewfieldSource.Input)
            {
                if (Level.Is2D)
                {
                    if (Level.IsVertical)
                    {
                        cameraOrthographicSize = inputField.width * 0.5f / cam.aspect;
                    }
                    else if (Level.IsHorizontal)
                    {
                        cameraOrthographicSize = inputField.height * 0.5f;
                    }
                }
            }

            cam.orthographicSize = cameraOrthographicSize / zoomFactor;
        }

        /// <summary>
        /// While this method at the moment seem superfluous, if a 3D level is introduced
        /// at some point it will have more utility.
        /// </summary>
        private void FindSpaceBetweenIndices()
        {
            Level level = Level.Instance;
            
            if(level == null)
            {
                level = FindObjectOfType<Level>();
            }
            
            if(level == null)
            {
                return;
            }

            if (Level.Is2D)
            {
                Level.SpaceBetweenIndices = level.SpaceBetween2DDepthIndices;
            }
        }

        /// <summary>
        /// Used in order to shift the camera upward or downward and make the background in full view of the 
        /// camera when level starts.
        /// </summary>
        private void ShiftCameraPosition()
        {
            if(viewfieldSource == ViewfieldSource.BackgroundDimensions)
            {
                if (Level.Is2D)
                {
                    if (Level.IsVertical)
                    {
                        float cameraYPosition = cameraOrthographicSize - (scrollingBackground.MainBackgroundPrefabHeight * 0.5f);
                        transform.position = new Vector3(0f, cameraYPosition, ZPositionIn2DLevel);
                    }
                    else if (Level.IsHorizontal)
                    {
                        float cameraXPosition = (cameraOrthographicSize * cam.aspect) - (scrollingBackground.MainBackgroundPrefabWidth * 0.5f);
                        transform.position = new Vector3(cameraXPosition, 0f, ZPositionIn2DLevel);
                    }
                }
            }
            else if(viewfieldSource == ViewfieldSource.Input)
            {
                if (Level.Is2D)
                {
                    if (Level.IsVertical)
                    {
                        float cameraYPosition = cameraOrthographicSize - (inputField.height * 0.5f);
                        transform.position = new Vector3(0f, cameraYPosition, ZPositionIn2DLevel);
                    }
                    else if (Level.IsHorizontal)
                    {
                        float cameraXPosition = (cameraOrthographicSize * cam.aspect) - (inputField.width * 0.5f);
                        transform.position = new Vector3(cameraXPosition, 0f, ZPositionIn2DLevel);
                    }
                }
            }
        }

        /// <summary>
        /// After the camera position and size have been set, the viewfield is calculated.
        /// </summary>
        private void FindCompleteCameraViewfield()
        {
            if (Level.Is2D)
            {
                Viewfield = new Rect
                    (0f,
                    0f,
                    ((cameraOrthographicSize * 2) * cam.aspect),
                    (cameraOrthographicSize * 2))
                {
                    center = transform.position
                };
            }
        }

        /// <summary>
        /// Finds the amount of shift a camera underwent in order to have the viewfield at the very start
        /// of a background or input, this value is used by all the different fields so that they can be 
        /// aligned with the viewfield if needed.
        /// </summary>
        /// <returns>The camera shifted position</returns>
        public Vector2 GetCameraShiftedPosition()
        {
            if (viewfieldSource == ViewfieldSource.BackgroundDimensions)
            {
                ScrollingBackground scrollingBackground = FindObjectOfType<ScrollingBackground>();

                if (scrollingBackground == null)
                {
                    return Vector2.zero;
                }

                if (Level.IsVertical)
                {
                    float cameraYPosition = cameraOrthographicSize - (scrollingBackground.MainBackgroundPrefabHeight * 0.5f);
                    return new Vector2(0.0f, cameraYPosition);
                }
                else if (Level.IsHorizontal)
                {
                    float cameraXPosition = (cameraOrthographicSize * cam.aspect) - (scrollingBackground.MainBackgroundPrefabWidth * 0.5f);
                    return new Vector2(cameraXPosition, 0.0f);
                }
            }
            else if(viewfieldSource == ViewfieldSource.Input)
            {
                if (Level.IsVertical)
                {
                    float cameraYPosition = cameraOrthographicSize - (inputFieldSize.y * 0.5f);
                    return new Vector2(0.0f, cameraYPosition);
                }
                else if (Level.IsHorizontal)
                {
                    float cameraXPosition = (cameraOrthographicSize * cam.aspect) - (inputFieldSize.x * 0.5f);
                    return new Vector2(cameraXPosition, 0.0f);
                }
            }

            return Vector2.zero;
        }

        public void ChangeCameraSize(float factor)
        {
            cam.orthographicSize = cameraOrthographicSize / factor;
        }

        /// <summary>
        /// Shakes the camera using the camera tracking script.
        /// </summary>
        public void ShakeCamera()
        {
            if(cameraPlayerTracker == null)
            {
                return;
            }

            cameraPlayerTracker.StartShaking();
        }

        /// <summary>
        /// After a vertical aspect has been forced (must be building for desktop and a vertical level), 
        /// this finds the empty width after forcing the camera.
        /// </summary>
        /// <param name="canvas">The complete canvas you want to compare with the camera used space.</param>
        /// <returns>The unused width</returns>
        public float GetUnusedWidth(RectTransform canvas)
        {
            if(canvas == null)
            {
                return 0.0f;
            }
            
            float usedSpace = canvas.rect.height * VerticalAspect;
            return canvas.rect.width - usedSpace;
        }

        /// <summary>
        /// Adjusts the canvas by moving the offset min and max with the unused margins values to contract 
        /// the canvas into the newly forced aspect.
        /// </summary>
        /// <param name="target">The canvas to modify and contract.</param>
        /// <param name="unusedWidth">The extra margins that need to be removed from the canvas.</param>
        public void AdujstCanvasToForceAspect(RectTransform target, float unusedWidth)
        {
            Vector2 offsetMin = target.offsetMin;
            Vector2 offsetMax = target.offsetMax;

            offsetMin.x += unusedWidth * 0.5f;
            offsetMax.x -= unusedWidth * 0.5f;

            target.offsetMin = offsetMin;
            target.offsetMax = offsetMax;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (isDrawingInputField)
            {
                FindInputField();
                GizmosExtension.DrawRect(inputField, Color.white);

                GUIStyle guiStyle = new GUIStyle();
                guiStyle.normal.textColor = Color.white;

                Vector2 gizmosLabelPosition = new Vector2(inputField.xMin + ProjectConstants.LabelMarginDistance, inputField.yMin);

                Handles.Label(gizmosLabelPosition, "Camera Input Field", guiStyle);
            }
        }
#endif
    }
}