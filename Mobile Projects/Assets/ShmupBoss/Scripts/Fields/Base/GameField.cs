using UnityEngine;
using UnityEditor;

namespace ShmupBoss
{
    /// <summary>
    /// The base class for multiple field type classes which are responsible for spawning/despawning 
    /// different game objects as they go in or out of a rectangular field.
    /// </summary>
    public abstract class GameField : MonoBehaviour
    {
        /// <summary>
        /// How is the field calculated, and what is the original source that will be used as a field.
        /// </summary>
        [Tooltip("How is the field calculated, and what is the original source that will be used as a field.")]
        [SerializeField]
        protected FieldSource fieldSource;

        /// <summary>
        /// Adds a general offset for the gamefield, this might be needed if you want to give extra margins, 
        /// so that for example things are not spawned/despawned exactly at the edge.
        /// </summary>
        [Tooltip("Adds a general offset for the gamefield, this might be needed if you want to give extra margins, " +
            "so that for example things are not spawned/despawned exactly at the edge.")]
        [SerializeField]
        protected float offset;

        [Tooltip("Determines the margin from the top for the field, check is drawing field and observe the effect.")]
        [Range(0.0f,1.0f)]
        [SerializeField]
        protected float offsetTop;

        [Tooltip("Determines the margin from the bottom for the field, check is drawing field and observe the effect.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        protected float offsetBottom;

        [Tooltip("Determines the margin from the right for the field, check is drawing field and observe the effect.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        protected float offsetRight;

        [Tooltip("Determines the margin from the left for the field, check is drawing field and observe the effect.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        protected float offsetLeft;

        /// <summary>
        /// This is only used if you have selected Input as the field source.
        /// </summary>
        [Header("If Source By Input")]
        [Tooltip("This is only used if you have selected Input as the field source.")]
        [SerializeField]
        protected Rect inputField;

#if UNITY_EDITOR
        [Header("Gizmos Settings")]
        [SerializeField]
        protected bool isDrawingField;
#endif

        /// <summary>
        /// The rect that will set the collider for the gamefield, this is essentially the gamefield.
        /// </summary>
        protected Rect boundries;

        /// <summary>
        /// The color which the gamefield gizmo will be drawn in, each different field should have a 
        /// different color so that you can tell which is which.
        /// </summary>
        protected abstract Color gizmosColor
        {
            get;
        }

        /// <summary>
        /// Subscribes the game field initialize method to the initializer.
        /// </summary>
        protected virtual void Awake()
        {
            Initializer.Instance.SubscribeToStage(Initialize, 2);
        }

        protected abstract void Initialize();

        /// <summary>
        /// Finds the rect boundries according to the selected field source.
        /// </summary>
        protected void FindFieldBoundries()
        {
            switch (fieldSource)
            {
                case FieldSource.Viewfield:
                    {
                        if (LevelCamera.Instance == null)
                        {
                            return;
                        }

                        Rect viewfieldRect = LevelCamera.Instance.Viewfield;
                        SetBoundriesToRect(viewfieldRect);
                        break;
                    }

                case FieldSource.BackgroundDimensions:
                    {
                        if (ScrollingBackground.Instance == null)
                        {
                            return;
                        }

                        Rect mainBackgroundPrefabRect = Dimensions.Vector2ToRect(ScrollingBackground.Instance.MainBackgroundPrefabDimensions);
                        SetBoundriesToRect(mainBackgroundPrefabRect);
                        break;
                    }

                case FieldSource.Input:
                    {
                        SetBoundriesToRect(inputField);
                        break;
                    }
            }
        }

        /// <summary>
        /// Sets the boundries field to the input rect taking into account all the offset values.
        /// </summary>
        /// <param name="rect">The rect that the boundries will be set to.</param>
        protected virtual void SetBoundriesToRect(Rect rect)
        {
            boundries = new Rect
            {
                xMin = rect.xMin + rect.width * offsetLeft,
                xMax = rect.xMin + rect.width * (1.0f - offsetRight),
                yMin = rect.yMin + rect.height * offsetBottom,
                yMax = rect.yMin + rect.height * (1.0f - offsetTop)
            };

            boundries.size += (Vector2.one * offset * 2.0f);
            boundries.center -= (Vector2.one * offset);
        }

        protected virtual void AddRigidBody2D()
        {
            Rigidbody2D rb2D = gameObject.AddComponent<Rigidbody2D>();
            rb2D.isKinematic = true;
        }

        protected virtual void AddBoxCollider2D()
        {
            BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = true;
            boxCollider2D.size = boundries.size;
            boxCollider2D.offset = boundries.center;

        }

#if UNITY_EDITOR
        public void FindBoundriesInEditor()
        {
            switch (fieldSource)
            {
                case FieldSource.Viewfield:
                    {
                        LevelCamera levelCamera = FindObjectOfType<LevelCamera>();

                        if (levelCamera == null)
                        {
                            return;
                        }

                        levelCamera.Initialize();

                        Rect viewfieldRect = levelCamera.Viewfield;
                        SetBoundriesToRect(viewfieldRect);
                        break;
                    }


                case FieldSource.BackgroundDimensions:
                    {
                        ScrollingBackground scrollingBackground = FindObjectOfType<ScrollingBackground>();

                        if (scrollingBackground == null)
                        {
                            return;
                        }

                        scrollingBackground.CacheBackgroundDimensions();
                        Rect mainBackgroundPrefabRect = Dimensions.Vector2ToRect(scrollingBackground.MainBackgroundPrefabDimensions);
                        SetBoundriesToRect(mainBackgroundPrefabRect);
                        break;
                    }

                case FieldSource.Input:
                    {
                        SetBoundriesToRect(inputField);
                        break;
                    }
            }
        }

        /// <summary>
        /// Draws the gamefield with the appropriate title and color. 
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            if (isDrawingField)
            {
                if (!EditorApplication.isPlaying)
                {
                    FindBoundriesInEditor();
                }

                GizmosExtension.DrawRect(boundries, gizmosColor);

                GUIStyle guiStyle = new GUIStyle();
                guiStyle.normal.textColor = gizmosColor;

                string className = StringTools.FindClassName(GetType());

                Handles.Label(GetGizmoLabelPosition(), className, guiStyle);
            }
        }

        protected abstract Vector2 GetGizmoLabelPosition();
#endif
    }
}