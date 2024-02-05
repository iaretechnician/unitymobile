using UnityEngine;

namespace ShmupBoss
{
    /// <summary>
    /// Uses input to find the direction and keeps the player inside the boundaries.
    /// </summary>
    [AddComponentMenu("Shmup Boss/Movers/Player Mover")]
    public class PlayerMover : Mover
    {
        /// <summary>
        /// The maximum speed the player can have after speed pickups.
        /// </summary>
        [Tooltip("The maximum speed the player can have after speed pickups.")]
        [SerializeField]
        private float maxSpeed;
        
        public override float CurrentSpeed
        {
            get
            {
                return currentSpeed * CurrentMultiplier.PlayerSpeedMultiplier;
            }
            protected set
            {
                if(value > maxSpeed)
                {
                    currentSpeed = maxSpeed;
                }
                else if(value < speed)
                {
                    currentSpeed = speed;
                }
                else
                {
                    currentSpeed = value;
                }
            }
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if(maxSpeed < speed)
            {
                maxSpeed = speed;
            }
        }

        protected override void FindCurrentDirection()
        {
            CurrentDirection = PlayerInput.Direction;
        }

        protected override void Update()
        {
            base.Update();
            KeepInBoundries();
        }

        private void KeepInBoundries()
        {
            if (PlayField.Instance.Boundries == null)
            {
                return;
            }

            if (PlayField.Instance.Boundries.size == Vector2.zero)
            {
                return;
            }

            transform.position = new Vector3
            (
                Mathf.Clamp(transform.position.x, PlayField.Instance.Boundries.xMin, PlayField.Instance.Boundries.xMax),
                Mathf.Clamp(transform.position.y, PlayField.Instance.Boundries.yMin, PlayField.Instance.Boundries.yMax),
                transform.position.z
            );
        }

        public void AdjustSpeed(float newSpeed)
        {
            CurrentSpeed += newSpeed;
        }
    }
}