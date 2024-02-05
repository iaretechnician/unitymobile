using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Rolls the game object when it moves sideways in a vertical level or up and down in a horizontal level.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Movers/Components/Roll By Level Direction")]
    [RequireComponent(typeof(Mover))]
    public class RollByLevelDirection : MonoBehaviour, IRotatable
    {
        /// <summary>
        /// The rotation speed on the roll axis which happens when a mover is moving sideways in a vertical 
        /// level or up and down in a horizontal level.
        /// </summary>
        [Tooltip("The rotation speed on the roll axis which happens when a mover is moving sideways " +
            "in a vertical level or up and down in a horizontal level.")]
        [SerializeField]
        private float rollSpeed;

        /// <summary>
        /// The maximum rotation angle that will be caused by this componenet. A value of zero means it 
        /// will not be limited and the game object will keep rolling until it stops moving.
        /// </summary>
        [Tooltip("The maximum rotation angle that will be caused by this component. A value of zero means it " +
            "will not be limited and the game object will keep rolling until it stops moving.")]
        [SerializeField]
        private float limitAngle;

        private Mover mover;
        private Axis rollAxis;

        private void Awake()
        {
            mover = GetComponent<Mover>();

            if (mover == null)
            {
                Debug.Log("RollByDirection.cs: you have added a roll by level direction component to an" +
                    " object which doesn't have a mover.");

                return;
            }

            CheckLimitAngle();
            FindRollAxis();
            SubscribeToRotationManagerOnRollAxis();
        }

        private void CheckLimitAngle()
        {
            if(limitAngle == 0.0f)
            {
                limitAngle = Mathf.Infinity;
            }
        }

        private void FindRollAxis()
        {
            if (Level.IsVertical)
            {
                rollAxis = Axis.Y;
            }
            else if (Level.IsHorizontal)
            {
                rollAxis = Axis.X;
            }
        }

        private void SubscribeToRotationManagerOnRollAxis()
        {
            RotationManager rotationManager = GetComponent<RotationManager>();

            if (rotationManager == null)
            {
                rotationManager = gameObject.AddComponent<RotationManager>();
            }

            // This will set the facing angle in accordance to the faction this component is applied to.
            if (mover is PlayerMover || mover is MissileMoverFollowingRandomEnemy)
            {
                rotationManager.FacingAngle = Math2D.VectorToDegree(FacingDirections.Player);
            }
            else
            {
                rotationManager.FacingAngle = Math2D.VectorToDegree(FacingDirections.NonPlayer);
            }

            rotationManager.Subscribe(this, rollAxis);
        }

        public float EditRotation(float angle)
        {
            float moverDirection = FindMoverDirectionCausingRoll();

            // If the target isn't moving; the spin angle will gruadually return to zero.
            if (moverDirection == 0.0f)
            {
                angle = ResetAngleToZero(angle);
            }
            else
            {
                angle = RollAccordingToMoverDirection(angle, moverDirection);
            }

            return Mathf.Clamp(angle, -limitAngle, limitAngle);
        }

        private float FindMoverDirectionCausingRoll()
        {
            if (Level.IsVertical)
            {
                return mover.CurrentDirection.x;
            }
            else if (Level.IsHorizontal)
            {
                return mover.CurrentDirection.y;
            }

            return 0.0f;
        }

        private float ResetAngleToZero(float angle)
        {
            if (angle > ProjectConstants.AngleTolerance)
            {
                angle -= Mathf.Abs(rollSpeed) * Time.deltaTime;
            }

            if (angle < -ProjectConstants.AngleTolerance)
            {
                angle += Mathf.Abs(rollSpeed) * Time.deltaTime;
            }

            return angle;
        }

        private float RollAccordingToMoverDirection(float angle, float moverDirection)
        {
            if (moverDirection > 0)
            {
                angle += rollSpeed * Time.deltaTime;
            }

            if (moverDirection < 0)
            {
                angle += -rollSpeed * Time.deltaTime;
            }

            return angle;
        }
    }
}