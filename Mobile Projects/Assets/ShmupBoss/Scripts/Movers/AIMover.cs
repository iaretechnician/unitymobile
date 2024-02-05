using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Will start with an initial move based on the spawn side, afterwards it will either wait or when the
    /// player is straight ahead of it, it will perform a random direction move (maneuver move).<br></br>
    /// </summary>
    [AddComponentMenu("Shmup Boss/Movers/AI Mover")]
    public class AIMover : Mover, ISideSpawnable
    {
        public FourDirection InitialDirection
        {
            get;
            set;
        }

        /// <summary>
        /// After being spawned, the AI mover will move from its spawning position in its initial 
        /// direction to take position in the playfield.<br></br>
        /// This is the distance it will move from its spawning position.
        /// </summary>
        [Tooltip("After being spawned, the AI mover will move from its spawning position in its initial " +
            "direction to take position in the playfield. This is the distance it will move from its " +
            "spawning position.")]
        [Space]
        [SerializeField]
        private float firstMoveDistance;

        /// <summary>
        /// The distance it will move after it has found the target ahead.
        /// </summary>
        [Tooltip("The distance it will move after it has found the target ahead.")]
        [SerializeField]
        private float maneuverDistance;

        /// <summary>
        /// Time after the AI mover has found the target ahead and before it makes the maneuver move.
        /// </summary>
        [Tooltip("Time after the AI mover has found the target ahead and before it makes the maneuver move.")]
        [SerializeField]
        private float timeBeforeManeuver;
        
        /// <summary>
        /// Wait time after the AI mover has reached it target and before it can start looking for target 
        /// ahead once again.
        /// </summary>
        [Tooltip("Wait time after the AI mover has reached it target and before it can start looking " +
            "for target ahead once again.")]
        [SerializeField]
        private float timeForNextManeuver;

        /// <summary>
        /// This width will determine when will this mover consider that its target (the player) 
        /// is ahead and this in turn will determine when it will start making the maneuver move.<br></br>
        /// You can think of this width as the inaccuracy in the line of sight. The target doesn't have to be 
        /// straight ahead for the AI mover to consider its target is ahead, if it's within this width it 
        /// will consider it ahead.
        /// </summary>
        [Tooltip("This width will determine when will this mover consider that its target (the player) " +
            "is ahead and this in turn will determine when it will start making the maneuver move." + "\n" +
            "You can think of this width as the inaccuracy in the line of sight. The target doesn't have to be " +
            "straight ahead for the AI mover to consider its target is ahead, if it's within this width it will " +
            "consider it ahead.")]
        [SerializeField]
        private float avgColliderWidth;

        /// <summary>
        /// This will determine the movement zone of the AI mover, the top value will determine how far 
        /// from the top margin can this mover come close to.
        /// </summary>
        [Header("Offset Settings")]
        [Tooltip("This will determine the movement zone of the AI mover, the top value will determine how far " +
            "from the top margin can this mover come close to.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float OffsetTop;

        /// <summary>
        /// This will determine the movement zone of the AI mover, the bottom value will determine how far 
        /// from the bottom margin can this mover come close to.
        /// </summary>
        [Tooltip("This will determine the movement zone of the AI mover, the bottom value will determine " +
            "how far from the bottom margin can this mover come close to.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float OffsetBottom;

        /// <summary>
        /// This will determine the movement zone of the AI mover, the left value will determine how far 
        /// from the left margin can this mover come close to.
        /// </summary>
        [Tooltip("This will determine the movement zone of the AI mover, the left value will determine " +
            "how far from the left margin can this mover come close to.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float OffsetLeft;

        /// <summary>
        /// This will determine the movement zone of the AI mover, the Right value will determine how far 
        /// from the Right margin can this mover come close to.
        /// </summary>
        [Tooltip("This will determine the movement zone of the AI mover, the Right value will determine " +
            "how far from the Right margin can this mover come close to.")]
        [Range(0.0f, 1.0f)]
        [SerializeField]
        private float OffsetRight;

        /// <summary>
        /// This mover can be in any number of states and according to those states it will 
        /// either move in a particular direction or stop.
        /// </summary>
        private ManeuverState maneuverState;

        private Vector2 targetPosition;
        
        /// <summary>
        /// The time until the mover is allowed to perform another maneuver.
        /// </summary>
        private float holdManeuverTime;

        /// <summary>
        /// This counter is used to avoid an infinite loop.
        /// </summary>
        private int moveAttemptsCounter;
        private const int MaxMoveAttempts = 5;

        private Rect MoverField;

        private PlayerMover playerMover;
        private FourDirection playerCurrentFourDirection;

        private bool isLateEnabled;

        [Header("Gizmo Settings")]
        [Space]
        [Tooltip("Draws the mover average collider width")]
        [SerializeField]
        private bool isDrawingGizmos;

        protected override void Awake()
        {
            base.Awake();

            Initializer.Instance.SubscribeToStage(FindMoverFieldAfterOffset, 4);
            Initializer.Instance.SubscribeToStage(CachePlayerMoverComp, 5);
        }

        protected override void Update()
        {
            if (!isLateEnabled)
            {
                InitiateFirstMove();
                isLateEnabled = true;
            }
            
            base.Update();
        }

        private void OnDisable()
        {
            isLateEnabled = false;
        }

        private void InitiateFirstMove()
        {
            maneuverState = ManeuverState.StartingMove;
            targetPosition = (Directions.FourDirectionToVector2(InitialDirection) * firstMoveDistance)
                + (Vector2)transform.position;
        }

        private void FindMoverFieldAfterOffset()
        {
            MoverField = new Rect
            {
                xMin = PlayField.Instance.Boundries.xMin + (PlayField.Instance.Boundries.width * OffsetLeft),
                xMax = PlayField.Instance.Boundries.xMin + (PlayField.Instance.Boundries.width * (1.0f - OffsetRight)),
                yMin = PlayField.Instance.Boundries.yMin + (PlayField.Instance.Boundries.height * OffsetBottom),
                yMax = PlayField.Instance.Boundries.yMin + (PlayField.Instance.Boundries.height * (1.0f - OffsetTop))
            };
        }

        private void CachePlayerMoverComp()
        {
            if (Level.Instance == null)
            {
                Debug.Log("AIMover.cs: AIMover needs to find an instance of the level script in order to locate " +
                    "the player mover. AI mover was unable to find an instance of the level.");

                return;
            }

            if (Level.Instance.PlayerComp == null)
            {
                Debug.Log("AIMover.cs: AIMover needs to find an instance of the player comp to be able to coordinate " +
                    "the maneuver moves. It was unable to locate the player comp inside the instance of the level.");

                return;
            }

            if (Level.Instance.PlayerComp.PlayerMoverComp == null)
            {
                Debug.Log("AIMover.cs: AIMover was unable to find the player mover comp, something " +
                    "wrong went with caching the player mover comp inside the player script.");

                return;
            }

            playerMover = Level.Instance.PlayerComp.PlayerMoverComp;
        }

        /// <summary>
        /// UpdateDirection is called from inside the mover class
        /// update function on every frame to update the mover direction.
        /// </summary>
        /// <returns>The current mover direction.</returns>
        protected override void FindCurrentDirection()
        {
            switch (maneuverState)
            {
                case ManeuverState.StartingMove:

                    if (IsWithinTargetPositionSqrThreshold())
                    {
                        maneuverState = ManeuverState.Waiting;
                        break;
                    }

                    CurrentDirection = Directions.FourDirectionToVector2(InitialDirection);
                    return;

                case ManeuverState.Waiting:

                    if (playerMover == null)
                    {
                        Debug.Log("AIMover.cs: AIMover is dependent on the player mover to operate. An AIMover was unable to " +
                            "find a player mover comp. Did you forget a level script with the player plugged in to it.");

                        break;
                    }

                    if (Level.IsVertical)
                    {
                        // Wait for the player to be in front of the mover.
                        if (Mathf.Abs(playerMover.transform.position.x - transform.position.x) <= avgColliderWidth)
                        {
                            SetToTargetAhead();
                            break;
                        }
                    }
                    else if (Level.IsHorizontal)
                    {
                        // Wait for the player to be in front of the mover.
                        if (Mathf.Abs(playerMover.transform.position.y - transform.position.y) <= avgColliderWidth)
                        {
                            SetToTargetAhead();
                            break;
                        }
                    }

                    break;

                case ManeuverState.TargetAhead:

                    //wait if the mover is on hold.
                    if (holdManeuverTime > Time.time)
                    {
                        break;
                    }

                    holdManeuverTime = Time.time + timeBeforeManeuver;
                    maneuverState = ManeuverState.SettingManeuverMove;

                    break;

                case ManeuverState.SettingManeuverMove:

                    if (holdManeuverTime > Time.time)
                    {
                        break;
                    }

                    Vector2 maneuverDirection = FindManeuverDirection();        

                    if (maneuverDirection != Vector2.zero)
                    {
                        maneuverState = ManeuverState.DoingManeuverMove;
                    }

                    CurrentDirection = maneuverDirection;
                    return;

                case ManeuverState.DoingManeuverMove:

                    if (IsWithinTargetPositionSqrThreshold())
                    {
                        holdManeuverTime = Time.time + timeForNextManeuver;
                        maneuverState = ManeuverState.Waiting;
                    }

                    return;
            }

            CurrentDirection = Vector2.zero;
        }

        private void SetToTargetAhead()
        {
            playerCurrentFourDirection = playerMover.CurrentFourDirection;
            maneuverState = ManeuverState.TargetAhead;
        }

        private Vector2 FindManeuverDirection()
        {
            if (Level.IsVertical)
            {
                return FindManeuverMoveInVerticalLevel();
            }
            else if (Level.IsHorizontal)
            {
                return FindManeuverMoveInHorizontalLevel();
            }

            return Vector2.zero;
        }

        private Vector2 FindManeuverMoveInVerticalLevel()
        {
            if (playerCurrentFourDirection == FourDirection.Left)
            {
                if (CheckIfDirectionIsInMoverField(RamdomDirections.Leftward))
                {
                    return pickMove(RamdomDirections.Leftward);
                }
                else
                {
                    return pickMove(RamdomDirections.Rightward);
                }
            }
            else
            {
                if (CheckIfDirectionIsInMoverField(RamdomDirections.Rightward))
                {
                    return pickMove(RamdomDirections.Rightward);
                }                    
                else
                {
                    return pickMove(RamdomDirections.Leftward);
                }
            }
        }

        Vector2 FindManeuverMoveInHorizontalLevel()
        {
            if (playerCurrentFourDirection == FourDirection.Up)
            {
                if (CheckIfDirectionIsInMoverField(RamdomDirections.Upward))
                {
                    return pickMove(RamdomDirections.Upward);
                }                   
                else
                {
                    return pickMove(RamdomDirections.Downward);
                }

            }
            else
            {
                if (CheckIfDirectionIsInMoverField(RamdomDirections.Downward))
                {
                    return pickMove(RamdomDirections.Downward);
                }                    
                else
                {
                    return pickMove(RamdomDirections.Upward);
                }
            }
        }

        /// <summary>
        /// Checks if there is a maneuver move valid for a given direction.
        /// </summary>
        /// <param name="direction">Array of desired directions.</param>
        /// <returns>True if there is valid maneuver move for any given direction.</returns>
        private bool CheckIfDirectionIsInMoverField(Vector2[] direction)
        {
            for (int i = 0; i < direction.Length; i++)
            {
                if ((IsInsideRect((direction[i] * maneuverDistance) + (Vector2)transform.position, MoverField)))
                {
                    return true;
                }                    
            }

            return false;
        }

        /// <summary>
        /// Tries to pick a valid move in the given directions.
        /// </summary>
        /// <param name="randomDirections">Array of desire directions.</param>
        /// <returns>True if there is a maneuver move valid for a given directions.</returns>
        private Vector2 pickMove(Vector2[] randomDirections)
        {
            moveAttemptsCounter = MaxMoveAttempts;

            do
            {
                Vector2 pickedRandomDirection = randomDirections[UnityEngine.Random.Range(0, randomDirections.Length)];
                targetPosition = pickedRandomDirection * maneuverDistance + (Vector2)transform.position;
                moveAttemptsCounter--;

                if (IsInsideRect(targetPosition, MoverField))
                {
                    return pickedRandomDirection;
                }
            } 
            while (moveAttemptsCounter > 0);

            return Vector2.zero;
        }

        /// <summary>
        /// Checks if the given position is inside the given rect.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <param name="rect">The field to check if the position is inside it.</param>
        /// <returns>True if the position is inside the rectangle.</returns>
        private bool IsInsideRect(Vector2 position, Rect rect)
        {
            if (position.x < rect.xMax && position.x > rect.xMin &&
                position.y < rect.yMax && position.y > rect.yMin)
            {
                return true;
            }
            else
            {
                return false;
            }              
        }

        private bool IsWithinTargetPositionSqrThreshold()
        {
            if (((Vector2)transform.position - targetPosition).sqrMagnitude <= ProjectConstants.ReacehdPositionSqrThreshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (!isDrawingGizmos)
            {
                return;
            }               

            Gizmos.color = Color.yellow;
            GizmosExtension.DrawCircle(targetPosition, 2f);

            Gizmos.color = new Color(0, 1, 0, 0.5f);
            GizmosExtension.DrawCircle(transform.position, avgColliderWidth * 2f);
            Gizmos.color = Color.cyan;
            GizmosExtension.DrawCircle(transform.position, maneuverDistance);
        }
#endif
    }
}