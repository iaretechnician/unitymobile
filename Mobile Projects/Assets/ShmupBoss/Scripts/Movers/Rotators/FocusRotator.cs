using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Rotates the game object on the Z axis to be pointing towards a tracked target.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Rotators/Focus Rotator")]
    public class FocusRotator : MonoBehaviour, IRotatable
    {
        /// <summary>
        /// The rotation change speed of turning towards the tracked target.
        /// </summary>
        [SB_HelpBox("In order for the focus rotator to know what to aim at, it needs a tracker. " +
            "Please add the needed type of tracker (i.e. TrackerPlayer.cs or TrackerRandomEnemy.cs etc..) " +
            "If no tracker has been added it will be assumed that this FocusRotator is targeting the " +
            "player and is aiming towards it.")]
        [Tooltip("The rotation change speed of turning towards the tracked target.")]
        [SerializeField]
        private float turnSpeed;

        /// <summary>
        /// The default facing angle of the agent type using this component, players are typically facing 
        /// upwards in vertical levels and rightward in horizontal ones, while non player agents are the opposite.
        /// </summary>
        private float facingAngle;

        private Tracker tracker;
        
        private Vector2 startDirection;
        private Vector2 currentDirection;            

        public Vector2 CurrentDirection
        {
            get;
            private set;
        }

        private void Awake()
        {                    
            tracker = GetComponent<Tracker>();

            if(tracker == null)
            {
                tracker = gameObject.AddComponent<TrackerPlayer>();
            }

            SetFacingAngleAndOriginalVector();
            SubscribeToRotationManagerOnZ();                       
        }

        private void Update()
        {
            FindCurrentDirection();
        }

        private void SetFacingAngleAndOriginalVector()
        {
            if (tracker is TrackerRandomEnemy)
            {
                facingAngle = FacingAngles.Player;
                startDirection = FacingDirections.Player;
            }
            else
            {
                facingAngle = FacingAngles.NonPlayer;
                startDirection = FacingDirections.NonPlayer;
            }                      
        }

        private void SubscribeToRotationManagerOnZ()
        {
            RotationManager rotationManager = GetComponent<RotationManager>();

            if (rotationManager == null)
            {
                rotationManager = gameObject.AddComponent<RotationManager>();
            }

            rotationManager.FacingAngle = facingAngle;
            rotationManager.Subscribe(this, Axis.Z);
        }

        public float EditRotation(float angle)
        {
            return Math2D.VectorToDegree(CurrentDirection);
        }

        private void FindCurrentDirection()
        {
            if (tracker != null)
            {
                float t = turnSpeed * ProjectConstants.TurnSpeedFactor * Time.deltaTime;

                if (tracker.IsTargetFound)
                {                   
                    currentDirection = Vector2.Lerp(currentDirection, tracker.DirectionToTarget, t);                    
                }
                else
                {
                    currentDirection = Vector2.Lerp(currentDirection, startDirection, t);
                }

                currentDirection.Normalize();
            }

            CurrentDirection = currentDirection;
        }
    }
}