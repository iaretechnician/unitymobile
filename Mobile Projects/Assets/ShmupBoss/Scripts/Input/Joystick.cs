using UnityEngine;
using UnityEngine.EventSystems;

namespace ShmupBoss
{
    /// <summary>
    /// A class which finds the Vector2 direction of a virtual stick image when dragged 
    /// in relation to a background image.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Input/Joystick")]
    public class Joystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        /// <summary>
        /// The limit of how much a stick can move in relation to its background image.
        /// </summary>
        [Tooltip("The limit of how much a stick can move in relation to its background image.")]
        [Range(0f, 1f)]
        public float StickLimit = 1.0f;
    
        /// <summary>
        /// The rect transform that is connected to the UI canvas image which represents the 
        /// joystick which the user will move.
        /// </summary>
        [Tooltip("The rect transform that is connected to the UI canvas image which represents the " +
            "joystick which the user will move.")]
        public RectTransform Stick;

        [Tooltip("The minimum radius at which the movement of the joystick will affect a change in the movement" +
            " of direction." + "\n\n" + "For example, a value of 0 means the slightest movement of joystick will " +
            "cause the player to move while a value of 0.5 means that you will need to move the joystick more than " +
            "half of the background radius for the player to start moving.")]
        [Range(0.0f, 0.5f)]
        public float minimumThreshold;

        private float backgroundRadius;
        private float minimumSensitivityRadius;
        private Vector2 backgroundPosition;       

        /// <summary>
        /// The current direction of the joystick in relation to the center of its background canvas image.
        /// </summary>
        public Vector2 Direction 
        { 
            get; 
            private set;
        }

        void Start()
        {
            RectTransform background = GetComponent<RectTransform>();

            if(background == null)
            {
                Debug.Log("Joystick.cs: Have you applied a joystick script to the wrong component? " +
                    "The joystic script needs to be applied to a UI element with a Rect Transform.");

                return;
            }

            backgroundRadius = background.sizeDelta.x * 0.5f;
            minimumSensitivityRadius = backgroundRadius * minimumThreshold;

            // Finds the joystick background position in camera pixels.
            backgroundPosition = RectTransformUtility.WorldToScreenPoint(new Camera(), background.position);            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        /// <summary>
        /// Finds a vector2 direction of a virtual stick being dragged in relation to a background image. 
        /// By finding your pointer position in relation to a background image center position.
        /// </summary>
        /// <param name="eventData">The current pointer event data to know the position of the drag movement.</param>
        public void OnDrag(PointerEventData eventData)
        {
            Vector2 inputVector;
            Vector2 direction = eventData.position - backgroundPosition;
            float directionMagnitude = direction.magnitude;

            // Remaps the input vector to be between its starting point and its limit so that the joystick 
            // handle does not go out of the joystick background bounds.
            if (directionMagnitude > backgroundRadius)
            {
                inputVector = direction.normalized;
            }
            else
            {
                inputVector = direction / backgroundRadius;
            }

            // Moves the virtual stick image to match the input.
            Stick.anchoredPosition = inputVector * backgroundRadius * StickLimit;

            // Assigns the value of the direction.
            if(directionMagnitude < minimumSensitivityRadius)
            {
                Direction = Vector2.zero;
            }
            else
            {
                Direction = inputVector;
            }
        }

        /// <summary>
        /// Resets the stick position when the pointer is up
        /// </summary>
        /// <param name="eventData">The current pointer event data</param>
        public void OnPointerUp(PointerEventData eventData)
        {
            Direction = Vector2.zero;
            Stick.anchoredPosition = Vector2.zero;
        }
    }
}