using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Base class for all agents movers, contains speed control and access 
    /// to what current direction the agent is in.
    /// </summary>
    public abstract class Mover : MonoBehaviour
    {
        public event CoreDelegate OnMoverStateChange;

        [SerializeField]
        protected float speed;
        public float Speed
        {
            get
            {
                return speed;
            }
        }

        protected float currentSpeed;
        public virtual float CurrentSpeed
        {
            get
            {
                return currentSpeed * CurrentMultiplier.EnemiesSpeedMultiplier;
            }
            protected set
            {
                currentSpeed = value;
            }
        }

        /// <summary>
        /// This will raise an event when the mover changes its state from moving to not moving or vice versa.
        /// </summary>
        private ChangingState<MoverState> changingMoverState =  new ChangingState<MoverState>();

        /// <summary>
        /// An enum for knowing if the player is currently moving or not.
        /// </summary>
        public MoverState CurrentMoverState
        {
            get
            {
                return changingMoverState.Current;
            }
            set
            {
                changingMoverState.Current = value;
            }
        }

        public Vector2 CurrentDirection 
        { 
            get; 
            protected set; 
        }

        public Vector2 DisplacementInLastFrame
        {
            get;
            protected set;
        }

        #region DirectionsAdditionalProperties
        public HorizontalDirection CurrentHorizontalDirection
        {
            get
            {
                if (CurrentSpeed == 0.0f)
                {
                    return HorizontalDirection.None;
                }

                return Directions.VectorToHorizontalDirection(CurrentDirection);
            }
        }

        public VerticalDirection CurrentVerticalDirection
        {
            get
            {
                if (CurrentSpeed == 0.0f)
                {
                    return VerticalDirection.None;
                }
                
                return Directions.VectorToVerticalDirection(CurrentDirection);
            }
        }

        public FourDirection CurrentFourDirection
        {
            get
            {
                if (CurrentSpeed == 0.0f)
                {
                    return FourDirection.None;
                }

                return Directions.VectorToFourDirection(CurrentDirection);
            }
        }

        public EightDirection CurrentEightDirection
        {
            get
            {
                if (CurrentSpeed == 0.0f)
                {
                    return EightDirection.None;
                }                    

                return Directions.VectorToEightDirection(CurrentDirection);
            }
        }
        #endregion

        protected virtual void OnValidate()
        {
            if(speed < 0)
            {
                speed = 0.0f;
            }
        }

        /// <summary>
        /// Caches the speed and adds the "RaiseOnMoverStateChange" event.
        /// </summary>
        protected virtual void Awake()
        {
            CurrentSpeed = speed;
            changingMoverState.OnStateChange += RaiseOnMoverStateChange;           
        } 
        
        protected virtual void Update()
        {
            FindCurrentDirection();
            Move();
            SetCurrentMoverState();
        }

        protected abstract void FindCurrentDirection();

        protected virtual void Move()
        {
            DisplacementInLastFrame = CurrentDirection * CurrentSpeed * Time.deltaTime;
            transform.position += Math2D.Vector2ToVector3(DisplacementInLastFrame);
        }

        protected virtual void SetCurrentMoverState()
        {
            if (DisplacementInLastFrame == Vector2.zero)
            {
                CurrentMoverState = MoverState.NotMoving;
            }
            else
            {
                CurrentMoverState = MoverState.Moving;
            }
        }

        public void ModifyCurrentSpeed(float newSpeed)
        {
            CurrentSpeed = newSpeed;
        }

        protected void RaiseOnMoverStateChange()
        {
            OnMoverStateChange?.Invoke(new MoverChangeArgs(changingMoverState.Current));
        }
    }
}